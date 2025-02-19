using SteamWeb.Script.Models;
using SteamWeb.Web;
using System.Text.Json;
using SteamWeb.Script.Enums;
using SteamWeb.Extensions;
using System.Web;
using SteamWeb.Models;

namespace SteamWeb.Script;
/// <summary>
/// Здесь собраны все методы для выполнения запросов на поддомене help. С их помощью можно изменить номер телефона, снять guard, поменять почту или пароль.
/// </summary>
public static class AjaxHelp
{
    /// <summary>
    /// Проверка номера для регистрации его на аккаунт
    /// </summary>
    /// <param name="number">Номер как +7 9991112233</param>
    public static AjaxValidPhone AjaxValidNumber(DefaultRequest ajaxRequest, string number)
    {
        var request = new PostRequest(SteamPoweredUrls.Phone_Validate, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = SteamPoweredUrls.Phone_Add,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddPostData("sessionID", ajaxRequest.Session!.SessionID).AddPostData("phoneNumber", number);
        var response = Downloader.Post(request);
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
    /// Проверка номера для регистрации его на аккаунт
    /// </summary>
    /// <param name="number">Номер как +7 9991112233</param>
    public static async Task<AjaxValidPhone> AjaxValidNumberAsync(DefaultRequest ajaxRequest, string number)
    {
        var request = new PostRequest(SteamPoweredUrls.Phone_Validate, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = SteamPoweredUrls.Phone_Add,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddPostData("sessionID", ajaxRequest.Session!.SessionID).AddPostData("phoneNumber", number);
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
    public static async Task<AjaxOp> AjaxAddjaxopAsync(DefaultRequest ajaxRequest, string input, OP_CODES op, bool confirmed = true)
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
        request.AddPostData("op", op.ToStringValue()).AddPostData("input", input).AddPostData("sessionID", ajaxRequest.Session!.SessionID)
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
    /// Отправка запроса на смену данных в мобильное приложение\код на почту\телефон
    /// </summary>
    public static AjaxDefault AjaxSendAccountRecoveryCode(AjaxInfoRequest ajaxRequest)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxSendAccountRecoveryCode, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = ajaxRequest.Referer,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddPostData("sessionid", ajaxRequest.Session!.SessionID, false).AddPostData("wizard_ajax", 1).AddPostData("gamepad", 0)
            .AddPostData("s", ajaxRequest.S).AddPostData("method", ajaxRequest.Method.ToDigitMethod()).AddPostData("link", string.Empty, false);
        var response = Downloader.Post(request);
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
    /// Отправка запроса на смену данных в мобильное приложение\код на почту\телефон
    /// </summary>
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
        request.AddPostData("sessionid", ajaxRequest.Session!.SessionID, false).AddPostData("wizard_ajax", 1).AddPostData("gamepad", 0)
            .AddPostData("s", ajaxRequest.S).AddPostData("method", ajaxRequest.Method.ToDigitMethod()).AddPostData("link", string.Empty, false);
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
    public static AjaxPollRecoveryConf AjaxPollAccountRecoveryConfirmation(AjaxInfoRequest ajaxRequest)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxPollAccountRecoveryConfirmation, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = ajaxRequest.Referer,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddPostData("sessionid", ajaxRequest.Session!.SessionID, false).AddPostData("wizard_ajax", 1).AddPostData("s", ajaxRequest.S)
            .AddPostData("reset", ajaxRequest.Reset.ToDigitReset()).AddPostData("lost", TypeLost.RecoveryConfirm.ToDigitLost()).AddPostData("gamepad", 0)
            .AddPostData("method", ajaxRequest.Method.ToDigitMethod()).AddPostData("issueid", ajaxRequest.Reset.ToDigitIssueId());
        var response = Downloader.Post(request);
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
    /// Проверка подтверждения на смену данных. Для продолжения должно быть:
    /// <code>
    /// if (AjaxPollRecoveryConf.success && !AjaxPollRecoveryConf.continue) return;
    /// </code>
    /// </summary>
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
        request.AddPostData("sessionid", ajaxRequest.Session!.SessionID, false).AddPostData("wizard_ajax", 1).AddPostData("s", ajaxRequest.S)
            .AddPostData("reset", ajaxRequest.Reset.ToDigitReset()).AddPostData("lost", TypeLost.RecoveryConfirm.ToDigitLost()).AddPostData("gamepad", 0)
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
    /// <param name="code">Код с телефона\почты; для мобильного подтверждения оставить пустую строку</param>
    public static AjaxNext AjaxVerifyAccountRecoveryCode(AjaxInfoRequest ajaxRequest, string code)
    {
        var request = new GetRequest(SteamPoweredUrls.Wizard_AjaxVerifyAccountRecoveryCode)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            IsAjax = true,
            Referer = ajaxRequest.Referer,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddQuery("code", code, false).AddQuery("s", ajaxRequest.S).AddQuery("reset", ajaxRequest.Reset.ToDigitReset())
            .AddQuery("lost", TypeLost.RecoveryConfirm.ToDigitLost()).AddQuery("method", ajaxRequest.Method.ToDigitMethod())
            .AddQuery("issueid", ajaxRequest.Reset.ToDigitIssueId()).AddQuery("sessionid", ajaxRequest.Session!.SessionID)
            .AddQuery("wizard_ajax", 1).AddQuery("gamepad", 0);
        var response = Downloader.Get(request);
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
    /// Проверка подтверждения\кода с почты
    /// </summary>
    /// <param name="code">Код с телефона\почты; для мобильного подтверждения оставить пустую строку</param>
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
        request.AddQuery("code", code, false).AddQuery("s", ajaxRequest.S).AddQuery("reset", ajaxRequest.Reset.ToDigitReset())
            .AddQuery("lost", TypeLost.RecoveryConfirm.ToDigitLost()).AddQuery("method", ajaxRequest.Method.ToDigitMethod())
            .AddQuery("issueid", ajaxRequest.Reset.ToDigitIssueId()).AddQuery("sessionid", ajaxRequest.Session!.SessionID)
            .AddQuery("wizard_ajax", 1).AddQuery("gamepad", 0);
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
        request.AddPostData("sessionid", ajaxRequest.Session!.SessionID, false).AddPostData("wizard_ajax", 1).AddPostData("gamepad", 0)
            .AddPostData("s", ajaxRequest.S).AddPostData("account", ajaxRequest.Account).AddPostData("email", Uri.EscapeDataString(email), false);
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
    /// Отправить код на почту, на которую нужно сменить
    /// </summary>
    /// <param name="email">Новая почта</param>
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
        request.AddPostData("sessionid", ajaxRequest.Session!.SessionID, false).AddPostData("wizard_ajax", 1).AddPostData("gamepad", 0)
            .AddPostData("s", ajaxRequest.S).AddPostData("account", ajaxRequest.Account).AddPostData("email", Uri.EscapeDataString(email), false);
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
        request.AddPostData("sessionid", ajaxRequest.Session!.SessionID, false).AddPostData("wizard_ajax", 1).AddPostData("s", ajaxRequest.S)
            .AddPostData("gamepad", 0).AddPostData("account", ajaxRequest.Account).AddPostData("email", Uri.EscapeDataString(email), false)
            .AddPostData("email_change_code", email_change_code, false);
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
    /// Подтверждение смены почты
    /// </summary>
    /// <param name="email">Новая почта</param>
    /// <param name="email_change_code">Код смены с новой почты</param>
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
        request.AddPostData("sessionid", ajaxRequest.Session!.SessionID).AddPostData("wizard_ajax", 1).AddPostData("s", ajaxRequest.S)
            .AddPostData("gamepad", 0).AddPostData("account", ajaxRequest.Account).AddPostData("email", Uri.EscapeDataString(email), false)
            .AddPostData("email_change_code", email_change_code, false);
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
    /// Проверка пароля на скомпрометированость, перед его изменением на аккаунте
    /// </summary>
    /// <param name="password">Пароль</param>
    public static AjaxPasswordAvailable AjaxCheckPasswordAvailable(DefaultRequest ajaxRequest, string password, string referer)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxCheckPasswordAvailable, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = referer,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddPostData("sessionid", ajaxRequest.Session!.SessionID).AddPostData("wizard_ajax", 1).AddPostData("password", password);
        var response = Downloader.Post(request);
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
    /// <summary>
    /// Проверка пароля на скомпрометированость, перед его изменением на аккаунте
    /// </summary>
    /// <param name="password">Пароль</param>
    public static async Task<AjaxPasswordAvailable> AjaxCheckPasswordAvailableAsync(DefaultRequest ajaxRequest, string password, string referer)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxCheckPasswordAvailable, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = referer,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddPostData("sessionid", ajaxRequest.Session!.SessionID).AddPostData("wizard_ajax", 1).AddPostData("password", password);
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
        request.AddPostData("sessionid", ajaxRequest.Session!.SessionID).AddPostData("account", ajaxRequest.Account);
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
    /// Вызывается для перехода на следующий шаг\пропуск текущего
    /// </summary>
    public static AjaxNextStep AjaxAccountRecoveryGetNextStep(AjaxInfoRequest ajaxRequest)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxAccountRecoveryGetNextStep, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = ajaxRequest.Referer,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddPostData("sessionid", ajaxRequest.Session!.SessionID).AddPostData("wizard_ajax", 1).AddPostData("gamepad", 0).AddPostData("s", ajaxRequest.S)
            .AddPostData("account", ajaxRequest.Account).AddPostData("reset", ajaxRequest.Reset.ToDigitReset())
            .AddPostData("issueid", ajaxRequest.Reset.ToDigitIssueId()).AddPostData("lost", ajaxRequest.Lost.ToDigitLost());
        var response = Downloader.Post(request);
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
    /// Вызывается для перехода на следующий шаг\пропуск текущего
    /// </summary>
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
        request.AddPostData("sessionid", ajaxRequest.Session!.SessionID).AddPostData("wizard_ajax", 1).AddPostData("gamepad", 0).AddPostData("s", ajaxRequest.S)
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
    /// Получает RSA от steam. Для получения rsa с провайдера подтверждения владения аккаунтов нужно указать текущую сессию аккаунта, иначе без сессии.
    /// </summary>
    /// <param name="login">Логин аккаунта</param>
    /// <param name="referer">Текущий url страницы</param>
    /// <returns>Класс содержащий данные RSA</returns>
    public static SteamRSA GetRSAKey(DefaultRequest ajaxRequest, string login, string referer)
    {
        var request = new PostRequest(SteamCommunityUrls.Login_GetRSAKey, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = referer,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddPostData("username", login.ToLower()).AddPostData("wizard_ajax", 1).AddPostData("gamepad", 0);
        if (ajaxRequest.Session != null)
        {
            request.AddPostData("sessionid", ajaxRequest.Session.SessionID);
            request.Url = SteamPoweredUrls.Wizard_Login_GetRSAKey;
        }
        var response = Downloader.Post(request);
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
    /// <summary>
    /// Получает RSA от steam. Для получения rsa с провайдера подтверждения владения аккаунтов нужно указать текущую сессию аккаунта, иначе без сессии.
    /// </summary>
    /// <param name="login">Логин аккаунта</param>
    /// <param name="referer">Текущий url страницы</param>
    /// <returns>Класс содержащий данные RSA</returns>
    public static async Task<SteamRSA> GetRSAKeyAsync(DefaultRequest ajaxRequest, string login, string referer)
    {
        var request = new PostRequest(SteamCommunityUrls.Login_GetRSAKey, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = referer,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddPostData("username", login.ToLower()).AddPostData("wizard_ajax", 1).AddPostData("gamepad", 0);
        if (ajaxRequest.Session != null)
        {
            request.AddPostData("sessionid", ajaxRequest.Session.SessionID);
            request.Url = SteamPoweredUrls.Wizard_Login_GetRSAKey;
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

    /// <summary>
    /// Вызывается для изменения пароля на аккаунте
    /// </summary>
    /// <param name="password">Новый пароль от аккаунта</param>
    public static AjaxNext AjaxAccountRecoveryChangePassword(AjaxWizardRequest ajaxRequest, uint account, string password, SteamRSA rsa)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxAccountRecoveryChangePassword, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = ajaxRequest.Referer,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        string encrypt = Helpers.Encrypt(password, rsa.PublicKeyMod!, rsa.PublicKeyExp!);
        request.AddPostData("sessionid", ajaxRequest.Session!.SessionID, false).AddPostData("wizard_ajax", 1).AddPostData("gamepad", 0)
            .AddPostData("s", ajaxRequest.S).AddPostData("account", account).AddPostData("password", Uri.EscapeDataString(encrypt), false)
            .AddPostData("rsatimestamp", rsa.Timestamp!, false);
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
    /// Вызывается для изменения пароля на аккаунте
    /// </summary>
    /// <param name="password">Новый пароль от аккаунта</param>
    public static async Task<AjaxNext> AjaxAccountRecoveryChangePasswordAsync(AjaxWizardRequest ajaxRequest, uint account, string password, SteamRSA rsa)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxAccountRecoveryChangePassword, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = ajaxRequest.Referer,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        string encrypt = Helpers.Encrypt(password, rsa.PublicKeyMod!, rsa.PublicKeyExp!);
        request.AddPostData("sessionid", ajaxRequest.Session!.SessionID, false).AddPostData("wizard_ajax", 1).AddPostData("gamepad", 0)
            .AddPostData("s", ajaxRequest.S).AddPostData("account", account).AddPostData("password", Uri.EscapeDataString(encrypt), false)
            .AddPostData("rsatimestamp", rsa.Timestamp!, false);
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
    /// Производит регистрацию ключа на аккаунте
    /// </summary>
    /// <param name="product_key">Ключ для регистрации</param>
    /// <returns>Информация о статусе регистрации ключа</returns>
    [Obsolete("Данный метод теперь находится в Ajax.account_registerkey_async")]
    public static async Task<AjaxLicense> AjaxRegisterKeyAsync(DefaultRequest ajaxRequest, string product_key)
    {
        var request = new PostRequest(SteamPoweredUrls.Account_AjaxRegisterKey, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = SteamPoweredUrls.Account_RegisterKey,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddPostData("product_key", product_key).AddPostData("sessionid", ajaxRequest.Session!.SessionID);
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
    /// <summary>
    /// Производит регистрацию ключа на аккаунте
    /// </summary>
    /// <param name="product_key">Ключ для регистрации</param>
    /// <returns>Информация о статусе регистрации ключа</returns>
    [Obsolete("Данный метод теперь находится в Ajax.account_registerkey")]
    public static AjaxLicense AjaxRegisterKey(DefaultRequest ajaxRequest, string product_key)
    {
        var request = new PostRequest(SteamPoweredUrls.Account_AjaxRegisterKey, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = SteamPoweredUrls.Account_RegisterKey,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddPostData("product_key", product_key).AddPostData("sessionid", ajaxRequest.Session!.SessionID);
        var response = Downloader.Post(request);
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

    public static PhoneAjax PhoneAjax(DefaultRequest ajaxRequest)
    {
        var request = new PostRequest(SteamCommunityUrls.SteamGuard_PhoneAjax, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddPostData("op", "has_phone").AddPostData("arg", "null").AddPostData("sessionid", ajaxRequest.Session!.SessionID);
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
    public static async Task<PhoneAjax> PhoneAjaxAsync(DefaultRequest ajaxRequest)
    {
        var request = new PostRequest(SteamCommunityUrls.SteamGuard_PhoneAjax, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        request.AddPostData("op", "has_phone").AddPostData("arg", "null").AddPostData("sessionid", ajaxRequest.Session!.SessionID);
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

    /// <summary>
    /// Отправляет пароль на проверку. Используется если нужно подтвердить владение аккаунтов.
    /// </summary>
    /// <param name="password">Оригинальный пароль (не зашифрованный)</param>
    public static AjaxNext AjaxAccountRecoveryVerifyPassword(AjaxInfoRequest ajaxRequest, SteamRSA rsa, string password)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxAccountRecoveryVerifyPassword, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        var crypted = Helpers.Encrypt(password, rsa.PublicKeyMod!, rsa.PublicKeyExp!);
        request.AddPostData("sessionid", ajaxRequest.Session!.SessionID).AddPostData("s", ajaxRequest.S)
            .AddPostData("lost", ajaxRequest.Lost.ToDigitLost()).AddPostData("reset", ajaxRequest.Reset.ToDigitReset())
            .AddPostData("password", Uri.EscapeDataString(crypted), false)
            .AddPostData("rsatimestamp", rsa.Timestamp!);
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
    /// Отправляет пароль на проверку. Используется если нужно подтвердить владение аккаунтов.
    /// </summary>
    /// <param name="password">Оригинальный пароль (не зашифрованный)</param>
    public static async Task<AjaxNext> AjaxAccountRecoveryVerifyPasswordAsync(AjaxInfoRequest ajaxRequest, SteamRSA rsa, string password)
    {
        var request = new PostRequest(SteamPoweredUrls.Wizard_AjaxAccountRecoveryVerifyPassword, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            IsAjax = true,
            CancellationToken = ajaxRequest.CancellationToken,
        };
        var crypted = Helpers.Encrypt(password, rsa.PublicKeyMod!, rsa.PublicKeyExp!);
        request.AddPostData("sessionid", ajaxRequest.Session!.SessionID).AddPostData("s", ajaxRequest.S)
            .AddPostData("lost", ajaxRequest.Lost.ToDigitLost()).AddPostData("reset", ajaxRequest.Reset.ToDigitReset())
            .AddPostData("password", Uri.EscapeDataString(crypted), false)
            .AddPostData("rsatimestamp", rsa.Timestamp!);
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