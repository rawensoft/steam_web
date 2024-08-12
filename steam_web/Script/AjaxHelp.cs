using SteamWeb.Script.Models;
using SteamWeb.Web;
using System.Text.Json;
using SteamWeb.Script.Enums;
using SteamWeb.Extensions;
using System.Web;

namespace SteamWeb.Script;
public static class AjaxHelp
{
	/// <summary>
	/// Проверка номера для рега его на акк
	/// </summary>
	/// <param name="number">Номер как +7 9991112233</param>
	/// <returns></returns>
	public static async Task<AjaxValidPhone> AjaxValidNumberAsync(AjaxDefaultRequest ajaxRequest, string number)
    {
        var request = new PostRequest(SteamPoweredUrls.Phone_Validate, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = SteamPoweredUrls.Phone_Add,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddPostData("sessionID", ajaxRequest.Session.SessionID).AddPostData("phoneNumber", number);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxValidPhone>(response.Data!)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    /// <summary>
    /// Нужно для привязывания номера к аккаунту
    /// </summary>
    /// <param name="input">только вписать при отправке кода из смс</param>
    /// <param name="op">get_phone_number - отправка кода на почту, email_verification - отправка смс на телефон, get_sms_code - отправка кода из смс</param>
    /// <returns></returns>
    public static async Task<AjaxOp> AjaxAddjaxopAsync(AjaxDefaultRequest ajaxRequest, string input, OP_CODES op, bool confirmed = true)
    {
        var request = new PostRequest(SteamPoweredUrls.Phone_AddAjaxOp, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = SteamPoweredUrls.Phone_Add,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        input = string.IsNullOrEmpty(input) ? string.Empty : input.Replace("+", "%2B").Replace(" ", "+");
        request.AddPostData("op", op.ToStringValue()).AddPostData("input", input).AddPostData("sessionID", ajaxRequest.Session.SessionID)
            .AddPostData("confirmed", confirmed ? 1 : 0).AddPostData("checkfortos", 1).AddPostData("bisediting", 0).AddPostData("token", 0);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxOp>(response.Data!.Replace("\"state\":false", "\"state\":\"false\""))!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

	/// <summary>
	/// Отправка запроса на смену данных в мобильное приложение\код на почту
	/// </summary>
	/// <param name="method">Какой запрос, на подтверждение владения аккаунтом, отправлять</param>
	/// <returns></returns>
	public static async Task<AjaxDefault> AjaxSendAccountRecoveryCodeAsync(AjaxInfoRequest ajaxRequest)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxSendAccountRecoveryCode, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = ajaxRequest.Referer,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddPostData("sessionid", ajaxRequest.Session.SessionID).AddPostData("wizard_ajax", 1)
            .AddPostData("s", ajaxRequest.S).AddPostData("method", ajaxRequest.Method.ToDigitMethod());
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxDefault>(response.Data!)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

	/// <summary>
	/// Проверка подтверждения на смену данных. Для продолжения должно быть:
	/// <code>
	/// if (AjaxPollRecoveryConf.success && !AjaxPollRecoveryConf.continue) return;
	/// </code>
	/// </summary>
	/// <returns></returns>
	public static async Task<AjaxPollRecoveryConf> AjaxPollAccountRecoveryConfirmationAsync(AjaxInfoRequest ajaxRequest)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxPollAccountRecoveryConfirmation, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = ajaxRequest.Referer,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddPostData("sessionid", ajaxRequest.Session.SessionID).AddPostData("wizard_ajax", 1).AddPostData("s", ajaxRequest.S)
            .AddPostData("reset", ajaxRequest.Reset.ToDigitReset()).AddPostData("lost", TypeLost.RecoveryConfirm.ToDigitLost())
            .AddPostData("method", ajaxRequest.Method.ToDigitMethod()).AddPostData("issueid", ajaxRequest.Reset.ToDigitIssueId());
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxPollRecoveryConf>(response.Data!)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

	/// <summary>
	/// Проверка подтверждения\кода с почты
	/// </summary>
	/// <param name="code">Для мобильного подтверждения оставить пустую строку</param>
	/// <returns></returns>
	public static async Task<AjaxNext> AjaxVerifyAccountRecoveryCodeAsync(AjaxInfoRequest ajaxRequest, string code)
    {
        var request = new GetRequest(SteamPoweredUrls.Wizard_AjaxVerifyAccountRecoveryCode)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            IsAjax = true,
            Referer = ajaxRequest.Referer,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddQuery("code", code).AddQuery("s", ajaxRequest.S).AddQuery("reset", ajaxRequest.Reset.ToDigitReset())
            .AddQuery("lost", TypeLost.RecoveryConfirm.ToDigitLost()).AddQuery("method", ajaxRequest.Method.ToDigitMethod())
            .AddQuery("issueid", ajaxRequest.Reset.ToDigitIssueId()).AddQuery("sessionid", ajaxRequest.Session.SessionID)
            .AddQuery("wizard_ajax", 1);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxNext>(response.Data!)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

	/// <summary>
	/// Отправить код на почту, на которую нужно сменить
	/// </summary>
	/// <param name="email">Новая почта</param>
	/// <returns></returns>
	public static async Task<AjaxEmailConfirm> AjaxAccountRecoveryChangeEmailAsync(AjaxInfoRequest ajaxRequest, string email)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxAccountRecoveryChangeEmail, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = ajaxRequest.Referer,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddPostData("sessionid", ajaxRequest.Session.SessionID, false).AddPostData("wizard_ajax", 1).AddPostData("gamepad", 0)
			.AddPostData("s", ajaxRequest.S, false).AddPostData("account", ajaxRequest.Account, false).AddPostData("email", HttpUtility.UrlEncode(email), false);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxEmailConfirm>(response.Data!)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

	/// <summary>
	/// Отправить код на почту, на которую нужно сменить
	/// </summary>
	/// <param name="email">Новая почта</param>
	/// <returns></returns>
	public static AjaxEmailConfirm AjaxAccountRecoveryChangeEmail(AjaxInfoRequest ajaxRequest, string email)
	{
		var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxAccountRecoveryChangeEmail, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = ajaxRequest.Referer,
			IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
		request.AddPostData("sessionid", ajaxRequest.Session.SessionID, false).AddPostData("wizard_ajax", 1).AddPostData("gamepad", 0)
			.AddPostData("s", ajaxRequest.S, false).AddPostData("account", ajaxRequest.Account, false).AddPostData("email", HttpUtility.UrlEncode(email), false);
		var response = Downloader.Post(request);
		if (!response.Success)
			return new();
		try
		{
			var obj = JsonSerializer.Deserialize<AjaxEmailConfirm>(response.Data!)!;
			return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

	/// <summary>
	/// Подтверждение смены почты
	/// </summary>
	/// <param name="email">Новая почта</param>
	/// <param name="email_change_code">Код смены с новой почты</param>
	/// <returns></returns>
	public static async Task<AjaxNext> AjaxAccountRecoveryConfirmChangeEmailAsync(AjaxInfoRequest ajaxRequest, string email, string email_change_code)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxAccountRecoveryConfirmChangeEmail, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = ajaxRequest.Referer,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddPostData("sessionid", ajaxRequest.Session.SessionID).AddPostData("wizard_ajax", 1).AddPostData("s", ajaxRequest.S)
            .AddPostData("gamepad", 0).AddPostData("account", ajaxRequest.Account).AddPostData("email", HttpUtility.UrlEncode(email), false)
            .AddPostData("email_change_code", email_change_code);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxNext>(response.Data!)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

	/// <summary>
	/// Подтверждение смены почты
	/// </summary>
	/// <param name="email">Новая почта</param>
	/// <param name="email_change_code">Код смены с новой почты</param>
	/// <returns></returns>
	public static AjaxNext AjaxAccountRecoveryConfirmChangeEmail(AjaxInfoRequest ajaxRequest, string email, string email_change_code)
	{
		var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxAccountRecoveryConfirmChangeEmail, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = ajaxRequest.Referer,
			IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
		request.AddPostData("sessionid", ajaxRequest.Session.SessionID).AddPostData("wizard_ajax", 1).AddPostData("s", ajaxRequest.S)
            .AddPostData("gamepad", 0).AddPostData("account", ajaxRequest.Account).AddPostData("email", HttpUtility.UrlEncode(email), false)
            .AddPostData("email_change_code", email_change_code);
		var response = Downloader.Post(request);
		if (!response.Success)
			return new();
		try
		{
			var obj = JsonSerializer.Deserialize<AjaxNext>(response.Data!)!;
			return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

	/// <summary>
	/// Проверка пароля
	/// </summary>
	/// <param name="password">пароль</param>
	/// <returns></returns>
	public static async Task<AjaxPasswordAvailable> AjaxCheckPasswordAvailableAsync(AjaxDefaultRequest ajaxRequest, string password, string referer)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxCheckPasswordAvailable, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = referer,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddPostData("sessionid", ajaxRequest.Session.SessionID).AddPostData("wizard_ajax", 1).AddPostData("password", password);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxPasswordAvailable>(response.Data!)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    public static async Task<AjaxNext> AjaxAccountRecoveryResetPhoneNumberAsync(AjaxInfoRequest ajaxRequest)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxAccountRecoveryResetPhoneNumber, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = ajaxRequest.Referer,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddPostData("sessionid", ajaxRequest.Session.SessionID).AddPostData("account", ajaxRequest.Account);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxNext>(response.Data!)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    public static async Task<AjaxNextStep> AjaxAccountRecoveryGetNextStepAsync(AjaxInfoRequest ajaxRequest)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxAccountRecoveryGetNextStep, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = ajaxRequest.Referer,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddPostData("sessionid", ajaxRequest.Session.SessionID).AddPostData("wizard_ajax", 1).AddPostData("s", ajaxRequest.S)
            .AddPostData("account", ajaxRequest.Account).AddPostData("reset", ajaxRequest.Reset.ToDigitReset())
            .AddPostData("issueid", ajaxRequest.Reset.ToDigitIssueId()).AddPostData("lost", ajaxRequest.Lost.ToDigitLost());
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxNextStep>(response.Data!)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    
    /// <summary>
    /// Получает RSA от steam
    /// </summary>
    /// <param name="login">Логин аккаунта</param>
    /// <param name="referer">Текущий url страницы</param>
    /// <returns>Класс содержащий данные RSA</returns>
    public static async Task<SteamRSA> GetRSAKeyAsync(AjaxDefaultRequest ajaxRequest, string login, string referer)
    {
        var request = new PostRequest(SteamCommunityUrls.Login_GetRSAKey, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = referer,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddPostData("donotcache", DateTime.UtcNow.ToTimeStamp()).AddPostData("username", login.ToLower());
        if (ajaxRequest.Session != null)
        {
            request.AddPostData("sessionid", ajaxRequest.Session.SessionID);
            request.Url = SteamPoweredUrls.Login_GetRSAKey;
        }
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<SteamRSA>(response.Data!)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    public static async Task<AjaxNext> AjaxAccountRecoveryChangePasswordAsync(AjaxWizardRequest ajaxRequest, string account, string password, SteamRSA rsa)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxAccountRecoveryChangePassword, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = ajaxRequest.Referer,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        string encrypt = Helpers.Encrypt(password, rsa.publickey_mod!, rsa.publickey_exp!);
        request.AddPostData("sessionid", ajaxRequest.Session.SessionID).AddPostData("wizard_ajax", 1)
            .AddPostData("s", ajaxRequest.S).AddPostData("account", account).AddPostData("password", encrypt)
            .AddPostData("rsatimestamp", rsa.timestamp!);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxNext>(response.Data!)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    
    public static async Task<AjaxLicense> AjaxRegisterKey(AjaxDefaultRequest ajaxRequest, string product_key)
    {
        var request = new PostRequest(SteamPoweredUrls.Account_AjaxRegisterKey, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = SteamPoweredUrls.Account_RegisterKey,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddPostData("product_key", product_key).AddPostData("sessionid", ajaxRequest.Session.SessionID);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxLicense>(response.Data!)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    public static async Task<PhoneAjax> PhoneAjaxAsync(AjaxDefaultRequest ajaxRequest)
    {
        var request = new PostRequest(SteamCommunityUrls.SteamGuard_PhoneAjax, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddPostData("op", "has_phone").AddPostData("arg", "null").AddPostData("sessionid", ajaxRequest.Session.SessionID);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<PhoneAjax>(response.Data!)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    public static PhoneAjax PhoneAjax(AjaxDefaultRequest ajaxRequest)
    {
        var request = new PostRequest(SteamCommunityUrls.SteamGuard_PhoneAjax, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddPostData("op", "has_phone").AddPostData("arg", "null").AddPostData("sessionid", ajaxRequest.Session.SessionID);
        var response = Downloader.Post(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<PhoneAjax>(response.Data!)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

	/// <summary>
	/// Отправляет пароль на проверку. Используется если нужно подтвердить владение аккаунтов, обычно после мобильного подтверждения, если отказ от почтового.
	/// </summary>
	/// <param name="rsa"></param>
	/// <param name="password">Оригинальный пароль (не зашифрованный)</param>
	/// <returns></returns>
	public static async Task<AjaxNext> AjaxAccountRecoveryVerifyPassword(AjaxInfoRequest ajaxRequest, SteamRSA rsa, string password)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxAccountRecoveryVerifyPassword, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddPostData("sessionid", ajaxRequest.Session.SessionID).AddPostData("s", ajaxRequest.S).AddPostData("lost", ajaxRequest.Lost.ToDigitLost())
            .AddPostData("reset", ajaxRequest.Reset.ToDigitReset()).AddPostData("password", Helpers.Encrypt(password, rsa.publickey_mod!, rsa.publickey_exp!))
            .AddPostData("rsatimestamp", rsa.timestamp!);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxNext>(response.Data!)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
}