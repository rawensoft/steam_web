using System;
using System.Net;
using ProtoBuf;
using SteamWeb.Web;
using System.IO;
using System.Threading.Tasks;
using SteamWeb.Extensions;

namespace SteamWeb.Auth.v2;
public class AuthenticatorLinker
{
    /// <summary>
    /// Set to register a new phone number when linking. If a phone number is not set on the account, this must be set. If a phone number is set on the account, this must be null.
    /// </summary>
    public string PhoneNumber { get; set; } = null;
    /// <summary>
    /// Randomly-generated device ID. Should only be generated once per linker.
    /// </summary>
    public string DeviceID { get; private set; }
    /// <summary>
    /// After the initial link step, if successful, this will be the SteamGuard data for the account. PLEASE save this somewhere after generating it; it's vital data.
    /// </summary>
    public SteamGuardAccount LinkedAccount { get; private set; }
    /// <summary>
    /// True if the authenticator has been fully finalized.
    /// </summary>
    public bool Finalized { get; private set; } = false;

    private readonly SessionData session;
    private readonly IWebProxy proxy;
    private bool allowAuthenticator = false;
    private string countryCode = null;
    private string phoneNumber = null;

    private bool needAddMobile = false;
    private bool isEmailConfirmed = false;
    private bool setNumberExecuted = false;
    private bool isCodeToPhoneNumberSended = false;
    public AuthenticatorLinker(SessionData session, IWebProxy proxy)
    {
        this.session = session;
        this.proxy = proxy;
        DeviceID = GenerateDeviceID();
    }

    // TODO:
    // в AddAuthenticator()
    // добавить проверку на привязаность номера
    // если status ответа 2, то нужно привязать номер
    // для этого нужно вызвать SetAccountPhoneNumber()
    // подтверждение почты проверять через IsAccountWaitingForEmailConfirmation()
    // отправить код через SendPhoneVerificationCode()
    // вызвать AddAuthenticator() и сохранить данные
    // повторная отправка кода также через SendPhoneVerificationCode()
    // код отправить через FinalizeAddAuthenticator()
    // привязка номера и гуарда прошла успешно

    /// <summary>
    /// Изменяет номер телефона
    /// </summary>
    /// <param name="number">формат {countryCodephoneNumber} - без пробелов пробел</param>
    public void SetPhoneNumber(string number)
    {
        phoneNumber = number;
        countryCode = GetCountryCodeByPhoneNumber(number);
        //var splitted = number.Split(' ');
        //if (splitted.Length != 2) throw new ArgumentException("Не обнаружен разделитель 'пробел'", "number");
        //SetPhoneNumber(splitted[0], splitted[1]);
    }
    /// <summary>
    /// Изменяет номер телефона
    /// </summary>
    /// <param name="cCode">код страны (+7)</param>
    /// <param name="pNumber">номер телефона без кода страны</param>
    public void SetPhoneNumber(string cCode, string pNumber)
    {
        phoneNumber = $"{cCode} {pNumber}";
        countryCode = GetCountryCodeByPhoneNumber(pNumber);
    }
    private static string GetCountryCodeByPhoneNumber(string pNumber)
    {
        if (pNumber.StartsWith("+7")) return "RU";
        if (pNumber.StartsWith("+380")) return "UA";
        if (pNumber.StartsWith("+372")) return "EE";
        return null;
    }
    public bool CheckStatus()
    {
        if (session == null)
            return false;
        using var memStream1 = new MemoryStream();
        Serializer.Serialize(memStream1, new CTwoFactor_Status_Request() { steamid = session.SteamID });
        var protoRequest = new ProtobufRequest("https://api.steampowered.com/ITwoFactorService/QueryStatus/v1", Convert.ToBase64String(memStream1.ToArray()))
        {
            AccessToken = session.AccessToken,
            Proxy = proxy,
            UserAgent = SessionData.UserAgentMobile
        };
        using var response = Downloader.PostProtobuf(protoRequest);
        if (!response.Success || response.EResult != EResult.OK)
            return false;
        var wallet = Serializer.Deserialize<CTwoFactor_Status_Response>(response.Stream);
        return allowAuthenticator = wallet.authenticator_allowed;
    }
    public async Task<bool> CheckStatusAsync()
    {
        if (session == null)
            return false;
        using var memStream1 = new MemoryStream();
        Serializer.Serialize(memStream1, new CTwoFactor_Status_Request() { steamid = session.SteamID });
        var protoRequest = new ProtobufRequest("https://api.steampowered.com/ITwoFactorService/QueryStatus/v1", Convert.ToBase64String(memStream1.ToArray()))
        {
            AccessToken = session.AccessToken,
            Proxy = proxy,
            UserAgent = SessionData.UserAgentMobile
        };
        using var response = await Downloader.PostProtobufAsync(protoRequest);
        if (!response.Success || response.EResult != EResult.OK)
            return false;
        var wallet = Serializer.Deserialize<CTwoFactor_Status_Response>(response.Stream);
        return allowAuthenticator = wallet.authenticator_allowed;
    }
    public bool ResendCode() => isCodeToPhoneNumberSended = false;

