using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using SteamWeb.Extensions;
using SteamWeb.Web;

namespace SteamWeb.Auth;
public class UserLogin
{
    private const string UrlMobile = "https://steamcommunity.com/login?oauth_client_id=DE45CD61&oauth_scope=read_profile%20write_profile%20read_client%20write_client";
    private const string UrlDesktop = "https://steamcommunity.com/login/home/?goto=";
    private const string CookieMobile = "mobileClientVersion=0 (2.1.3); mobileClient=android; Steam_Language=english; ";
    private const string CookieDesktop = "Steam_Language=english; ";

    public string Username { get; init; }
    public string Password { get; init; }
    public ulong SteamID { get; set; }

    public bool RequiresCaptcha { get; set; } = false;
    public long CaptchaGID { get; set; } = -1;
    public string CaptchaText { get; set; } = null;

    public bool RequiresEmail { get; set; } = false;
    public string EmailDomain { get; set; } = null;
    public string EmailCode { get; set; } = null;

    public bool Requires2FA { get; set; } = false;
    public string TwoFactorCode { get; set; } = null;

    public SessionData Session { get; set; } = null;
    public bool LoggedIn { get; set; } = false;
    public IWebProxy Proxy { get; set; } = null;

    public UserLogin(string username, string password)
    {
        Username = username;
        Password = password;
    }
    public UserLogin(string username, string password, IWebProxy proxy) : this(username, password) => Proxy = proxy;

