using System.Net;
using ProtoBuf;
using SteamWeb.Web;
using SteamWeb.Extensions;
using SteamWeb.Auth.v2.Enums;
using LinkResult = SteamWeb.Auth.v1.Enums.LinkResult;
using FinalizeResult = SteamWeb.Auth.v1.Enums.FinalizeResult;
using TimeAligner = SteamWeb.Auth.v1.TimeAligner;
using SteamWeb.Auth.v2.Models;

namespace SteamWeb.Auth.v2;
public class AuthenticatorLinker
{
    /// <summary>
    /// Set to register a new phone number when linking. If a phone number is not set on the account, this must be set. If a phone number is set on the account, this must be null.
    /// </summary>
    public string? PhoneNumber { get; set; } = null;
    /// <summary>
    /// Randomly-generated device ID. Should only be generated once per linker.
    /// </summary>
    public string DeviceID { get; private set; }
    /// <summary>
    /// After the initial link step, if successful, this will be the SteamGuard data for the account. PLEASE save this somewhere after generating it; it's vital data.
    /// </summary>
    public SteamGuardAccount? LinkedAccount { get; private set; }
    /// <summary>
    /// True if the authenticator has been fully finalized.
    /// </summary>
    public bool Finalized { get; private set; } = false;

    private readonly SessionData session;
    private readonly IWebProxy? proxy;
    private bool allowAuthenticator = false;
    private string? countryCode = null;
    private string? phoneNumber = null;