    /// <summary>
    /// Начало привязывания аутентификатора
    /// </summary>
    /// <returns>
    /// <code>MustProvidePhoneNumber</code> - нужно указать номер телефона и код страны
    /// <code>AuthenticatorNotAllowed</code> - нельзя добавить аутентификатор на этот аккаунт (КТ, уже привязан, etc)
    /// <code>GeneralFailure</code> - нет сессии, ошибки запросов
    /// <code>MustConfirmEmail</code> - нужно подтвердить добавление номера
    /// <code>AwaitingFinalization</code> - нужно перейти к след шагу
    /// <code>TooManyRequests</code> - слишком много было отправлено смс на телефон
    /// </returns>
    public LinkResult AddAuthenticator()
    {
        if (!allowAuthenticator)
            return LinkResult.AuthenticatorNotAllowed;
        if (session == null)
            return LinkResult.GeneralFailure;
        if (needAddMobile && !isEmailConfirmed)
        {
            var emailConfirm = IsAccountWaitingForEmailConfirmation();
            if (emailConfirm == null)
                return LinkResult.GeneralFailure;
            if (emailConfirm.awaiting_email_confirmation)
                return LinkResult.MustConfirmEmail;
            isEmailConfirmed = true;
        }
        if ((needAddMobile && isEmailConfirmed && !isCodeToPhoneNumberSended) ||
            (!needAddMobile && !isCodeToPhoneNumberSended))
        {
            var sendPhoneVerifCode = SendPhoneVerificationCode(new() { language = 0 });
            if (sendPhoneVerifCode == SENDVERIFCODE.BadSession ||
                sendPhoneVerifCode == SENDVERIFCODE.Error ||
                sendPhoneVerifCode == SENDVERIFCODE.No)
                return LinkResult.GeneralFailure;
            if (sendPhoneVerifCode == SENDVERIFCODE.TooManyRequests)
                return LinkResult.TooManyRequests;
            isCodeToPhoneNumberSended = true;
        }

        using var memStream1 = new MemoryStream();
        Serializer.Serialize(memStream1, new CTwoFactor_AddAuthenticator_Request()
        {
            steamid = session.SteamID,
            authenticator_type = 1,
            device_identifier = DeviceID,
            sms_phone_id = "1",
            version = 2
        });
        string content = Convert.ToBase64String(memStream1.ToArray());
        var protoRequest = new ProtobufRequest($"https://api.steampowered.com/ITwoFactorService/AddAuthenticator/v1", content)
        {
            AccessToken = session.AccessToken,
            Proxy = proxy,
            UserAgent = SessionData.UserAgentMobile
        };
        using var response = Downloader.PostProtobuf(protoRequest);
        if (!response.Success || response.EResult != EResult.OK)
        {
            if (response.EResult == EResult.NoVerifiedPhone && !setNumberExecuted)
            {
                if (countryCode.IsEmpty() || phoneNumber.IsEmpty())
                    return LinkResult.MustProvidePhoneNumber;
                var responseSetNumber = SetAccountPhoneNumber(new() { phone_number = phoneNumber, phone_country_code = countryCode });
                if (responseSetNumber == null)
                    return LinkResult.GeneralFailure;
                setNumberExecuted = true;
                needAddMobile = true;
                phoneNumber = responseSetNumber.phone_number_formatted;
                return LinkResult.MustConfirmEmail;
            }
            return LinkResult.GeneralFailure;
        }
        var addAuthenticatorResponse = Serializer.Deserialize<CTwoFactor_AddAuthenticator_Response>(response.Stream);

        if (addAuthenticatorResponse.status == 2)
        {
            if (!setNumberExecuted)
            {
                var responseSetNumber = SetAccountPhoneNumber(new() { phone_number = phoneNumber, phone_country_code = countryCode });
                if (responseSetNumber == null)
                    return LinkResult.GeneralFailure;
                setNumberExecuted = true;
                needAddMobile = true;
                phoneNumber = responseSetNumber.phone_number_formatted;
                return LinkResult.MustConfirmEmail;
            }
            return LinkResult.GeneralFailure;
        }
        if (addAuthenticatorResponse.status == 29)
            return LinkResult.AuthenticatorPresent;
        if (addAuthenticatorResponse.status != 1)
            return LinkResult.GeneralFailure;

        LinkedAccount = new()
        {
            AccountName = addAuthenticatorResponse.account_name,
            RevocationCode = addAuthenticatorResponse.revocation_code,
            DeviceID = DeviceID,
            IdentitySecret = Convert.ToBase64String(addAuthenticatorResponse.identity_secret),
            Secret1 = Convert.ToBase64String(addAuthenticatorResponse.secret_1),
            SerialNumber = addAuthenticatorResponse.serial_number,
            ServerTime = addAuthenticatorResponse.server_time,
            SharedSecret = Convert.ToBase64String(addAuthenticatorResponse.shared_secret),
            Status = addAuthenticatorResponse.status,
            Session = session,
            Proxy = proxy,
            TokenGID = addAuthenticatorResponse.token_gid,
            URI = addAuthenticatorResponse.uri,
        };
        return LinkResult.AwaitingFinalization;
    }
    /// <summary>
    /// Завершает привязку аунтентификатора
    /// </summary>
    /// <param name="smsCode"></param>
    /// <returns>
    /// <code>AuthenticatorNotAllowed</code> - нельзя добавить аутентификатор на этот аккаунт (КТ, уже привязан, etc)
    /// <code>GeneralFailure</code> - нет сессии, ошибки запросов
    /// <code>BadSMSCode</code> - не верный код из смс
    /// <code>UnableToGenerateCorrectCodes</code> - невозможно нормально сгенерировать 2FA коды
    /// <code>Success</code> - аунтентификатор привязан
    /// </returns>
    public FinalizeResult FinalizeAddAuthenticator(string smsCode)
    {
        if (!allowAuthenticator)
            return FinalizeResult.AuthenticatorNotAllowed;
        //The act of checking the SMS code is necessary for Steam to finalize adding the phone number to the account.
        //Of course, we only want to check it if we're adding a phone number in the first place...

        if (session == null)
            return FinalizeResult.GeneralFailure;
        using var memStream1 = new MemoryStream();
        var request = new CTwoFactor_FinalizeAddAuthenticator_Request()
        {
            steamid = session.SteamID,
            activation_code = smsCode,
            validate_sms_code = true
        };
        int tries = 0;
        while (tries <= 30)
        {
            request.authenticator_code = LinkedAccount.GenerateSteamGuardCode();
            request.authenticator_time = TimeAligner.GetSteamTime();
            Serializer.Serialize(memStream1, request);
            string content = Convert.ToBase64String(memStream1.ToArray());

            var protoRequest = new ProtobufRequest("https://api.steampowered.com/ITwoFactorService/FinalizeAddAuthenticator/v1", content)
            {
                AccessToken = session.AccessToken,
                Proxy = proxy,
                UserAgent = SessionData.UserAgentMobile
            };
            using var response = Downloader.PostProtobuf(protoRequest);
            if (!response.Success || response.EResult != EResult.OK) return FinalizeResult.GeneralFailure;
            var finalizeResponse = Serializer.Deserialize<CTwoFactor_FinalizeAddAuthenticator_Response>(response.Stream);

            if (finalizeResponse.status == 89)
                return FinalizeResult.BadSMSCode;
            if (finalizeResponse.status == 88 && tries >= 30)
                return FinalizeResult.UnableToGenerateCorrectCodes;
            if (!finalizeResponse.success)
                return FinalizeResult.GeneralFailure;
            if (finalizeResponse.want_more)
            {
                request.validate_sms_code = false;
                request.activation_code = null;
                tries++;
                continue;
            }

            LinkedAccount.FullyEnrolled = true;
            return FinalizeResult.Success;
        }

        return FinalizeResult.GeneralFailure;
    }

