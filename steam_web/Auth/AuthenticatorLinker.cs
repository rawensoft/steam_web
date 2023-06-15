using System;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SteamWeb.Extensions;
using SteamWeb.Web;

namespace SteamWeb.Auth;
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

    private readonly SessionData _session;
    //private readonly CookieContainer _cookies;
    private bool _confirmationEmailSent = false;
    private readonly IWebProxy _proxy;

    public AuthenticatorLinker(SessionData session, IWebProxy proxy)
    {
        if (session.Platform != SignInPlatform.Mobile)
            throw new ArgumentException("Вход в сессию должен быть выполнен через мобильную платформу", "session");
        _session = session;
        DeviceID = GenerateDeviceID();
        _proxy = proxy;

        //_cookies = new CookieContainer();
        //session.AddCookieToContainer(ref _cookies);
    }

    public LinkResult AddAuthenticator()
    {
        bool hasPhone = HasPhoneAttached();
        if (hasPhone && PhoneNumber != null)
            return LinkResult.MustRemovePhoneNumber;
        if (!hasPhone && PhoneNumber == null)
            return LinkResult.MustProvidePhoneNumber;

        if (!hasPhone)
        {
            if (_confirmationEmailSent)
            {
                if (!CheckEmailConfirmation())
                    return LinkResult.GeneralFailure;
            }
            else if (!AddPhoneNumber())
                return LinkResult.GeneralFailure;
            else
            {
                _confirmationEmailSent = true;
                return LinkResult.MustConfirmEmail;
            }
        }

        var postRequest = new PostRequest(APIEndpoints.STEAMAPI_BASE + "/ITwoFactorService/AddAuthenticator/v0001", Downloader.AppFormUrlEncoded)
        {
            UserAgent = Downloader.UserAgentOkHttp,
            Session = _session,
            Proxy = _proxy,
            IsMobile = true
        }
        .AddPostData("access_token", _session.OAuthToken).AddPostData("steamid", _session.SteamID).AddPostData("authenticator_type", 1)
        .AddPostData("device_identifier", DeviceID).AddPostData("sms_phone_id", 1);
        var response = Downloader.Post(postRequest);
        if (!response.Success || response.Data.IsEmpty()) return LinkResult.GeneralFailure;

        var options = new JsonSerializerOptions()
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = true,
            IncludeFields = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            PropertyNameCaseInsensitive = false,
        };
        var addAuthenticatorResponse = JsonSerializer.Deserialize<AddAuthenticatorResponse>(response.Data, options);
        if (addAuthenticatorResponse == null || addAuthenticatorResponse.Response == null) return LinkResult.GeneralFailure;
        if (addAuthenticatorResponse.Response.Status == 29) return LinkResult.AuthenticatorPresent;
        if (addAuthenticatorResponse.Response.Status != 1) return LinkResult.GeneralFailure;

        LinkedAccount = addAuthenticatorResponse.Response;
        LinkedAccount.Session = _session;
        LinkedAccount.DeviceID = DeviceID;

        return LinkResult.AwaitingFinalization;
    }
    public FinalizeResult FinalizeAddAuthenticator(string smsCode)
    {
        //The act of checking the SMS code is necessary for Steam to finalize adding the phone number to the account.
        //Of course, we only want to check it if we're adding a phone number in the first place...

        //if (!PhoneNumber.IsEmpty() && !CheckSMSCode(smsCode)) return FinalizeResult.BadSMSCode;

        var postRequest = new PostRequest(APIEndpoints.STEAMAPI_BASE + "/ITwoFactorService/FinalizeAddAuthenticator/v0001", Downloader.AppFormUrlEncoded)
        {
            Content = $"access_token={_session.OAuthToken}&steamid={_session.SteamID}&activation_code={smsCode}",
            Session = _session,
            Proxy = _proxy,
            UserAgent = Downloader.UserAgentOkHttp,
            IsMobile = true
        };
        int tries = 0;
        var options = new JsonSerializerOptions()
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = true,
            IncludeFields = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            PropertyNameCaseInsensitive = false,
        };
        while (tries <= 30)
        {
            postRequest.AddPostData("authenticator_code", LinkedAccount.GenerateSteamGuardCode()).AddPostData("authenticator_time", TimeAligner.GetSteamTime());
            var response = Downloader.Post(postRequest);
            if (!response.Success || response.Data.IsEmpty()) return FinalizeResult.GeneralFailure;
            var finalizeResponse = JsonSerializer.Deserialize<FinalizeAuthenticatorResponse>(response.Data, options);
            if (finalizeResponse == null || finalizeResponse.Response == null) return FinalizeResult.GeneralFailure;
            if (finalizeResponse.Response.Status == 89) return FinalizeResult.BadSMSCode;
            if (finalizeResponse.Response.Status == 88 && tries >= 30)
                return FinalizeResult.UnableToGenerateCorrectCodes;
            if (!finalizeResponse.Response.Success) return FinalizeResult.GeneralFailure;
            if (finalizeResponse.Response.WantMore)
            {
                tries++;
                continue;
            }

            LinkedAccount.FullyEnrolled = true;
            return FinalizeResult.Success;
        }

        return FinalizeResult.GeneralFailure;
    }
    private bool CheckSMSCode(string smsCode)
    {
        var postRequest = new PostRequest(APIEndpoints.COMMUNITY_BASE + "/steamguard/phoneajax", Downloader.AppFormUrlEncoded)
        {
            Proxy = _proxy,
            Session = _session,
            UserAgent = Downloader.UserAgentOkHttp,
            IsMobile = true,
            IsAjax = true,
        }
        .AddPostData("op", "check_sms_code").AddPostData("arg", smsCode).AddPostData("checkfortos", 0).AddPostData("skipvoip", 1).AddPostData("sessionid", _session.SessionID);
        var response = Downloader.PostMobile(postRequest);
        if (!response.Success || response.Data.IsEmpty()) return false;
        var addPhoneNumberResponse = JsonSerializer.Deserialize<AddPhoneResponse>(response.Data);
        if (!addPhoneNumberResponse.Success)
        {
            new ManualResetEvent(false).WaitOne(3500); //It seems that Steam needs a few seconds to finalize the phone number on the account.
            return HasPhoneAttached();
        }
        return true;
    }
    private bool AddPhoneNumber()
    {
        var postRequest = new PostRequest(APIEndpoints.COMMUNITY_BASE + "/steamguard/phoneajax", Downloader.AppFormUrlEncoded)
        {
            Proxy = _proxy,
            Session = _session,
            UserAgent = Downloader.UserAgentOkHttp,
            IsMobile = true,
            IsAjax = true,
        }
        .AddPostData("op", "add_phone_number").AddPostData("arg", PhoneNumber).AddPostData("sessionid", _session.SessionID);
        var response = Downloader.PostMobile(postRequest);
        if (!response.Success || response.Data.IsEmpty()) return false;
        var addPhoneNumberResponse = JsonSerializer.Deserialize<AddPhoneResponse>(response.Data);
        return addPhoneNumberResponse.Success;
    }
    private bool CheckEmailConfirmation()
    {
        var postRequest = new PostRequest(APIEndpoints.COMMUNITY_BASE + "/steamguard/phoneajax", Downloader.AppFormUrlEncoded)
        {
            Proxy = _proxy,
            Session = _session,
            UserAgent = Downloader.UserAgentOkHttp,
            IsMobile = true,
            IsAjax = true,
        }
        .AddPostData("op", "email_confirmation").AddPostData("arg", "").AddPostData("sessionid", _session.SessionID);
        var response = Downloader.PostMobile(postRequest);
        if (!response.Success || response.Data.IsEmpty()) return false;
        var emailConfirmationResponse = JsonSerializer.Deserialize<AddPhoneResponse>(response.Data);
        return emailConfirmationResponse.Success;
    }
    private bool HasPhoneAttached()
    {
        var postRequest = new PostRequest(APIEndpoints.COMMUNITY_BASE + "/steamguard/phoneajax", Downloader.AppFormUrlEncoded)
        {
            Proxy = _proxy,
            Session = _session,
            UserAgent = Downloader.UserAgentOkHttp,
            IsMobile = true,
            IsAjax = true,
        }
        .AddPostData("op", "has_phone").AddPostData("arg", "null").AddPostData("sessionid", _session.SessionID);
        var response = Downloader.PostMobile(postRequest);
        if (!response.Success || response.Data.IsEmpty()) return false;
        var hasPhoneResponse = JsonSerializer.Deserialize<HasPhoneResponse>(response.Data);
        return hasPhoneResponse.HasPhone;
    }

    public async Task<LinkResult> AddAuthenticatorAsync()
    {
        bool hasPhone = await HasPhoneAttachedAsync();
        if (hasPhone && PhoneNumber != null)
            return LinkResult.MustRemovePhoneNumber;
        if (!hasPhone && PhoneNumber == null)
            return LinkResult.MustProvidePhoneNumber;

        if (!hasPhone)
        {
            if (_confirmationEmailSent)
            {
                if (!await CheckEmailConfirmationAsync())
                    return LinkResult.GeneralFailure;
            }
            else if (!await AddPhoneNumberAsync())
                return LinkResult.GeneralFailure;
            else
            {
                _confirmationEmailSent = true;
                return LinkResult.MustConfirmEmail;
            }
        }

        var postRequest = new PostRequest(APIEndpoints.STEAMAPI_BASE + "/ITwoFactorService/AddAuthenticator/v0001", Downloader.AppFormUrlEncoded)
        {
            UserAgent = Downloader.UserAgentOkHttp,
            Session = _session,
            Proxy = _proxy,
            IsMobile = true
        }
        .AddPostData("access_token", _session.OAuthToken).AddPostData("steamid", _session.SteamID).AddPostData("authenticator_type", 1)
        .AddPostData("device_identifier", DeviceID).AddPostData("sms_phone_id", 1);
        var response = await Downloader.PostAsync(postRequest);
        if (!response.Success || response.Data.IsEmpty()) return LinkResult.GeneralFailure;

        var options = new JsonSerializerOptions()
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = true,
            IncludeFields = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            PropertyNameCaseInsensitive = false,
        };
        var addAuthenticatorResponse = JsonSerializer.Deserialize<AddAuthenticatorResponse>(response.Data, options);
        if (addAuthenticatorResponse == null || addAuthenticatorResponse.Response == null) return LinkResult.GeneralFailure;
        if (addAuthenticatorResponse.Response.Status == 29) return LinkResult.AuthenticatorPresent;
        if (addAuthenticatorResponse.Response.Status != 1) return LinkResult.GeneralFailure;

        LinkedAccount = addAuthenticatorResponse.Response;
        LinkedAccount.Session = _session;
        LinkedAccount.DeviceID = DeviceID;

        return LinkResult.AwaitingFinalization;
    }
    public async Task<FinalizeResult> FinalizeAddAuthenticatorAsync(string smsCode)
    {
        //The act of checking the SMS code is necessary for Steam to finalize adding the phone number to the account.
        //Of course, we only want to check it if we're adding a phone number in the first place...

        //if (!PhoneNumber.IsEmpty() && !CheckSMSCode(smsCode)) return FinalizeResult.BadSMSCode;

        var postRequest = new PostRequest(APIEndpoints.STEAMAPI_BASE + "/ITwoFactorService/FinalizeAddAuthenticator/v0001", Downloader.AppFormUrlEncoded)
        {
            Content = $"access_token={_session.OAuthToken}&steamid={_session.SteamID}&activation_code={smsCode}",
            Session = _session,
            Proxy = _proxy,
            UserAgent = Downloader.UserAgentOkHttp,
            IsMobile = true
        };
        int tries = 0;
        var options = new JsonSerializerOptions()
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = true,
            IncludeFields = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            PropertyNameCaseInsensitive = false,
        };
        while (tries <= 30)
        {
            postRequest.AddPostData("authenticator_code", LinkedAccount.GenerateSteamGuardCode()).AddPostData("authenticator_time", TimeAligner.GetSteamTime());
            var response = await Downloader.PostAsync(postRequest);
            if (!response.Success || response.Data.IsEmpty()) return FinalizeResult.GeneralFailure;
            var finalizeResponse = JsonSerializer.Deserialize<FinalizeAuthenticatorResponse>(response.Data, options);
            if (finalizeResponse == null || finalizeResponse.Response == null) return FinalizeResult.GeneralFailure;
            if (finalizeResponse.Response.Status == 89) return FinalizeResult.BadSMSCode;
            if (finalizeResponse.Response.Status == 88 && tries >= 30)
                return FinalizeResult.UnableToGenerateCorrectCodes;
            if (!finalizeResponse.Response.Success) return FinalizeResult.GeneralFailure;
            if (finalizeResponse.Response.WantMore)
            {
                tries++;
                continue;
            }

            LinkedAccount.FullyEnrolled = true;
            return FinalizeResult.Success;
        }

        return FinalizeResult.GeneralFailure;
    }
    private async Task<bool> CheckSMSCodeAsync(string smsCode)
    {
        var postRequest = new PostRequest(APIEndpoints.COMMUNITY_BASE + "/steamguard/phoneajax", Downloader.AppFormUrlEncoded)
        {
            Proxy = _proxy,
            Session = _session,
            UserAgent = Downloader.UserAgentOkHttp,
            IsMobile = true,
            IsAjax = true,
        }
        .AddPostData("op", "check_sms_code").AddPostData("arg", smsCode).AddPostData("checkfortos", 0).AddPostData("skipvoip", 1).AddPostData("sessionid", _session.SessionID);
        var response = await Downloader.PostAsync(postRequest);
        if (!response.Success || response.Data.IsEmpty()) return false;
        var addPhoneNumberResponse = JsonSerializer.Deserialize<AddPhoneResponse>(response.Data);
        if (!addPhoneNumberResponse.Success)
        {
            await Task.Delay(3500);
            return await HasPhoneAttachedAsync();
        }
        return true;
    }
    private async Task<bool> AddPhoneNumberAsync()
    {
        var postRequest = new PostRequest(APIEndpoints.COMMUNITY_BASE + "/steamguard/phoneajax", Downloader.AppFormUrlEncoded)
        {
            Proxy = _proxy,
            Session = _session,
            UserAgent = Downloader.UserAgentOkHttp,
            IsMobile = true,
            IsAjax = true,
        }
        .AddPostData("op", "add_phone_number").AddPostData("arg", PhoneNumber).AddPostData("sessionid", _session.SessionID);
        var response = await Downloader.PostAsync(postRequest);
        if (!response.Success || response.Data.IsEmpty()) return false;
        var addPhoneNumberResponse = JsonSerializer.Deserialize<AddPhoneResponse>(response.Data);
        return addPhoneNumberResponse.Success;
    }
    private async Task<bool> CheckEmailConfirmationAsync()
    {
        var postRequest = new PostRequest(APIEndpoints.COMMUNITY_BASE + "/steamguard/phoneajax", Downloader.AppFormUrlEncoded)
        {
            Proxy = _proxy,
            Session = _session,
            UserAgent = Downloader.UserAgentOkHttp,
            IsMobile = true,
            IsAjax = true,
        }
        .AddPostData("op", "email_confirmation").AddPostData("arg", "").AddPostData("sessionid", _session.SessionID);
        var response = await Downloader.PostAsync(postRequest);
        if (!response.Success || response.Data.IsEmpty()) return false;
        var emailConfirmationResponse = JsonSerializer.Deserialize<AddPhoneResponse>(response.Data);
        return emailConfirmationResponse.Success;
    }
    private async Task<bool> HasPhoneAttachedAsync()
    {
        var postRequest = new PostRequest(APIEndpoints.COMMUNITY_BASE + "/steamguard/phoneajax", Downloader.AppFormUrlEncoded)
        {
            Proxy = _proxy,
            Session = _session,
            UserAgent = Downloader.UserAgentOkHttp,
            IsMobile = true,
            IsAjax = true,
        }
        .AddPostData("op", "has_phone").AddPostData("arg", "null").AddPostData("sessionid", _session.SessionID);
        var response = await Downloader.PostAsync(postRequest);
        if (!response.Success || response.Data.IsEmpty()) return false;
        var hasPhoneResponse = JsonSerializer.Deserialize<HasPhoneResponse>(response.Data);
        return hasPhoneResponse.HasPhone;
    }

    private class AddAuthenticatorResponse
    {
        [JsonPropertyName("response")] public SteamGuardAccount Response { get; init; } = new();
    }
    private class FinalizeAuthenticatorResponse
    {
        [JsonPropertyName("response")] public FinalizeAuthenticatorInternalResponse Response { get; init; }

        internal class FinalizeAuthenticatorInternalResponse
        {
            [JsonPropertyName("status")] public int Status { get; init; }
            [JsonPropertyName("server_time")] public long ServerTime { get; init; }
            [JsonPropertyName("want_more")] public bool WantMore { get; init; }
            [JsonPropertyName("success")] public bool Success { get; init; }
        }
    }
    private class HasPhoneResponse
    {
        [JsonPropertyName("has_phone")] public bool HasPhone { get; init; }
    }
    private class AddPhoneResponse
    {
        [JsonPropertyName("success")] public bool Success { get; init; } = false;
    }

    public static string GenerateDeviceID()
    {
        return "android:" + Guid.NewGuid().ToString();
    }
}