    public LoginResult DoLogin(SignInPlatform platform)
    {
        string userAgent = platform == SignInPlatform.Desktop ? Downloader.UserAgentChrome : Downloader.UserAgentOkHttp;
        SessionData session = new SessionData() { Platform = platform };
        var getRequest = new GetRequest(platform == SignInPlatform.Mobile ? UrlMobile : UrlDesktop, Proxy, session)
        {
            UserAgent = userAgent,
            Cookie = platform == SignInPlatform.Mobile ? CookieMobile : CookieDesktop,
            IsMobile = platform == SignInPlatform.Mobile
        };
        var response = Downloader.Get(getRequest);

        // Получаем RSA
        var postRequest = new PostRequest("https://steamcommunity.com/login/getrsakey", Downloader.AppFormUrlEncoded)
        {
            Proxy = Proxy,
            Session = session,
            UserAgent = userAgent,
            Cookie = response.Cookie,
            IsMobile = platform == SignInPlatform.Mobile,
            IsAjax = true,
        }
        .AddPostData("donotcache", TimeAligner.GetSteamTime()).AddPostData("username", Username);
        response = Downloader.Post(postRequest);
        if (!response.Success || response.Data.Contains("<BODY>\nAn error occurred while processing your request.")) return LoginResult.GeneralFailure;
        var rsaResponse = JsonSerializer.Deserialize<RSAResponse>(response.Data);
        if (!rsaResponse.Success) return LoginResult.BadRSA;

        new ManualResetEvent(false).WaitOne(350); //Sleep for a bit to give Steam a chance to catch up??

        byte[] encryptedPasswordBytes;
        using var rsaEncryptor = new RSACryptoServiceProvider();
        var passwordBytes = Encoding.ASCII.GetBytes(Password);
        var rsaParameters = rsaEncryptor.ExportParameters(false);
        rsaParameters.Exponent = Util.HexStringToByteArray(rsaResponse.Exponent);
        rsaParameters.Modulus = Util.HexStringToByteArray(rsaResponse.Modulus);
        rsaEncryptor.ImportParameters(rsaParameters);
        encryptedPasswordBytes = rsaEncryptor.Encrypt(passwordBytes, false);
        string encryptedPassword = Convert.ToBase64String(encryptedPasswordBytes);

        postRequest = new PostRequest("https://steamcommunity.com/login/dologin", Downloader.AppFormUrlEncoded)
        {
            Proxy = Proxy,
            Session = session,
            UserAgent = userAgent,
            Cookie = response.Cookie,
            IsMobile = platform == SignInPlatform.Mobile,
            IsAjax = true,
        }
        .AddPostData("donotcache", TimeAligner.GetSteamTime()).AddPostData("password", encryptedPassword).AddPostData("username", Username)
        .AddPostData("remember_login", "true").AddPostData("twofactorcode", TwoFactorCode ?? "").AddPostData("emailauth", EmailCode ?? "").AddPostData("loginfriendlyname", "")
        .AddPostData("rsatimestamp", rsaResponse.Timestamp).AddPostData("tokentype", -1);
        if (RequiresCaptcha) postRequest.AddPostData("captchagid", CaptchaGID).AddPostData("captcha_text", CaptchaText);
        else postRequest.AddPostData("captchagid", -1).AddPostData("captcha_text", "");

        if (Requires2FA || RequiresEmail) postRequest.AddPostData("emailsteamid", SteamID);
        else postRequest.AddPostData("emailsteamid", "");
        if (platform == SignInPlatform.Mobile)
            postRequest.AddPostData("oauth_client_id", "DE45CD61").AddPostData("oauth_scope", "read_profile write_profile read_client write_client");
        response = Downloader.Post(postRequest);
        if (!response.Success) return LoginResult.GeneralFailure;
        dynamic loginResponse = platform == SignInPlatform.Mobile ? JsonSerializer.Deserialize<LoginResponse<OAuthMobile>>(response.Data) :
            JsonSerializer.Deserialize<LoginResponse<OAuthDesktop>>(response.Data);

        if (loginResponse.Message != null)
        {
            if (loginResponse.Message.Contains("There have been too many login failures"))
                return LoginResult.TooManyFailedLogins;

            if (loginResponse.Message == "The account name or password that you have entered is incorrect.")
                return LoginResult.BadCredentials;
        }
        if (loginResponse.ClearPasswordField)
            return LoginResult.BadCredentials;
        if (loginResponse.CaptchaNeeded)
        {
            RequiresCaptcha = true;
            CaptchaGID = loginResponse.CaptchaGID;
            return LoginResult.NeedCaptcha;
        }
        if (loginResponse.EmailAuthNeeded)
        {
            RequiresEmail = true;
            SteamID = ulong.Parse(loginResponse.EmailSteamID);
            return LoginResult.NeedEmail;
        }
        if (loginResponse.TwoFactorNeeded && !loginResponse.Success)
        {
            Requires2FA = true;
            return LoginResult.Need2FA;
        }
        if (loginResponse.OAuthData == null ||
            loginResponse.OAuthData.OAuthToken == null ||
            loginResponse.OAuthData.OAuthToken.Length == 0)
        {
            return LoginResult.GeneralFailure;
        }
        if (!loginResponse.LoginComplete)
            return LoginResult.BadCredentials;
        else
        {
            var oAuthData = loginResponse.OAuthData;
            foreach (var item in response.Cookie.Split("; "))
            {
                var splitted = item.Split('=');
                var name = splitted[0];
                if (name.Length == 0) continue;
                else if (name == "steamLoginSecure") session.SteamLoginSecure = splitted[1];
                else if (name.StartsWith("steamMachineAuth")) session.SteamMachineAuth = splitted[1];
                else if (name == "steamRememberLogin") session.SteamRememberLogin = splitted[1];
                else if (name == "sessionid") session.SessionID = splitted[1];
                else if (name == "browserid") session.BrowserID = splitted[1];
                else if (name == "steamCountry") session.SteamCountry = splitted[1];
            }
            session.OAuthToken = oAuthData.OAuthToken;
            session.SteamID = oAuthData.SteamID64;
            session.WebCookie = oAuthData.WebCookie;
            if (session.SteamLoginSecure.IsEmpty())
                session.SteamLoginSecure = $"{session.SteamID}%7C%7C{oAuthData.SteamLoginSecure}";
            Session = session;
            LoggedIn = true;
            return LoginResult.LoginOkay;
        }
    }
    public async Task<LoginResult> DoLoginAsync(SignInPlatform platform)
    {
        string userAgent = platform == SignInPlatform.Desktop ? Downloader.UserAgentChrome : Downloader.UserAgentOkHttp;
        SessionData session = new SessionData() { Platform = platform };
        var getRequest = new GetRequest(platform == SignInPlatform.Mobile ? UrlMobile : UrlDesktop, Proxy, session)
        {
            UserAgent = userAgent,
            Cookie = platform == SignInPlatform.Mobile ? CookieMobile : CookieDesktop,
            IsMobile = platform == SignInPlatform.Mobile
        };
        var response = await Downloader.GetAsync(getRequest);

        // Получаем RSA
        var postRequest = new PostRequest("https://steamcommunity.com/login/getrsakey", Downloader.AppFormUrlEncoded)
        {
            Proxy = Proxy,
            Session = session,
            UserAgent = userAgent,
            Cookie = response.Cookie,
            IsMobile = platform == SignInPlatform.Mobile,
            IsAjax = true,
        }
        .AddPostData("donotcache", TimeAligner.GetSteamTime()).AddPostData("username", Username);
        response = await Downloader.PostAsync(postRequest);
        if (!response.Success || response.Data.Contains("<BODY>\nAn error occurred while processing your request.")) return LoginResult.GeneralFailure;
        var rsaResponse = JsonSerializer.Deserialize<RSAResponse>(response.Data);
        if (!rsaResponse.Success) return LoginResult.BadRSA;

        await Task.Delay(350); //Sleep for a bit to give Steam a chance to catch up??

        byte[] encryptedPasswordBytes;
        using var rsaEncryptor = new RSACryptoServiceProvider();
        var passwordBytes = Encoding.ASCII.GetBytes(Password);
        var rsaParameters = rsaEncryptor.ExportParameters(false);
        rsaParameters.Exponent = Util.HexStringToByteArray(rsaResponse.Exponent);
        rsaParameters.Modulus = Util.HexStringToByteArray(rsaResponse.Modulus);
        rsaEncryptor.ImportParameters(rsaParameters);
        encryptedPasswordBytes = rsaEncryptor.Encrypt(passwordBytes, false);
        string encryptedPassword = Convert.ToBase64String(encryptedPasswordBytes);

        postRequest = new PostRequest("https://steamcommunity.com/login/dologin", Downloader.AppFormUrlEncoded)
        {
            Proxy = Proxy,
            Session = session,
            UserAgent = userAgent,
            Cookie = response.Cookie,
            IsMobile = platform == SignInPlatform.Mobile,
            IsAjax = true,
        }
        .AddPostData("donotcache", TimeAligner.GetSteamTime()).AddPostData("password", encryptedPassword).AddPostData("username", Username)
        .AddPostData("remember_login", "true").AddPostData("twofactorcode", TwoFactorCode ?? "").AddPostData("emailauth", EmailCode ?? "").AddPostData("loginfriendlyname", "")
        .AddPostData("rsatimestamp", rsaResponse.Timestamp).AddPostData("tokentype", -1);
        if (RequiresCaptcha) postRequest.AddPostData("captchagid", CaptchaGID).AddPostData("captcha_text", CaptchaText);
        else postRequest.AddPostData("captchagid", -1).AddPostData("captcha_text", "");

        if (Requires2FA || RequiresEmail) postRequest.AddPostData("emailsteamid", SteamID);
        else postRequest.AddPostData("emailsteamid", "");
        if (platform == SignInPlatform.Mobile)
            postRequest.AddPostData("oauth_client_id", "DE45CD61").AddPostData("oauth_scope", "read_profile write_profile read_client write_client");
        response = await Downloader.PostAsync(postRequest);
        if (!response.Success) return LoginResult.GeneralFailure;
        dynamic loginResponse = platform == SignInPlatform.Mobile ? JsonSerializer.Deserialize<LoginResponse<OAuthMobile>>(response.Data) :
            JsonSerializer.Deserialize<LoginResponse<OAuthDesktop>>(response.Data);

        if (loginResponse.Message != null)
        {
            if (loginResponse.Message.Contains("There have been too many login failures"))
                return LoginResult.TooManyFailedLogins;

            if (loginResponse.Message == "The account name or password that you have entered is incorrect.")
                return LoginResult.BadCredentials;
        }
        if (loginResponse.ClearPasswordField)
            return LoginResult.BadCredentials;
        if (loginResponse.CaptchaNeeded)
        {
            RequiresCaptcha = true;
            CaptchaGID = loginResponse.CaptchaGID;
            return LoginResult.NeedCaptcha;
        }
        if (loginResponse.EmailAuthNeeded)
        {
            RequiresEmail = true;
            SteamID = ulong.Parse(loginResponse.EmailSteamID);
            return LoginResult.NeedEmail;
        }
        if (loginResponse.TwoFactorNeeded && !loginResponse.Success)
        {
            Requires2FA = true;
            return LoginResult.Need2FA;
        }
        if (loginResponse.OAuthData == null ||
            loginResponse.OAuthData.OAuthToken == null ||
            loginResponse.OAuthData.OAuthToken.Length == 0)
        {
            return LoginResult.GeneralFailure;
        }
        if (!loginResponse.LoginComplete)
            return LoginResult.BadCredentials;
        else
        {
            var oAuthData = loginResponse.OAuthData;
            foreach (var item in response.Cookie.Split("; "))
            {
                var splitted = item.Split('=');
                var name = splitted[0];
                if (name.Length == 0) continue;
                else if (name == "steamLoginSecure") session.SteamLoginSecure = splitted[1];
                else if (name.StartsWith("steamMachineAuth")) session.SteamMachineAuth = splitted[1];
                else if (name == "steamRememberLogin") session.SteamRememberLogin = splitted[1];
                else if (name == "sessionid") session.SessionID = splitted[1];
                else if (name == "browserid") session.BrowserID = splitted[1];
                else if (name == "steamCountry") session.SteamCountry = splitted[1];
            }
            session.OAuthToken = oAuthData.OAuthToken;
            session.SteamID = oAuthData.SteamID64;
            session.WebCookie = oAuthData.WebCookie;
            if (session.SteamLoginSecure.IsEmpty())
                session.SteamLoginSecure = $"{session.SteamID}%7C%7C{oAuthData.SteamLoginSecure}";
            Session = session;
            LoggedIn = true;
            return LoginResult.LoginOkay;
        }
    }
}
internal class RSAResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; init; } = false;

    [JsonPropertyName("publickey_exp")]
    public string Exponent { get; init; }

    [JsonPropertyName("publickey_mod")]
    public string Modulus { get; init; }

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; init; }

    [JsonPropertyName("token_gid")]
    public string token_gid { get; init; }
}
internal class LoginResponse<T> where T : IOAuth
{
    [JsonPropertyName("success")]
    public bool Success { get; init; } = false;