    /// <summary>
    /// Начало привязывания аутентификатора
    /// </summary>
    /// <returns>
    /// <code>MustProvidePhoneNumber</code> - нужно указать номер телефона и код страны
    /// <code>AuthenticatorNotAllowed</code> - нельзя добавить аутентификатор на этот аккаунт (КТ, уже привязан, etc)
    /// <code>GeneralFailure</code> - нет сессии, ошибки запросов
    /// <code>MustConfirmEmail</code> - нужно подтвердить добавление номера
    /// <code>AwaitingFinalization</code> - нужно перейти к след шагу
    /// <code>TooManyRequests</code> - слишком много было отправлено смс на телефон
    /// </returns>
    public async Task<LinkResult> AddAuthenticatorAsync()
    {
        if (!allowAuthenticator)
            return LinkResult.AuthenticatorNotAllowed;
        if (session == null)
            return LinkResult.GeneralFailure;
        if (needAddMobile && !isEmailConfirmed)
        {
            var emailConfirm = await IsAccountWaitingForEmailConfirmationAsync();
            if (emailConfirm == null)
                return LinkResult.GeneralFailure;
            if (emailConfirm.awaiting_email_confirmation)
                return LinkResult.MustConfirmEmail;
            isEmailConfirmed = true;
        }
        if ((needAddMobile && isEmailConfirmed && !isCodeToPhoneNumberSended) ||
            (!needAddMobile && !isCodeToPhoneNumberSended))
        {
            var sendPhoneVerifCode = await SendPhoneVerificationCodeAsync(new() { language = 0 });
            if (sendPhoneVerifCode == SENDVERIFCODE.BadSession ||
                sendPhoneVerifCode == SENDVERIFCODE.Error ||
                sendPhoneVerifCode == SENDVERIFCODE.No)
                return LinkResult.GeneralFailure;
            if (sendPhoneVerifCode == SENDVERIFCODE.TooManyRequests)
                return LinkResult.TooManyRequests;
            isCodeToPhoneNumberSended = true;
        }

        using var memStream1 = new MemoryStream();
        Serializer.Serialize(memStream1, new CTwoFactor_AddAuthenticator_Request()
        {
            steamid = session.SteamID,
            authenticator_type = 1,
            device_identifier = DeviceID,
            sms_phone_id = "1",
            version = 2
        });
        var protoRequest = new ProtobufRequest($"https://api.steampowered.com/ITwoFactorService/AddAuthenticator/v1", Convert.ToBase64String(memStream1.ToArray()))
        {
            AccessToken = session.AccessToken,
            Proxy = proxy,
            UserAgent = SessionData.UserAgentMobile
        };
        using var response = await Downloader.PostProtobufAsync(protoRequest);
        if (!response.Success || response.EResult != EResult.OK)
        {
            if (response.EResult == EResult.NoVerifiedPhone && !setNumberExecuted)
            {
                if (countryCode.IsEmpty() || phoneNumber.IsEmpty())
                    return LinkResult.MustProvidePhoneNumber;
                var responseSetNumber = await SetAccountPhoneNumberAsync(new() { phone_number = phoneNumber, phone_country_code = countryCode });
                if (responseSetNumber == null)
                    return LinkResult.GeneralFailure;
                setNumberExecuted = true;
                needAddMobile = true;
                phoneNumber = responseSetNumber.phone_number_formatted;
                return LinkResult.MustConfirmEmail;
            }
            return LinkResult.GeneralFailure;
        }
        var addAuthenticatorResponse = Serializer.Deserialize<CTwoFactor_AddAuthenticator_Response>(response.Stream);

        if (addAuthenticatorResponse.status == 2)
        {
            if (!setNumberExecuted)
            {
                var responseSetNumber = await SetAccountPhoneNumberAsync(new() { phone_number = phoneNumber, phone_country_code = countryCode });
                if (responseSetNumber == null)
                    return LinkResult.GeneralFailure;
                setNumberExecuted = true;
                needAddMobile = true;
                phoneNumber = responseSetNumber.phone_number_formatted;
                return LinkResult.MustConfirmEmail;
            }
            return LinkResult.GeneralFailure;
        }
        if (addAuthenticatorResponse.status == 29)
            return LinkResult.AuthenticatorPresent;
        if (addAuthenticatorResponse.status != 1)
            return LinkResult.GeneralFailure;

        LinkedAccount = new()
        {
            AccountName = addAuthenticatorResponse.account_name,
            RevocationCode = addAuthenticatorResponse.revocation_code,
            DeviceID = DeviceID,
            IdentitySecret = Convert.ToBase64String(addAuthenticatorResponse.identity_secret),
            Secret1 = Convert.ToBase64String(addAuthenticatorResponse.secret_1),
            SerialNumber = addAuthenticatorResponse.serial_number,
            ServerTime = addAuthenticatorResponse.server_time,
            SharedSecret = Convert.ToBase64String(addAuthenticatorResponse.shared_secret),
            Status = addAuthenticatorResponse.status,
            Session = session,
            Proxy = proxy,
            TokenGID = addAuthenticatorResponse.token_gid,
            URI = addAuthenticatorResponse.uri,
        };
        return LinkResult.AwaitingFinalization;
    }
    /// <summary>
    /// Завершает привязку аунтентификатора
    /// </summary>
    /// <param name="smsCode"></param>
    /// <returns>
    /// <code>AuthenticatorNotAllowed</code> - нельзя добавить аутентификатор на этот аккаунт (КТ, уже привязан, etc)
    /// <code>GeneralFailure</code> - нет сессии, ошибки запросов
    /// <code>BadSMSCode</code> - не верный код из смс
    /// <code>UnableToGenerateCorrectCodes</code> - невозможно нормально сгенерировать 2FA коды
    /// <code>Success</code> - аунтентификатор привязан
    /// </returns>
    public async Task<FinalizeResult> FinalizeAddAuthenticatorAsync(string smsCode)
    {
        if (!allowAuthenticator)
            return FinalizeResult.AuthenticatorNotAllowed;
        //The act of checking the SMS code is necessary for Steam to finalize adding the phone number to the account.
        //Of course, we only want to check it if we're adding a phone number in the first place...

        if (session == null)
            return FinalizeResult.GeneralFailure;
        using var memStream1 = new MemoryStream();
        var request = new CTwoFactor_FinalizeAddAuthenticator_Request()
        {
            steamid = session.SteamID,
            activation_code = smsCode,
            validate_sms_code = true
        };
        int tries = 0;
        while (tries <= 30)
        {
            request.authenticator_code = LinkedAccount.GenerateSteamGuardCode();
            request.authenticator_time = TimeAligner.GetSteamTime();
            Serializer.Serialize(memStream1, request);
            string content = Convert.ToBase64String(memStream1.ToArray());

            var protoRequest = new ProtobufRequest("https://api.steampowered.com/ITwoFactorService/FinalizeAddAuthenticator/v1", content)
            {
                AccessToken = session.AccessToken,
                Proxy = proxy,
                UserAgent = SessionData.UserAgentMobile
            };
            using var response = await Downloader.PostProtobufAsync(protoRequest);
            if (!response.Success || response.EResult != EResult.OK) return FinalizeResult.GeneralFailure;
            var finalizeResponse = Serializer.Deserialize<CTwoFactor_FinalizeAddAuthenticator_Response>(response.Stream);

            if (finalizeResponse.status == 89)
                return FinalizeResult.BadSMSCode;
            if (finalizeResponse.status == 88 && tries >= 30)
                return FinalizeResult.UnableToGenerateCorrectCodes;
            if (!finalizeResponse.success)
                return FinalizeResult.GeneralFailure;
            if (finalizeResponse.want_more)
            {
                request.validate_sms_code = false;
                request.activation_code = null;
                tries++;
                continue;
            }

            LinkedAccount.FullyEnrolled = true;
            return FinalizeResult.Success;
        }

        return FinalizeResult.GeneralFailure;
    }

