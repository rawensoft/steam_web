using SteamWeb.Script.Models;
using SteamWeb.Web;
using System.Text.Json;
using SteamWeb.Script.Enums;
using SteamWeb.Extensions;
using System.Web;
using SteamWeb.Auth.Interfaces;

namespace SteamWeb.Script;
public static class AjaxHelp
{
	/// <summary>
	/// Проверка номера для рега его на акк
	/// </summary>
	/// <param name="number">Номер как +7 9991112233</param>
	/// <returns></returns>
	public static async Task<AjaxValidPhone> AjaxValidNumberAsync(ISessionProvider session, System.Net.IWebProxy proxy, string number)
    {
        var request = new PostRequest(SteamPoweredUrls.Phone_Validate, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            Referer = SteamPoweredUrls.Phone_Add,
            IsAjax = true
        };
        request.AddPostData("sessionID", session.SessionID).AddPostData("phoneNumber", number);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxValidPhone>(response.Data);
            return obj;
        }
        catch (Exception ex)
        { }
        return new();
    }

    /// <summary>
    /// Нужно для привязывания номера к аккаунту
    /// </summary>
    /// <param name="input">только вписать при отправке кода из смс</param>
    /// <param name="op">get_phone_number - отправка кода на почту, email_verification - отправка смс на телефон, get_sms_code - отправка кода из смс</param>
    /// <returns></returns>
    public static async Task<AjaxOp> AjaxAddjaxopAsync(ISessionProvider session, System.Net.IWebProxy proxy, string input, OP_CODES op, bool confirmed = true)
    {
        var request = new PostRequest(SteamPoweredUrls.Phone_AddAjaxOp, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            Referer = SteamPoweredUrls.Phone_Add,
            IsAjax = true
        };
        input = input == null ? string.Empty : input.Replace("+", "%2B").Replace(" ", "+");
        request.AddPostData("op", GetOpCode(op)).AddPostData("input", input).AddPostData("sessionID", session.SessionID).AddPostData("confirmed", confirmed ? 1 : 0)
            .AddPostData("checkfortos", 1).AddPostData("bisediting", 0).AddPostData("token", 0);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxOp>(response.Data.Replace("\"state\":false", "\"state\":\"false\""));
            return obj;
        }
        catch (Exception ex)
        { }
        return new();
    }

	/// <summary>
	/// Отправка запроса на смену данных в мобильное приложение\код на почту
	/// </summary>
	/// <param name="s">s параметр полученный от steam</param>
	/// <param name="referer">Url открытия страницы смены данных</param>
	/// <param name="method">Какой запрос, на подтверждение владения аккаунтом, отправлять</param>
	/// <returns></returns>
	public static async Task<AjaxDefault> AjaxSendAccountRecoveryCodeAsync(ISessionProvider session, System.Net.IWebProxy proxy, string s, string referer, TypeMethod method)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxSendAccountRecoveryCode, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            Referer = referer,
            IsAjax = true
        };
        request.AddPostData("sessionid", session.SessionID).AddPostData("wizard_ajax", 1).AddPostData("s", s).AddPostData("method", GetMethod(method));
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxDefault>(response.Data);
            return obj;
        }
        catch (Exception ex)
        { }
        return new();
    }

	/// <summary>
	/// Проверка подтверждения на смену данных. Для продолжения должно быть:
	/// <code>
	/// if (AjaxPollRecoveryConf.success && !AjaxPollRecoveryConf.continue) return;
	/// </code>
	/// </summary>
	/// <param name="s">s параметр полученный от steam</param>
	/// <param name="method">Какой запрос, на подтверждение владения аккаунтом, отправлять</param>
	/// <param name="reset">Какие данные сбрасываем\изменяем</param>
	/// <param name="referer">url открытия страницы смены данных</param>
	/// <returns></returns>
	public static async Task<AjaxPollRecoveryConf> AjaxPollAccountRecoveryConfirmationAsync(ISessionProvider session, System.Net.IWebProxy proxy, string s, TypeReset reset, TypeMethod method, string referer)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxPollAccountRecoveryConfirmation, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            Referer = referer,
            IsAjax = true
        };
        request.AddPostData("sessionid", session.SessionID).AddPostData("wizard_ajax", 1).AddPostData("s", s).AddPostData("reset", GetReset(reset))
            .AddPostData("lost", GetLost(TypeLost.RecoveryConfirm)).AddPostData("method", GetMethod(method)).AddPostData("issueid", GetIssueID(reset));
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxPollRecoveryConf>(response.Data);
            return obj;
        }
        catch (Exception ex)
        { }
        return new();
    }

	/// <summary>
	/// Проверка подтверждения\кода с почты
	/// </summary>
	/// <param name="code">Для мобильного подтверждения оставить пустую строку</param>
	/// <param name="s">s параметр полученный от steam</param>
	/// <param name="method">Какой запрос, на подтверждение владения аккаунтом, был выбран</param>
	/// <param name="reset">Какие данные сбрасываем\изменяем</param>
	/// <returns></returns>
	public static async Task<AjaxNext> AjaxVerifyAccountRecoveryCodeAsync(ISessionProvider session, System.Net.IWebProxy proxy, string code, string s, TypeMethod method, TypeReset reset, string referer)
    {
        var request = new GetRequest(SteamPoweredUrls.Wizard_AjaxVerifyAccountRecoveryCode, proxy, session, referer)
        {
            IsAjax = true
        };
        request.AddQuery("code", code).AddQuery("s", s).AddQuery("reset", GetReset(reset)).AddQuery("lost", GetLost(TypeLost.RecoveryConfirm))
            .AddQuery("method", GetMethod(method)).AddQuery("issueid", GetIssueID(reset)).AddQuery("sessionid", session.SessionID).AddQuery("wizard_ajax", 1);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxNext>(response.Data);
            return obj;
        }
        catch (Exception ex)
        { }
        return new();
    }

	/// <summary>
	/// Отправить код на почту, на которую нужно сменить
	/// </summary>
	/// <param name="email">Новая почта</param>
	/// <param name="account">steamid32</param>
	/// <param name="s">s параметр полученный от steam</param>
	/// <param name="referer">url страницы смены данных</param>
	/// <returns></returns>
	public static async Task<AjaxEmailConfirm> AjaxAccountRecoveryChangeEmailAsync(ISessionProvider session, System.Net.IWebProxy proxy, string email, string account, string s, string referer)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxAccountRecoveryChangeEmail, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            Referer = referer,
            IsAjax = true
        };
        request.AddPostData("sessionid", session.SessionID, false).AddPostData("wizard_ajax", 1).AddPostData("s", s, false)
            .AddPostData("account", account, false).AddPostData("email", HttpUtility.UrlEncode(email), false);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxEmailConfirm>(response.Data);
            return obj;
        }
        catch (Exception ex)
        { }
        return new();
    }

    /// <summary>
    /// Подтверждение смены почты
    /// </summary>
    /// <param name="email">Новая почта</param>
    /// <param name="email_code">Код смены с новой почты</param>
    /// <param name="account">Account ID</param>
    /// <returns></returns>
    public static async Task<AjaxNext> AjaxAccountRecoveryConfirmChangeEmailAsync(ISessionProvider session, System.Net.IWebProxy proxy, string email, string email_change_code, string account, string s, string referer)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxAccountRecoveryConfirmChangeEmail, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            Referer = referer,
            IsAjax = true
        };
        request.AddPostData("sessionid", session.SessionID).AddPostData("wizard_ajax", 1).AddPostData("s", s).AddPostData("account", account)
            .AddPostData("email", email).AddPostData("email_change_code", email_change_code);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxNext>(response.Data);
            return obj;
        }
        catch (Exception ex)
        { }
        return new();
    }

    /// <summary>
    /// Проверка пароля
    /// </summary>
    /// <param name="password">пароль</param>
    /// <returns></returns>
    public static async Task<AjaxPasswordAvailable> AjaxCheckPasswordAvailableAsync(ISessionProvider session, System.Net.IWebProxy proxy, string password, string referer)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxCheckPasswordAvailable, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            Referer = referer,
            IsAjax = true
        };
        request.AddPostData("sessionid", session.SessionID).AddPostData("wizard_ajax", 1).AddPostData("password", password);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxPasswordAvailable>(response.Data);
            return obj;
        }
        catch (Exception ex)
        { }
        return new();
    }

    public static async Task<AjaxNext> AjaxAccountRecoveryResetPhoneNumberAsync(ISessionProvider session, System.Net.IWebProxy proxy, string s, string account, string referer)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxAccountRecoveryResetPhoneNumber, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            Referer = referer,
            IsAjax = true
        };
        request.AddPostData("sessionid", session.SessionID).AddPostData("account", account);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxNext>(response.Data);
            return obj;
        }
        catch (Exception ex)
        { }
        return new();
    }

    public static async Task<AjaxNextStep> AjaxAccountRecoveryGetNextStepAsync(ISessionProvider session, System.Net.IWebProxy proxy, string account, string s, string referer, TypeReset reset, TypeLost lost)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxAccountRecoveryGetNextStep, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            Referer = referer,
            IsAjax = true
        };
        request.AddPostData("sessionid", session.SessionID).AddPostData("wizard_ajax", 1).AddPostData("s", s).AddPostData("account", account)
            .AddPostData("reset", GetReset(reset)).AddPostData("issueid", GetIssueID(reset)).AddPostData("lost", GetLost(lost));
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxNextStep>(response.Data);
            return obj;
        }
        catch (Exception ex)
        { }
        return new();
    }
    
    /// <summary>
    /// Получает RSA от steam
    /// </summary>
    /// <param name="login">Логин аккаунта</param>
    /// <param name="referer">Текущий url страницы</param>
    /// <returns>Класс содержащий данные RSA</returns>
    public static async Task<SteamRSA> GetRSAKeyAsync(ISessionProvider session, System.Net.IWebProxy proxy, string login, string referer)
    {
        var request = new PostRequest(SteamCommunityUrls.Login_GetRSAKey, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            Referer = referer,
            IsAjax = true
        };
        request.AddPostData("donotcache", DateTime.UtcNow.ToTimeStamp()).AddPostData("username", login.ToLower());
        if (session != null)
        {
            request.AddPostData("sessionid", session.SessionID);
            request.Url = SteamPoweredUrls.Login_GetRSAKey;
        }
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<SteamRSA>(response.Data);
            return obj;
        }
        catch (Exception ex)
        { }
        return new();
    }

    public static async Task<AjaxNext> AjaxAccountRecoveryChangePasswordAsync(ISessionProvider session, System.Net.IWebProxy proxy, string account, string s, string password, SteamRSA rsa, string referer)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxAccountRecoveryChangePassword, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            Referer = referer,
            IsAjax = true
        };
        string encrypt = Helpers.Encrypt(password, rsa.publickey_mod, rsa.publickey_exp);
        request.AddPostData("sessionid", session.SessionID).AddPostData("wizard_ajax", 1).AddPostData("s", s).AddPostData("account", account)
            .AddPostData("password", encrypt).AddPostData("rsatimestamp", rsa.timestamp);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxNext>(response.Data);
            return obj;
        }
        catch (Exception ex)
        { }
        return new();
    }
    
    public static async Task<AjaxLicense> AjaxRegisterKey(ISessionProvider session, System.Net.IWebProxy proxy, string product_key)
    {
        var request = new PostRequest(SteamPoweredUrls.Account_AjaxRegisterKey, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            Referer = SteamPoweredUrls.Account_RegisterKey,
            IsAjax = true
        };
        request.AddPostData("product_key", product_key).AddPostData("sessionid", session.SessionID);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxLicense>(response.Data);
            return obj;
        }
        catch (Exception ex)
        { }
        return new();
    }

    public static async Task<string> DoLogin(System.Net.IWebProxy proxy, string username, string password, SteamRSA rsa, string fa2 = "", string email = "", string pc_name = "", string captcha_text = "", ulong captcha_gid = 0)
    {
        var request = new PostRequest(SteamCommunityUrls.Login_DoLogin, Downloader.AppFormUrlEncoded)
        {
            Proxy = proxy,
            Referer = "https://steamcommunity.com/mobilelogin?oauth_client_id=DE45CD61&oauth_scope=read_profile%20write_profile%20read_client%20write_client",
            IsAjax = true
        };
        request.AddPostData("donotcache", DateTime.UtcNow.ToTimeStamp()).AddPostData("password", Helpers.Encrypt(password, rsa.publickey_mod, rsa.publickey_exp))
            .AddPostData("username", username).AddPostData("twofactorcode", fa2).AddPostData("captcha_text", captcha_text).AddPostData("emailsteamid", "")
            .AddPostData("rsatimestamp", rsa.timestamp).AddPostData("emailauth", email).AddPostData("loginfriendlyname", pc_name).AddPostData("remember_login", "true");
        request.AddPostData("captchagid", captcha_gid > 0 ? captcha_gid.ToString() : "-1");
        var response = await Downloader.PostAsync(request);
        return response.Data;
    }
    
    public static async Task<PhoneAjax> PhoneAjaxAsync(ISessionProvider session, System.Net.IWebProxy proxy)
    {
        var request = new PostRequest(SteamCommunityUrls.SteamGuard_PhoneAjax, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            IsAjax = true
        };
        request.AddPostData("op", "has_phone").AddPostData("arg", "null").AddPostData("sessionid", session.SessionID);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<PhoneAjax>(response.Data);
            return obj;
        }
        catch (Exception ex)
        { }
        return new();
    }
    public static PhoneAjax PhoneAjax(ISessionProvider session, System.Net.IWebProxy proxy)
    {
        var request = new PostRequest(SteamCommunityUrls.SteamGuard_PhoneAjax, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            IsAjax = true
        };
        request.AddPostData("op", "has_phone").AddPostData("arg", "null").AddPostData("sessionid", session.SessionID);
        var response = Downloader.Post(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<PhoneAjax>(response.Data);
            return obj;
        }
        catch (Exception ex)
        { }
        return new();
    }

	/// <summary>
	/// Отправляет пароль на проверку. Используется если нужно подтвердить владение аккаунтов, обычно после мобильного подтверждения, если отказ от почтового.
	/// </summary>
	/// <param name="rsa"></param>
	/// <param name="s"></param>
	/// <param name="reset"></param>
	/// <param name="lost"></param>
	/// <param name="password">Оригинальный пароль (не зашифрованный)</param>
	/// <returns></returns>
	public static async Task<AjaxNext> AjaxAccountRecoveryVerifyPassword(ISessionProvider session, System.Net.IWebProxy proxy, SteamRSA rsa, string s, TypeReset reset, TypeLost lost, string password)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxAccountRecoveryVerifyPassword, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            IsAjax = true
        };
        request.AddPostData("sessionid", session.SessionID).AddPostData("s", s).AddPostData("lost", GetLost(lost)).AddPostData("reset", GetReset(reset))
            .AddPostData("password", Helpers.Encrypt(password, rsa.publickey_mod, rsa.publickey_exp)).AddPostData("rsatimestamp", rsa.timestamp);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxNext>(response.Data);
            return obj;
        }
        catch (Exception ex)
        { }
        return new();
    }

    public static string GetOpCode(OP_CODES op) => op switch
    {
        OP_CODES.GetSMSCode => "get_sms_code",
        OP_CODES.GetPhoneNumber => "get_phone_number",
        OP_CODES.RetryEmailVerification => "retry_email_verification",
        OP_CODES.ReSendSMS => "resend_sms",
        _ => "email_verification",
    };
    public static int GetMethod(TypeMethod method) => method switch
    {
        TypeMethod.Mobile => 8,
        TypeMethod.Email => 2,
        _ => -1
    };
    public static int GetReset(TypeReset reset) => reset switch
    {
        TypeReset.Email => 2,
        TypeReset.Password => 1,
        TypeReset.Phone => 4,
        TypeReset.KTEmail => 0,
        TypeReset.KTGuard => 0,
        TypeReset.KTPhone => 0,
        TypeReset.KTPassword => 0,
        _ => -1
    };
    public static int GetIssueID(TypeReset reset) => reset switch
    {
        TypeReset.Email => 409,
        TypeReset.Password => 406,
        TypeReset.Phone => 403,
        TypeReset.KTEmail => 0,
        TypeReset.KTGuard => 0,
        TypeReset.KTPhone => 0,
        TypeReset.KTPassword => 0,
        _ => -1
    };
    public static byte GetLost(TypeLost lost) => (byte)lost;
}