    [JsonPropertyName("login_complete")]
    public bool LoginComplete { get; init; } = false;

    [JsonPropertyName("transfer_parameters")]
    public T OAuthData { get; init; } = default;

    [JsonPropertyName("oauth")]
    public string SetOauth { init => OAuthData = JsonSerializer.Deserialize<T>(value); }

    [JsonPropertyName("captcha_needed")]
    public bool CaptchaNeeded { get; init; } = false;//проверить

    [JsonPropertyName("captcha_gid")]
    public long CaptchaGID { get; init; } //проверить

    [JsonPropertyName("emaildomain")]
    public string EmailDomain { get; init; }

    [JsonPropertyName("emailsteamid")]
    public string EmailSteamID { get; init; }

    [JsonPropertyName("emailauth_needed")]
    public bool EmailAuthNeeded { get; init; } = false;

    [JsonPropertyName("requires_twofactor")]
    public bool TwoFactorNeeded { get; init; } = false;

    [JsonPropertyName("message")]
    public string Message { get; init; }

    [JsonPropertyName("clear_password_field")]
    public bool ClearPasswordField { get; init; } = false;
}

public interface IOAuth
{
    public string SteamID { get; init; }
    public string OAuthToken { get; init; }
    public string SteamLoginSecure { get; init; }
    public string WebCookie { get; init; }
}
internal class OAuthDesktop : IOAuth
{
    [JsonPropertyName("steamid")] public string SteamID { get; init; } = "0";
    [JsonIgnore] public ulong SteamID64 => SteamID.ParseUInt64();
    [JsonPropertyName("auth")] public string OAuthToken { get; init; }
    [JsonPropertyName("token_secure")] public string SteamLoginSecure { get; init; }
    [JsonPropertyName("webcookie")] public string WebCookie { get; init; }
    [JsonPropertyName("remember_login")] public bool RememberLogin { get; init; } = false;
}
internal class OAuthMobile : IOAuth
{
    [JsonPropertyName("account_name")] public string AccountName { get; init; }
    [JsonPropertyName("steamid")] public string SteamID { get; init; } = "0";
    [JsonIgnore] public ulong SteamID64 => SteamID.ParseUInt64();
    [JsonPropertyName("oauth_token")] public string OAuthToken { get; init; }
    [JsonPropertyName("wgtoken")] public string SteamLogin { get; init; }
    [JsonPropertyName("wgtoken_secure")] public string SteamLoginSecure { get; init; }
    [JsonPropertyName("webcookie")] public string WebCookie { get; init; }
}