    private CPhone_SetAccountPhoneNumber_Response SetAccountPhoneNumber(CPhone_SetAccountPhoneNumber_Request request)
    {
        if (session == null)
            return null;
        using var memStream1 = new MemoryStream();
        Serializer.Serialize(memStream1, request);
        var protoRequest = new ProtobufRequest("https://api.steampowered.com/IPhoneService/SetAccountPhoneNumber/v1", Convert.ToBase64String(memStream1.ToArray()))
        {
            AccessToken = session.AccessToken,
            Proxy = proxy,
            UserAgent = SessionData.UserAgentMobile
        };
        using var response = Downloader.PostProtobuf(protoRequest);
        if (!response.Success || (response.EResult != EResult.OK && response.EResult != EResult.Pending))
            return null;
        var obj = Serializer.Deserialize<CPhone_SetAccountPhoneNumber_Response>(response.Stream);
        return obj;
    }
    private CPhone_IsAccountWaitingForEmailConfirmation_Response IsAccountWaitingForEmailConfirmation()
    {
        if (session == null)
            return null;
        var protoRequest = new ProtobufRequest("https://api.steampowered.com/IPhoneService/IsAccountWaitingForEmailConfirmation/v1", "")
        {
            AccessToken = session.AccessToken,
            Proxy = proxy,
            UserAgent = SessionData.UserAgentMobile
        };
        using var response = Downloader.PostProtobuf(protoRequest);
        if (!response.Success || response.EResult != EResult.OK)
            return null;
        var obj = Serializer.Deserialize<CPhone_IsAccountWaitingForEmailConfirmation_Response>(response.Stream);
        return obj;
    }
    private SENDVERIFCODE SendPhoneVerificationCode(CPhone_SendPhoneVerificationCode_Request request)
    {
        if (session == null)
            return SENDVERIFCODE.BadSession;
        using var memStream1 = new MemoryStream();
        Serializer.Serialize(memStream1, request);
        var protoRequest = new ProtobufRequest("https://api.steampowered.com/IPhoneService/SendPhoneVerificationCode/v1", Convert.ToBase64String(memStream1.ToArray()))
        {
            AccessToken = session.AccessToken,
            Proxy = proxy,
            UserAgent = SessionData.UserAgentMobile
        };
        using var response = Downloader.PostProtobuf(protoRequest);
        if (response.EResult == EResult.RateLimitExceeded)
            return SENDVERIFCODE.TooManyRequests;
        if (response.EResult == EResult.InvalidState)
            return SENDVERIFCODE.InvalidState;
        if (!response.Success)
            return SENDVERIFCODE.Error;
        if (response.EResult == EResult.OK)
            return SENDVERIFCODE.Yes;
        return SENDVERIFCODE.No;
    }
    private SENDVERIFCODE VerifyAccountPhoneWithCode(CPhone_VerifyAccountPhoneWithCode_Request request)
    {
        if (session == null)
            return SENDVERIFCODE.BadSession;
        using var memStream1 = new MemoryStream();
        Serializer.Serialize(memStream1, request);
        var protoRequest = new ProtobufRequest("https://api.steampowered.com/IPhoneService/VerifyAccountPhoneWithCode/v1", Convert.ToBase64String(memStream1.ToArray()))
        {
            AccessToken = session.AccessToken,
            Proxy = proxy,
            UserAgent = SessionData.UserAgentMobile
        };
        using var response = Downloader.PostProtobuf(protoRequest);
        if (!response.Success || response.EResult != EResult.OK)
            return SENDVERIFCODE.Error;
        if (response.EResult == EResult.OK)
            return SENDVERIFCODE.Yes;
        return SENDVERIFCODE.No;
    }