    private bool needAddMobile = false;
    private bool isEmailConfirmed = false;
    private bool setNumberExecuted = false;
    private bool isCodeToPhoneNumberSended = false;
    public AuthenticatorLinker(SessionData session, IWebProxy? proxy)
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
    /// <param name="number">формат {countryCodephoneNumber} - без пробелов пробел (+7 9631234455)</param>
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
        phoneNumber = cCode + " " + pNumber;
        countryCode = GetCountryCodeByPhoneNumber(pNumber);
    }
    private static string? GetCountryCodeByPhoneNumber(string pNumber)
    {
        if (pNumber.StartsWith("+7"))
            return "RU";
        if (pNumber.StartsWith("+380"))
            return "UA";
        if (pNumber.StartsWith("+372"))
            return "EE";
        return null;
    }
    public bool CheckStatus()
    {
        if (session == null)
            return false;
        using var memStream1 = new MemoryStream();
        Serializer.Serialize(memStream1, new CTwoFactor_Status_Request() { steamid = session.SteamID });
        var protoRequest = new ProtobufRequest(SteamApiUrls.ITwoFactorService_QueryStatus_v1, Convert.ToBase64String(memStream1.ToArray()))
        {
            AccessToken = session.AccessToken,
            Proxy = proxy,
            UserAgent = KnownUserAgents.OkHttp
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
        var protoRequest = new ProtobufRequest(SteamApiUrls.ITwoFactorService_QueryStatus_v1, Convert.ToBase64String(memStream1.ToArray()))
        {
            AccessToken = session.AccessToken,
            Proxy = proxy,
            UserAgent = KnownUserAgents.OkHttp
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
    public LinkResult AddAuthenticatorMobile()
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
        Serializer.Serialize(memStream1, new CTwoFactor_AddAuthenticator_Request_Mobile()
        {
            steamid = session.SteamID,
            authenticator_type = 1,
            device_identifier = DeviceID,
            sms_phone_id = "1",
            version = 2
        });
        string content = Convert.ToBase64String(memStream1.ToArray());
        var protoRequest = new ProtobufRequest(SteamApiUrls.ITwoFactorService_AddAuthenticator_v1, content)
        {
            AccessToken = session.AccessToken,
            Proxy = proxy,
            UserAgent = KnownUserAgents.OkHttp
		};
        using var response = Downloader.PostProtobuf(protoRequest);
        if (!response.Success || response.EResult != EResult.OK)
        {
            if (response.EResult == EResult.NoVerifiedPhone && !setNumberExecuted)
            {
                if (countryCode.IsEmpty() || phoneNumber.IsEmpty())
                    return LinkResult.MustProvidePhoneNumber;
                var responseSetNumber = SetAccountPhoneNumber(new() { phone_number = phoneNumber!, phone_country_code = countryCode! });
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
                var responseSetNumber = SetAccountPhoneNumber(new() { phone_number = phoneNumber!, phone_country_code = countryCode! });
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
			AddedThrough = ADD_THROUGH.PhoneNumber
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
    public FinalizeResult FinalizeAddAuthenticatorMobile(string smsCode)
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

            var protoRequest = new ProtobufRequest(SteamApiUrls.ITwoFactorService_FinalizeAddAuthenticator_v1, content)
            {
                AccessToken = session.AccessToken,
                Proxy = proxy,
                UserAgent = KnownUserAgents.OkHttp
			};
            using var response = Downloader.PostProtobuf(protoRequest);
            if (!response.Success || response.EResult != EResult.OK) return FinalizeResult.GeneralFailure;
            var finalizeResponse = Serializer.Deserialize<CTwoFactor_FinalizeAddAuthenticator_Response>(response.Stream);

            if (finalizeResponse.status == 89)
                return FinalizeResult.BadSMSCode;
            if (finalizeResponse.status == 88 && tries >= 30)
                return FinalizeResult.UnableToGenerateCorrectCodes;
            if (!finalizeResponse.success)
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
    public async Task<LinkResult> AddAuthenticatorMobileAsync()
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
		Serializer.Serialize(memStream1, new CTwoFactor_AddAuthenticator_Request_Mobile()
		{
			steamid = session.SteamID,
			authenticator_type = 1,
			device_identifier = DeviceID,
			sms_phone_id = "1",
			version = 2
		});
		string content = Convert.ToBase64String(memStream1.ToArray());
		var protoRequest = new ProtobufRequest(SteamApiUrls.ITwoFactorService_AddAuthenticator_v1, content)
		{
			AccessToken = session.AccessToken,
			Proxy = proxy,
			UserAgent = KnownUserAgents.OkHttp
		};
		using var response = await Downloader.PostProtobufAsync(protoRequest);
		if (!response.Success || response.EResult != EResult.OK)
		{
			if (response.EResult == EResult.NoVerifiedPhone && !setNumberExecuted)
			{
				if (countryCode.IsEmpty() || phoneNumber.IsEmpty())
					return LinkResult.MustProvidePhoneNumber;
				var responseSetNumber = await SetAccountPhoneNumberAsync(new() { phone_number = phoneNumber!, phone_country_code = countryCode! });
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
				var responseSetNumber = await SetAccountPhoneNumberAsync(new() { phone_number = phoneNumber!, phone_country_code = countryCode! });
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
			AddedThrough = ADD_THROUGH.PhoneNumber
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
    public async Task<FinalizeResult> FinalizeAddAuthenticatorMobileAsync(string smsCode)
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

			var protoRequest = new ProtobufRequest(SteamApiUrls.ITwoFactorService_FinalizeAddAuthenticator_v1, content)
			{
				AccessToken = session.AccessToken,
				Proxy = proxy,
				UserAgent = KnownUserAgents.OkHttp
			};
			using var response = await Downloader.PostProtobufAsync(protoRequest);
			if (!response.Success || response.EResult != EResult.OK)
				return FinalizeResult.GeneralFailure;
			var finalizeResponse = Serializer.Deserialize<CTwoFactor_FinalizeAddAuthenticator_Response>(response.Stream);

			if (finalizeResponse.status == 89)
				return FinalizeResult.BadSMSCode;
			if (finalizeResponse.status == 88 && tries >= 30)
				return FinalizeResult.UnableToGenerateCorrectCodes;
			if (!finalizeResponse.success)
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


	public LinkResult AddAuthenticatorEmail()
	{
		using var memStream1 = new MemoryStream();
		Serializer.Serialize(memStream1, new CTwoFactor_AddAuthenticator_Request_Email()
		{
			steamid = session.SteamID,
			device_identifier = DeviceID,
		});
		string content = Convert.ToBase64String(memStream1.ToArray());
		var protoRequest = new ProtobufRequest(SteamApiUrls.ITwoFactorService_AddAuthenticator_v1, content)
		{
			AccessToken = session.AccessToken,
			Proxy = proxy,
			UserAgent = KnownUserAgents.OkHttp
		};
		using var response = Downloader.PostProtobuf(protoRequest);
		if (!response.Success || response.EResult != EResult.OK)
		{
			return LinkResult.GeneralFailure;
		}
		var addAuthenticatorResponse = Serializer.Deserialize<CTwoFactor_AddAuthenticator_Response>(response.Stream);

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
			AddedThrough = ADD_THROUGH.EmailCode
		};
		return LinkResult.AwaitingFinalization;
	}
	public FinalizeResult FinalizeAddAuthenticatorEmail(string emailCode)
	{
		if (session == null)
			return FinalizeResult.GeneralFailure;
		using var memStream1 = new MemoryStream();
		var request = new CTwoFactor_FinalizeAddAuthenticator_Request()
		{
			steamid = session.SteamID,
			activation_code = emailCode,
			validate_sms_code = true,
			authenticator_code = LinkedAccount.GenerateSteamGuardCode(),
			authenticator_time = TimeAligner.GetSteamTime()
		};
		Serializer.Serialize(memStream1, request);
		string content = Convert.ToBase64String(memStream1.ToArray());

		var protoRequest = new ProtobufRequest(SteamApiUrls.ITwoFactorService_FinalizeAddAuthenticator_v1, content)
		{
			AccessToken = session.AccessToken,
			Proxy = proxy,
			UserAgent = KnownUserAgents.OkHttp
		};
		using var response = Downloader.PostProtobuf(protoRequest);
		if (!response.Success || response.EResult != EResult.OK)
			return FinalizeResult.GeneralFailure;
		var finalizeResponse = Serializer.Deserialize<CTwoFactor_FinalizeAddAuthenticator_Response>(response.Stream);

		if (finalizeResponse.status == 89)
			return FinalizeResult.BadSMSCode;
		if (finalizeResponse.status == 88)
			return FinalizeResult.UnableToGenerateCorrectCodes;
		if (!finalizeResponse.success)
		{
			return FinalizeResult.GeneralFailure;
		}

		LinkedAccount.FullyEnrolled = true;
		return FinalizeResult.Success;
	}

	public async Task<LinkResult> AddAuthenticatorEmailAsync()
	{
		using var memStream1 = new MemoryStream();
		Serializer.Serialize(memStream1, new CTwoFactor_AddAuthenticator_Request_Email()
		{
			steamid = session.SteamID,
			device_identifier = DeviceID,
		});
		string content = Convert.ToBase64String(memStream1.ToArray());
		var protoRequest = new ProtobufRequest(SteamApiUrls.ITwoFactorService_AddAuthenticator_v1, content)
		{
			AccessToken = session.AccessToken,
			Proxy = proxy,
			UserAgent = KnownUserAgents.OkHttp
		};
		using var response = await Downloader.PostProtobufAsync(protoRequest);
		if (!response.Success || response.EResult != EResult.OK)
		{
			return LinkResult.GeneralFailure;
		}
		var addAuthenticatorResponse = Serializer.Deserialize<CTwoFactor_AddAuthenticator_Response>(response.Stream);

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
			AddedThrough = ADD_THROUGH.EmailCode
		};
		return LinkResult.AwaitingFinalization;
	}
	public async Task<FinalizeResult> FinalizeAddAuthenticatorEmailAsync(string emailCode)
	{
		if (session == null)
			return FinalizeResult.GeneralFailure;
		using var memStream1 = new MemoryStream();
		var request = new CTwoFactor_FinalizeAddAuthenticator_Request()
		{
			steamid = session.SteamID,
			activation_code = emailCode,
			validate_sms_code = true,
			authenticator_code = LinkedAccount.GenerateSteamGuardCode(),
			authenticator_time = TimeAligner.GetSteamTime()
		};
		Serializer.Serialize(memStream1, request);
		string content = Convert.ToBase64String(memStream1.ToArray());

		var protoRequest = new ProtobufRequest(SteamApiUrls.ITwoFactorService_FinalizeAddAuthenticator_v1, content)
		{
			AccessToken = session.AccessToken,
			Proxy = proxy,
			UserAgent = KnownUserAgents.OkHttp
		};
		using var response = await Downloader.PostProtobufAsync(protoRequest);
		if (!response.Success || response.EResult != EResult.OK)
			return FinalizeResult.GeneralFailure;
		var finalizeResponse = Serializer.Deserialize<CTwoFactor_FinalizeAddAuthenticator_Response>(response.Stream);

		if (finalizeResponse.status == 89)
			return FinalizeResult.BadSMSCode;
		if (finalizeResponse.status == 88)
			return FinalizeResult.UnableToGenerateCorrectCodes;
		if (!finalizeResponse.success)
		{
			return FinalizeResult.GeneralFailure;
		}

		LinkedAccount.FullyEnrolled = true;
		return FinalizeResult.Success;
	}


	/// <summary>
	/// Изменяет номер аккаунта, при его привязке
	/// </summary>
	/// <param name="request"></param>
	/// <returns>Null если запрос не прошёл или нет сессии</returns>
	private CPhone_SetAccountPhoneNumber_Response? SetAccountPhoneNumber(CPhone_SetAccountPhoneNumber_Request request)
    {
        if (session == null)
            return null;
        using var memStream1 = new MemoryStream();
        Serializer.Serialize(memStream1, request);
        var protoRequest = new ProtobufRequest(SteamApiUrls.IPhoneService_SetAccountPhoneNumber_v1, Convert.ToBase64String(memStream1.ToArray()))
        {
            AccessToken = session.AccessToken,
            Proxy = proxy,
            UserAgent = KnownUserAgents.OkHttp
		};
        using var response = Downloader.PostProtobuf(protoRequest);
        if (!response.Success || (response.EResult != EResult.OK && response.EResult != EResult.Pending))
            return null;
        var obj = Serializer.Deserialize<CPhone_SetAccountPhoneNumber_Response>(response.Stream);
        return obj;
    }
    private CPhone_IsAccountWaitingForEmailConfirmation_Response? IsAccountWaitingForEmailConfirmation()
    {
        if (session == null)
            return null;
        var protoRequest = new ProtobufRequest(SteamApiUrls.IPhoneService_IsAccountWaitingForEmailConfirmation_v1, string.Empty)
        {
            AccessToken = session.AccessToken,
            Proxy = proxy,
            UserAgent = KnownUserAgents.OkHttp
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
        var protoRequest = new ProtobufRequest(SteamApiUrls.IPhoneService_SendPhoneVerificationCode_v1, Convert.ToBase64String(memStream1.ToArray()))
        {
            AccessToken = session.AccessToken,
            Proxy = proxy,
            UserAgent = KnownUserAgents.OkHttp
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
    /// <summary>
    /// Используется если не нужно привязывать телефон, т.к. один код для одного использования
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private SENDVERIFCODE VerifyAccountPhoneWithCode(CPhone_VerifyAccountPhoneWithCode_Request request)
    {
        if (session == null)
            return SENDVERIFCODE.BadSession;
        using var memStream1 = new MemoryStream();
        Serializer.Serialize(memStream1, request);
        var protoRequest = new ProtobufRequest(SteamApiUrls.IPhoneService_VerifyAccountPhoneWithCode_v1, Convert.ToBase64String(memStream1.ToArray()))
        {
            AccessToken = session.AccessToken,
            Proxy = proxy,
            UserAgent = KnownUserAgents.OkHttp
		};
        using var response = Downloader.PostProtobuf(protoRequest);
        if (!response.Success || response.EResult != EResult.OK)
            return SENDVERIFCODE.Error;
        if (response.EResult == EResult.OK)
            return SENDVERIFCODE.Yes;
        return SENDVERIFCODE.No;
    }

    /// <summary>
    /// Изменяет номер аккаунта, при его привязке
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Null если запрос не прошёл или нет сессии</returns>
    private async Task<CPhone_SetAccountPhoneNumber_Response?> SetAccountPhoneNumberAsync(CPhone_SetAccountPhoneNumber_Request request)
    {
        if (session == null)
            return null;
        using var memStream1 = new MemoryStream();
        Serializer.Serialize(memStream1, request);
        var protoRequest = new ProtobufRequest(SteamApiUrls.IPhoneService_SetAccountPhoneNumber_v1, Convert.ToBase64String(memStream1.ToArray()))
        {
            AccessToken = session.AccessToken,
            Proxy = proxy,
            UserAgent = KnownUserAgents.OkHttp
		};
        using var response = await Downloader.PostProtobufAsync(protoRequest);
        if (!response.Success || (response.EResult != EResult.OK && response.EResult != EResult.Pending))
            return null;
        var obj = Serializer.Deserialize<CPhone_SetAccountPhoneNumber_Response>(response.Stream);
        return obj;
    }
    private async Task<CPhone_IsAccountWaitingForEmailConfirmation_Response?> IsAccountWaitingForEmailConfirmationAsync()
    {
        if (session == null)
            return null;
        var protoRequest = new ProtobufRequest(SteamApiUrls.IPhoneService_SetAccountPhoneNumber_v1, string.Empty)
        {
            AccessToken = session.AccessToken,
            Proxy = proxy,
            UserAgent = KnownUserAgents.OkHttp
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
        var protoRequest = new ProtobufRequest(SteamApiUrls.IPhoneService_SendPhoneVerificationCode_v1, Convert.ToBase64String(memStream1.ToArray()))
        {
            AccessToken = session.AccessToken,
            Proxy = proxy,
            UserAgent = KnownUserAgents.OkHttp
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
    /// <summary>
    /// Используется если не нужно привязывать телефон, т.к. один код для одного использования
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private async Task<SENDVERIFCODE> VerifyAccountPhoneWithCodeAsync(CPhone_VerifyAccountPhoneWithCode_Request request)
    {
        if (session == null)
            return SENDVERIFCODE.BadSession;
        using var memStream1 = new MemoryStream();
        Serializer.Serialize(memStream1, request);
        var protoRequest = new ProtobufRequest(SteamApiUrls.IPhoneService_VerifyAccountPhoneWithCode_v1, Convert.ToBase64String(memStream1.ToArray()))
        {
            AccessToken = session.AccessToken,
            Proxy = proxy,
            UserAgent = KnownUserAgents.OkHttp
		};
        using var response = await Downloader.PostProtobufAsync(protoRequest);
        if (!response.Success || response.EResult != EResult.OK)
            return SENDVERIFCODE.Error;
        if (response.EResult == EResult.OK)
            return SENDVERIFCODE.Yes;
        return SENDVERIFCODE.No;
    }

    public static string GenerateDeviceID() => "android:" + Guid.NewGuid().ToString();
}