    private async Task<CPhone_SetAccountPhoneNumber_Response> SetAccountPhoneNumberAsync(CPhone_SetAccountPhoneNumber_Request request)
    {
        if (session == null)
            return null;
        using var memStream1 = new MemoryStream();
        Serializer.Serialize(memStream1, request);
        var protoRequest = new ProtobufRequest("https://api.steampowered.com/IPhoneService/SetAccountPhoneNumber/v1", Convert.ToBase64String(memStream1.ToArray()))
        {
            AccessToken = session.AccessToken,
            Proxy = proxy,
            UserAgent = SessionData.UserAgentMobile
        };
        using var response = await Downloader.PostProtobufAsync(protoRequest);
        if (!response.Success || (response.EResult != EResult.OK && response.EResult != EResult.Pending))
            return null;
        var obj = Serializer.Deserialize<CPhone_SetAccountPhoneNumber_Response>(response.Stream);
        return obj;
    }
    private async Task<CPhone_IsAccountWaitingForEmailConfirmation_Response> IsAccountWaitingForEmailConfirmationAsync()
    {
        if (session == null)
            return null;
        var protoRequest = new ProtobufRequest("https://api.steampowered.com/IPhoneService/IsAccountWaitingForEmailConfirmation/v1", "")
        {
            AccessToken = session.AccessToken,
            Proxy = proxy,
            UserAgent = SessionData.UserAgentMobile
        };
        using var response = await Downloader.PostProtobufAsync(protoRequest);
        if (!response.Success || response.EResult != EResult.OK)
            return null;
        var obj = Serializer.Deserialize<CPhone_IsAccountWaitingForEmailConfirmation_Response>(response.Stream);
        return obj;
    }
    private async Task<SENDVERIFCODE> SendPhoneVerificationCodeAsync(CPhone_SendPhoneVerificationCode_Request request)
    {
        if (session == null)
            return SENDVERIFCODE.BadSession;
        using var memStream1 = new MemoryStream();
        Serializer.Serialize(memStream1, request);
        var protoRequest = new ProtobufRequest("https://api.steampowered.com/IPhoneService/SendPhoneVerificationCode/v1", Convert.ToBase64String(memStream1.ToArray()))
        {
            AccessToken = session.AccessToken,
            Proxy = proxy,
            UserAgent = SessionData.UserAgentMobile
        };
        using var response = await Downloader.PostProtobufAsync(protoRequest);
        if (response.EResult == EResult.RateLimitExceeded)
            return SENDVERIFCODE.TooManyRequests;
        if (response.EResult == EResult.InvalidState)
            return SENDVERIFCODE.InvalidState;
        if (!response.Success)
            return SENDVERIFCODE.Error;
        if (response.EResult == EResult.OK)
            return SENDVERIFCODE.Yes;
        return SENDVERIFCODE.No;
    }
    private async Task<SENDVERIFCODE> VerifyAccountPhoneWithCodeAsync(CPhone_VerifyAccountPhoneWithCode_Request request)
    {
        if (session == null)
            return SENDVERIFCODE.BadSession;
        using var memStream1 = new MemoryStream();
        Serializer.Serialize(memStream1, request);
        var protoRequest = new ProtobufRequest("https://api.steampowered.com/IPhoneService/VerifyAccountPhoneWithCode/v1", Convert.ToBase64String(memStream1.ToArray()))
        {
            AccessToken = session.AccessToken,
            Proxy = proxy,
            UserAgent = SessionData.UserAgentMobile
        };
        using var response = await Downloader.PostProtobufAsync(protoRequest);
        if (!response.Success || response.EResult != EResult.OK)
            return SENDVERIFCODE.Error;
        if (response.EResult == EResult.OK)
            return SENDVERIFCODE.Yes;
        return SENDVERIFCODE.No;
    }

    public static string GenerateDeviceID()
    {
        return "android:" + Guid.NewGuid().ToString();
    }
}
enum SENDVERIFCODE
{
    Yes,
    No,
    BadSession,
    Error,
    TooManyRequests,
    InvalidState
}
[ProtoContract]
class CTwoFactor_Status_Request
{
    [ProtoMember(1, DataFormat = DataFormat.FixedSize)] public ulong steamid { get; set; }
}
[ProtoContract]
class CTwoFactor_Status_Response
{
    /// <summary>
    /// Authenticator state
    /// </summary>
    [ProtoMember(1)] public int state { get; set; }
    /// <summary>
    /// Inactivation reason (if any)
    /// </summary>
    [ProtoMember(2)] public int inactivation_reason { get; set; }
    /// <summary>
    /// Type of authenticator
    /// </summary>
    [ProtoMember(3)] public int authenticator_type { get; set; }
    /// <summary>
    /// Account allowed to have an authenticator?
    /// </summary>
    [ProtoMember(4)] public bool authenticator_allowed { get; set; } = false;
    /// <summary>
    /// Steam Guard scheme in effect
    /// </summary>
    [ProtoMember(5)] public int steamguard_scheme { get; set; }
    /// <summary>
    /// String rep of token GID assigned by server
    /// </summary>
    [ProtoMember(6)] public string token_gid { get; set; }
    /// <summary>
    /// Account has verified email capability
    /// </summary>
    [ProtoMember(7)] public bool email_validated { get; set; } = false;
    /// <summary>
    /// Authenticator (phone) identifier
    /// </summary>
    [ProtoMember(8)] public string device_identifier { get; set; }
    /// <summary>
    /// When the token was created
    /// </summary>
    [ProtoMember(9)] public int time_created { get; set; }
    /// <summary>
    /// Number of revocation code attempts remaining
    /// </summary>
    [ProtoMember(10)] public int revocation_attempts_remaining { get; set; }
    /// <summary>
    /// Agent that added the authenticator (e.g., ios / android / other)
    /// </summary>
    [ProtoMember(11)] public string classified_agent { get; set; }
    /// <summary>
    /// Allow a third-party authenticator (in addition to two-factor)
    /// </summary>
    [ProtoMember(12)] public bool allow_external_authenticator { get; set; } = false;
    /// <summary>
    /// When the token was transferred from another device, if applicable
    /// </summary>
    [ProtoMember(13)] public int time_transferred { get; set; }
}

[ProtoContract]
class CTwoFactor_AddAuthenticator_Request
{
    [ProtoMember(1, DataFormat = DataFormat.FixedSize)] public ulong steamid { get; set; }
    //[ProtoMember(2)] public ulong authenticator_time { get; set; }
    //[ProtoMember(3)] public ulong serial_number { get; set; }
    [ProtoMember(4)] public uint authenticator_type { get; set; } = 1;
    /// <summary>
    /// DeviceID, example: android:c4397b5c-d49a-4987-80dc-70e8129c61d0
    /// </summary>
    [ProtoMember(5)] public string device_identifier { get; set; }
    [ProtoMember(6)] public string sms_phone_id { get; set; } = "1";
    //[ProtoMember(7)] public string http_headers { get; set; }
    /// <summary>
    /// What the version of our token should be
    /// </summary>
    [ProtoMember(8)] public uint version { get; set; } = 2;
}
[ProtoContract]
class CTwoFactor_AddAuthenticator_Response
{
    /// <summary>
    /// Shared secret between server and authenticator
    /// </summary>
    [ProtoMember(1)] public byte[] shared_secret { get; set; }
    /// <summary>
    /// Authenticator serial number (unique per token)
    /// </summary>
    [ProtoMember(2, DataFormat = DataFormat.FixedSize)] public ulong serial_number { get; set; }
    [ProtoMember(3)] public string revocation_code { get; set; }
    /// <summary>
    /// URI for QR code generation
    /// </summary>
    [ProtoMember(4)] public string uri { get; set; }
    [ProtoMember(5)] public int server_time { get; set; }
    [ProtoMember(6)] public string account_name { get; set; }
    /// <summary>
    /// Token GID assigned by server
    /// </summary>
    [ProtoMember(7)] public string token_gid { get; set; }
    /// <summary>
    /// Secret used for identity attestation (e.g., for eventing)
    /// </summary>
    [ProtoMember(8)] public byte[] identity_secret { get; set; }
    /// <summary>
    /// Spare shared secret
    /// </summary>
    [ProtoMember(9)] public byte[] secret_1 { get; set; }
    /// <summary>
    /// Result code - 2 - нужно привязать номер(?), 1 - ожидание ввода смс кода(?)
    /// </summary>
    [ProtoMember(10)] public int status { get; set; }
    /// <summary>
    /// a portion of the phone number the SMS code was sent to
    /// </summary>
    [ProtoMember(11)] public string phone_number_hint { get; set; }
}

[ProtoContract]
class CTwoFactor_FinalizeAddAuthenticator_Request
{
    [ProtoMember(1, DataFormat = DataFormat.FixedSize)] public ulong steamid { get; set; }
    /// <summary>
    /// 2FA code
    /// </summary>
    [ProtoMember(2)] public string authenticator_code { get; set; }
    /// <summary>
    /// Текущее время клиента
    /// </summary>
    [ProtoMember(3)] public int authenticator_time { get; set; }
    /// <summary>
    /// Activation code from out-of-band message
    /// </summary>
    [ProtoMember(4)] public string activation_code { get; set; }
    //[ProtoMember(5)] public string http_headers { get; set; }
    /// <summary>
    /// When finalizing with an SMS code, pass the request on to the PhoneService to update its state too.
    /// </summary>
    [ProtoMember(6)]
    public bool validate_sms_code { get; set; } = true;
}
[ProtoContract]
class CTwoFactor_FinalizeAddAuthenticator_Response
{
    /// <summary>
    /// True if succeeded, or want more tries
    /// </summary>
    [ProtoMember(1)] public bool success { get; set; }
    /// <summary>
    /// True if want more tries
    /// </summary>
    [ProtoMember(2)] public bool want_more { get; set; }
    /// <summary>
    /// Current server time
    /// </summary>
    [ProtoMember(3)] public int server_time { get; set; }
    /// <summary>
    /// Result code. 2 - это success=true
    /// </summary>
    [ProtoMember(4)] public int status { get; set; }
}

[ProtoContract]
class CPhone_SetAccountPhoneNumber_Request
{
    /// <summary>
    /// example: +7 9773130028
    /// </summary>
    [ProtoMember(1)] public string phone_number { get; set; }
    /// <summary>
    /// example: RU
    /// </summary>
    [ProtoMember(2)] public string phone_country_code { get; set; }
}
[ProtoContract]
class CPhone_SetAccountPhoneNumber_Response
{
    [ProtoMember(1)] public string confirmation_email_address { get; set; }
    [ProtoMember(2)] public string phone_number_formatted { get; set; }
}

[ProtoContract]
class CPhone_IsAccountWaitingForEmailConfirmation_Response
{
    /// <summary>
    /// Ожидать ли подтверждения почты?
    /// </summary>
    [ProtoMember(1)] public bool awaiting_email_confirmation { get; set; }
    /// <summary>
    /// Через сколько делать следующий запрос в сек
    /// </summary>
    [ProtoMember(2)] public int seconds_to_wait { get; set; }
}

[ProtoContract]
class CPhone_SendPhoneVerificationCode_Request
{
    [ProtoMember(1)] public int language { get; set; } = 0;
}
[ProtoContract]
class CPhone_SendPhoneVerificationCode_Response { }

[ProtoContract]
class CPhone_VerifyAccountPhoneWithCode_Request
{
    [ProtoMember(1)] public string code { get; set; }
}
[ProtoContract]
class CPhone_VerifyAccountPhoneWithCode_Response { }