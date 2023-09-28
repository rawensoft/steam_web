using System.Text.Json;
using SteamWeb.Web;
using SteamWeb.Models;
using SteamWeb.Extensions;
using AngleSharp.Html.Parser;
using System.Globalization;
using SteamWeb.Models.Trade;
using System.Text.RegularExpressions;
using System.Web;
using SteamWeb.Auth.Interfaces;
using SteamWeb.Auth.v2.Enums;

using LoginResultv1 = SteamWeb.Auth.v1.Enums.LoginResult;
using SessionDatav1 = SteamWeb.Auth.v1.Models.SessionData;
using UserLoginv1 = SteamWeb.Auth.v1.UserLogin;
using SteamGuardAccuntv1 = SteamWeb.Auth.v1.SteamGuardAccount;
using SignInPlatform = SteamWeb.Auth.v1.Enums.SignInPlatform;

using LoginResultv2 = SteamWeb.Auth.v2.Enums.LoginResult;
using SessionDatav2 = SteamWeb.Auth.v2.Models.SessionData;
using UserLoginv2 = SteamWeb.Auth.v2.UserLogin;
using SteamGuardAccuntv2 = SteamWeb.Auth.v2.SteamGuardAccount;
using SteamWeb.API.Models.IEconService;

namespace SteamWeb;
public static partial class Steam
{
    /// <summary>
    /// Значение которое отнимается или прибавляется к steamid64\steamid32
    /// </summary>
    public const ulong SteamIDConverter = 76561197960265728;
    static Regex rgxTradeurl1 = new(@"https://steamcommunity\.com/tradeoffer/new/\?partner=\d{1,12}&token=\S{4,10}", RegexOptions.Compiled);
    
    public static async Task<bool> SwitchToMailCodeAsync(ISessionProvider session, System.Net.IWebProxy proxy, SteamGuardAccuntv2 SDA)
    {
        var att_phone = await Script.AjaxHelp.PhoneAjaxAsync(session, proxy);
        if (att_phone.has_phone == null)
            return false;
        if (att_phone.has_phone == true && SDA != null)
        {
            SDA.Proxy = proxy;
            var status = await Task.Run(() => SDA.DeactivateAuthenticator());
            return status;
        }
        var request = new PostRequest(SteamPoweredUrls.TwoFactor_ManageAction, Downloader.AppFormUrlEncoded)
            .AddPostData("action", "email").AddPostData("sessionid", session.SessionID).AddPostData("email_authenticator_check", "on");
        var response = await Downloader.PostAsync(request);
        return response.Success;
    }
    public static bool SwitchToMailCode(ISessionProvider session, System.Net.IWebProxy proxy, SteamGuardAccuntv2 SDA)
    {
        var att_phone = Script.AjaxHelp.PhoneAjax(session, proxy);
        if (att_phone.has_phone == null)
            return false;
        if (att_phone.has_phone == true && SDA != null)
        {
            SDA.Proxy = proxy;
            return SDA.DeactivateAuthenticator();
        }
        var request = new PostRequest(SteamPoweredUrls.TwoFactor_ManageAction, Downloader.AppFormUrlEncoded)
            .AddPostData("action", "email").AddPostData("sessionid", session.SessionID).AddPostData("email_authenticator_check", "on");
        var response = Downloader.Post(request);
        return response.Success;
    }
    public static async Task<(bool, string)> SwitchToNonGuardAsync(ISessionProvider session, System.Net.IWebProxy proxy)
    {
        var response = await Downloader.GetAsync(new(SteamPoweredUrls.TwoFactor_ManageAction, proxy, session));
        if (!response.Success)
            return (false, response.ErrorMessage!);

        var request = new PostRequest(SteamPoweredUrls.TwoFactor_ManageAction, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy
        }
        .AddPostData("action", "none").AddPostData("sessionid", session.SessionID).AddPostData("none_authenticator_check", "on");
        response = await Downloader.PostAsync(request);
        if (!response.Success)
            return (false, response.ErrorMessage!);

        request.PostData.Clear();
        request.AddPostData("action", "actuallynone").AddPostData("sessionid", session.SessionID);
        response = await Downloader.PostAsync(request);
        if (!response.Success)
            return (false, response.ErrorMessage!);

        var doc = new HtmlParser().ParseDocument(response.Data);
        var elements = doc.GetElementsByClassName("phone_box");
        if (elements.Length > 0)
        {
            string text = elements[0].TextContent.Replace("\t", "").Replace("\r", "").Replace("\n", "");
            return (true, text);
        }
        return (false, response.Data);
    }
    public static (bool, string) SwitchToNonGuard(ISessionProvider session, System.Net.IWebProxy proxy)
    {
        var response = Downloader.Get(new(SteamPoweredUrls.TwoFactor_ManageAction, proxy, session));
        if (!response.Success)
            return (false, response.ErrorMessage!);

        var request = new PostRequest(SteamPoweredUrls.TwoFactor_ManageAction, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy
        }
        .AddPostData("action", "none").AddPostData("sessionid", session.SessionID).AddPostData("none_authenticator_check", "on");
        response = Downloader.Post(request);
        if (!response.Success)
            return (false, response.ErrorMessage!);

        request.PostData.Clear();
        request.AddPostData("action", "actuallynone").AddPostData("sessionid", session.SessionID);
        response = Downloader.Post(request);
        if (!response.Success)
            return (false, response.ErrorMessage!);

        var doc = new HtmlParser().ParseDocument(response.Data);
        var elements = doc.GetElementsByClassName("phone_box");
        if (elements.Length > 0)
        {
            string text = elements[0].TextContent.Replace("\t", "").Replace("\r", "").Replace("\n", "");
            return (true, text);
        }
        return (false, response.Data);
    }

    /// <summary>
    /// Проверяет имеется ли community ban на аккаунте
    /// </summary>
    /// <param name="session"></param>
    /// <param name="proxy"></param>
    /// <returns>True имеется</returns>
    public static async Task<bool> CheckOnKTAsync(ISessionProvider session, System.Net.IWebProxy proxy)
    {
        var response = await Downloader.GetAsync(new(SteamCommunityUrls.My_Edit_Info, proxy, session));
        if (response.Data?.Contains("profile_fatalerror") == true &&
            response.Data?.Contains("profile_fatalerror_message") == true)
            return true;
        return false;
    }
    /// <summary>
    /// Проверяет имеется ли community ban на аккаунте
    /// </summary>
    /// <param name="session"></param>
    /// <param name="proxy"></param>
    /// <returns>True имеется</returns>
    public static bool CheckOnKT(ISessionProvider session, System.Net.IWebProxy proxy)
    {
        var response = Downloader.Get(new(SteamCommunityUrls.My_Edit_Info, proxy, session));
        if (response.Data?.Contains("profile_fatalerror") == true &&
            response.Data?.Contains("profile_fatalerror_message") == true)
            return true;
        return false;
    }

    /// <summary>
    /// Создаёт трейд
    /// </summary>
    /// <returns>ConfTradeOffer != null если трейд создался и SteamTradeError != null если трейд не создался</returns>
    public static (ConfTradeOffer?, SteamTradeError?) CreateTrade(ISessionProvider session, System.Net.IWebProxy proxy, NewTradeOffer trade,
        Token token, string tradeoffermessage, uint offerpartner)
    {
        var request = new PostRequest(SteamCommunityUrls.TradeOffer_New_Send, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            IsAjax = true,
            Referer = $"{SteamCommunityUrls.TradeOffer_New}/?partner={offerpartner}"
        }
        .AddPostData("sessionid", session.SessionID).AddPostData("serverid", 1).AddPostData("partner", Steam32ToSteam64(offerpartner))
        .AddPostData("tradeoffermessage", tradeoffermessage, true).AddPostData("json_tradeoffer", HttpUtility.UrlEncode(JsonSerializer.Serialize(trade)), false)
        .AddPostData("captcha", "").AddPostData("trade_offer_create_params", HttpUtility.UrlEncode(JsonSerializer.Serialize(token)), false);
        try
        {
            var response = Downloader.Post(request);
            if (!response.Success)
            {
                var steamerror = SteamTradeError.Deserialize(response.Data);
                return (null, steamerror);
            }
            try
			{
				var options = new JsonSerializerOptions
				{
					NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
				};
				var conf = JsonSerializer.Deserialize<ConfTradeOffer>(response.Data!, options);
                return (conf, null);
            }
            catch (Exception ex)
            {
                return (null, new() { strError = ex.Message });
            }
        }
        catch (Exception ex)
        {
            return (null, new() { strError = ex.Message });
        }
    }
    /// <summary>
    /// Создаёт трейд
    /// </summary>
    /// <returns>ConfTradeOffer != null если трейд создался и SteamTradeError != null если трейд не создался</returns>
    public static async Task<(ConfTradeOffer?, SteamTradeError?)> CreateTradeAsync(ISessionProvider session, System.Net.IWebProxy proxy,
        NewTradeOffer trade, Token token, string tradeoffermessage, uint offerpartner)
    {
        var request = new PostRequest(SteamCommunityUrls.TradeOffer_New_Send, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            IsAjax = true,
            Referer = $"{SteamCommunityUrls.TradeOffer_New}/?partner={offerpartner}"
        }
        .AddPostData("sessionid", session.SessionID).AddPostData("serverid", 1).AddPostData("partner", Steam32ToSteam64(offerpartner))
        .AddPostData("tradeoffermessage", HttpUtility.UrlEncode(tradeoffermessage), false).AddPostData("json_tradeoffer", HttpUtility.UrlEncode(JsonSerializer.Serialize(trade)), false)
        .AddPostData("captcha", "").AddPostData("trade_offer_create_params", HttpUtility.UrlEncode(JsonSerializer.Serialize(token)), false);
        try
        {
            var response = await Downloader.PostAsync(request);
            if (!response.Success)
            {
                var steamerror = SteamTradeError.Deserialize(response.Data);
                return (null, steamerror);
            }
            try
			{
				var options = new JsonSerializerOptions
				{
					NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
				};
				var conf = JsonSerializer.Deserialize<ConfTradeOffer>(response.Data!, options);
                return (conf, null);
            }
            catch (Exception ex)
            {
                return (null, new() { strError = ex.Message });
            }
        }
        catch (Exception ex)
        {
            return (null, new() { strError = ex.Message });
        }
    }

    /// <summary>
    /// Изменяет язык на steam аккаунте
    /// </summary>
    /// <param name="language">язык для смены, пример: russian, english</param>
    /// <returns>True язык изменён</returns>
    public static async Task<bool> ChangeLanguageAsync(ISessionProvider session, System.Net.IWebProxy proxy, string language)
    {
        var request = new PostRequest(SteamCommunityUrls.Actions_SetLanguage, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy
        }
        .AddPostData("language", language).AddPostData("sessionid", session.SessionID);
        var response = await Downloader.PostAsync(request);
        return response.Success;
    }
    /// <summary>
    /// Изменяет язык на steam аккаунте
    /// </summary>
    /// <param name="language">язык для смены, пример: russian, english</param>
    /// <returns>True язык изменён</returns>
    public static bool ChangeLanguage(ISessionProvider session, System.Net.IWebProxy proxy, string language)
    {
        var request = new PostRequest(SteamCommunityUrls.Actions_SetLanguage, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy
        }
        .AddPostData("language", language).AddPostData("sessionid", session.SessionID);
        var response = Downloader.Post(request);
        return response.Success;
    }

    /// <summary>
    /// Получает информацию со страницы <see href="https://store.steampowered.com/account/">store.steampowered.com/account</see>
    /// </summary>
    /// <returns>Полученные данные</returns>
    public static async Task<AboutProfile> Get2FAAsync(ISessionProvider session, System.Net.IWebProxy proxy)
    {
        var response = await Downloader.GetAsync(new(SteamPoweredUrls.Account, proxy, session));
        if (!response.Success)
            return new();
        return AboutProfile.Deserialize(response.Data);
    }
    /// <summary>
    /// Получает информацию со страницы <see href="https://store.steampowered.com/account/">store.steampowered.com/account</see>
    /// </summary>
    /// <returns>Полученные данные</returns>
    public static AboutProfile Get2FA(ISessionProvider session, System.Net.IWebProxy proxy)
    {
        var response = Downloader.Get(new(SteamPoweredUrls.Account, proxy, session));
        if (!response.Success)
            return new();
        return AboutProfile.Deserialize(response.Data);
    }

    public static async Task<WebApiKey> GetWebAPIKeyAsync(ISessionProvider session, System.Net.IWebProxy proxy)
    {
        var response = await Downloader.GetAsync(new(SteamCommunityUrls.Dev_APIKey, proxy, session));
        if (!response.Success)
            return new WebApiKey(response.Data);
        else if (response.Data == "<!DOCTYPE html>")
        {
            var to1 = new WebApiKey("Бан\\блок на запросы.");
            return to1;
        }
        else if (response.Data.Contains("<p><a style=\"font-size: 16px;\" href=\"https://support.steampowered.com/kb_article.php?ref=3330-IAGK-7663\">"))
        {
            var to1 = new WebApiKey(response.Data.GetBetween("<p>", "</p>"));
            return to1;
        }
        else if (response.Data.Contains("Register for a new Steam Web API Key"))
        {
            var request = new PostRequest(SteamCommunityUrls.Dev_RegisterKey, Downloader.AppFormUrlEncoded)
            {
                Session = session,
                Proxy = proxy
            }.AddPostData("domain", "localhost").AddPostData("agreeToTerms", "agreed").AddPostData("sessionid", session.SessionID).AddPostData("Submit", "Зарегистрировать");
            response = await Downloader.PostAsync(request);
        }
        else if (response.Data.Contains("Access Denied"))
        {
            var to1 = new WebApiKey("Access Denied");
            return to1;
        }
        else if (response.Data.Contains("A new free account") || response.Data.Contains("Sign In"))
        {
            var to1 = new WebApiKey("The session has expired. The session needs to be updated.");
            return to1;
        }
        HtmlParser html = new HtmlParser();
        var parser = await html.ParseDocumentAsync(response.Data);
        var children = parser.GetElementById("bodyContents_ex")?.Children;
        if (children == null)
            return new("Неверная страница");
        var to = new WebApiKey(children[2].InnerHtml.Replace(" ", "").Split(':')[1], children[1].InnerHtml.Replace(" ", "").Split(':')[1]);
        return to;
    }
    public static WebApiKey GetWebAPIKey(ISessionProvider session, System.Net.IWebProxy proxy)
    {
        var response = Downloader.Get(new(SteamCommunityUrls.Dev_APIKey, proxy, session));
        if (!response.Success)
            return new WebApiKey(response.Data);
        else if (response.Data == "<!DOCTYPE html>")
        {
            var to1 = new WebApiKey("Бан\\блок на запросы.");
            return to1;
        }
        else if (response.Data.Contains("<p><a style=\"font-size: 16px;\" href=\"https://support.steampowered.com/kb_article.php?ref=3330-IAGK-7663\">"))
        {
            var to1 = new WebApiKey(response.Data.GetBetween("<p>", "</p>"));
            return to1;
        }
        else if (response.Data.Contains("Register for a new Steam Web API Key"))
        {
            var request = new PostRequest(SteamCommunityUrls.Dev_RegisterKey, Downloader.AppFormUrlEncoded)
            {
                Session = session,
                Proxy = proxy
            }.AddPostData("domain", "localhost").AddPostData("agreeToTerms", "agreed").AddPostData("sessionid", session.SessionID).AddPostData("Submit", "Зарегистрировать");
            response = Downloader.Post(request);
        }
        else if (response.Data.Contains("Access Denied"))
        {
            var to1 = new WebApiKey("Access Denied");
            return to1;
        }
        else if (response.Data.Contains("A new free account") || response.Data.Contains("Sign In"))
        {
            var to1 = new WebApiKey("The session has expired. The session needs to be updated.");
            return to1;
        }
        HtmlParser html = new HtmlParser();
        var parser = html.ParseDocument(response.Data);
        var children = parser.GetElementById("bodyContents_ex")?.Children;
        if (children == null)
            return new("Неверная страница");
        var to = new WebApiKey(children[2].InnerHtml.Replace(" ", "").Split(':')[1], children[1].InnerHtml.Replace(" ", "").Split(':')[1]);
        return to;
    }

    /// <summary>
    /// Загружает текущее состояние доступа к маркету для аккаунта (webTradeEligibilityState)
    /// </summary>
    /// <returns>Null если нет данных</returns>
    public static WebTradeEligibility? GetWebTradeEligibility(ISessionProvider session, System.Net.IWebProxy proxy)
    {
        var response = Downloader.Get(new(SteamCommunityUrls.Market_EligibilityCheck, proxy, session));
        if (!response.Success || response.Cookie == null)
            return null;
        else if ((!response.Data.Contains("menuitem supernav username persona_name_text_content") &&
            !response.Data.Contains("whiteLink persona_name_text_content")) ||
            response.Data.Contains("see, edit, or remove your Community Market listings."))
            return null;
        foreach (var item in response.Cookie.Split(';'))
        {
            if (item.Replace(" ", "").StartsWith("webTradeEligibility"))
            {
                var web = item.Replace(" ", "").Split('=')[1];
                web = HttpUtility.UrlDecode(web);
                var obj = JsonSerializer.Deserialize<WebTradeEligibility>(web);
                return obj;
            }
        }
        return null;
    }
    /// <summary>
    /// Загружает текущее состояние доступа к маркету для аккаунта (webTradeEligibilityState)
    /// </summary>
    /// <returns>Null если нет данных</returns>
    public static async Task<WebTradeEligibility?> GetWebTradeEligibilityAsync(ISessionProvider session, System.Net.IWebProxy proxy)
    {
        var response = await Downloader.GetAsync(new(SteamCommunityUrls.Market_EligibilityCheck, proxy, session));
        if (!response.Success || response.Cookie == null)
            return null;
        else if ((!response.Data.Contains("menuitem supernav username persona_name_text_content") &&
            !response.Data.Contains("whiteLink persona_name_text_content")) ||
            response.Data.Contains("see, edit, or remove your Community Market listings."))
            return null;
        foreach (var item in response.Cookie.Split(';'))
        {
            if (item.Replace(" ", "").StartsWith("webTradeEligibility"))
            {
                var web = item.Replace(" ", "").Split('=')[1];
                web = System.Web.HttpUtility.UrlDecode(web);
                var obj = JsonSerializer.Deserialize<WebTradeEligibility>(web);
                return obj;
            }
        }
        return null;
    }

    /// <summary>
    /// Получает trade ссылку и разрешения маркета
    /// </summary>
    /// <returns>true если запрос выполнен успешно, Null если трейд ссылка не найдена, Null если разрешений маркета нет</returns>
    public static (bool, string?, WebTradeEligibility?) GetTradeURL(ISessionProvider session, System.Net.IWebProxy proxy)
    {
        var response = Downloader.Get(new(SteamCommunityUrls.Market_EligibilityCheck, proxy, session));
        if (!response.Success)
            return (false, null, null);
        else if (response.Data.Contains("link_forgot_password"))
            return (false, null, null);

        string? trade_url = null;
        HtmlParser html = new HtmlParser();
        var doc = html.ParseDocument(response.Data);
        var el = doc.GetElementById("trade_offer_access_url");
        if (el != null) trade_url = ((AngleSharp.Html.Dom.IHtmlInputElement)el).DefaultValue.GetClearWebString();

        WebTradeEligibility? webState = null;
        if (!response.Cookie.IsEmpty())
        {
            foreach (var item in response.Cookie.Split(';'))
            {
                if (item.Replace(" ", "").StartsWith("webTradeEligibility"))
                {
                    var web = item.Replace(" ", "").Split('=')[1];
                    web = HttpUtility.UrlDecode(web);
                    webState = JsonSerializer.Deserialize<WebTradeEligibility>(web);
                    break;
                }
            }
        }
        return (true, trade_url.IsEmpty() ? null : trade_url, webState);
    }
    /// <summary>
    /// Получает trade ссылку и разрешения маркета
    /// </summary>
    /// <returns>true если запрос выполнен успешно, Null если трейд ссылка не найдена, Null если разрешений маркета нет</returns>
    public static async Task<(bool, string?, WebTradeEligibility?)> GetTradeURLAsync(ISessionProvider session, System.Net.IWebProxy proxy)
    {
        var response = await Downloader.GetAsync(new(SteamCommunityUrls.Market_EligibilityCheck, proxy, session));
        if (!response.Success)
            return (false, null, null);
        else if (response.Data.Contains("link_forgot_password"))
            return (false, null, null);

        string? trade_url = null;
        HtmlParser html = new HtmlParser();
        var doc = html.ParseDocument(response.Data);
        var el = doc.GetElementById("trade_offer_access_url");
        if (el != null) trade_url = ((AngleSharp.Html.Dom.IHtmlInputElement)el).DefaultValue.GetClearWebString();

        WebTradeEligibility? webState = null;
        if (!response.Cookie.IsEmpty())
        {
            foreach (var item in response.Cookie.Split(';'))
            {
                if (item.Replace(" ", "").StartsWith("webTradeEligibility"))
                {
                    var web = item.Replace(" ", "").Split('=')[1];
                    web = HttpUtility.UrlDecode(web);
                    webState = JsonSerializer.Deserialize<WebTradeEligibility>(web);
                    break;
                }
            }
        }
        return (true, trade_url.IsEmpty() ? null : trade_url, webState);
    }

    public static async Task<(LoginResultv1, SessionDatav1?, long)> Authv1Async(UserLoginv1 user_login, SignInPlatform platform)
    {
        var result = await user_login.DoLoginAsync(platform);
        if (result != LoginResultv1.LoginOkay)
            return (result, null, user_login.CaptchaGID);
        return (result, user_login.Session, -1);
    }
    public static async Task<(LoginResultv1, SessionDatav1?)> AuthV1Async(string username, string password, string fa2_code,
        string email_code, System.Net.IWebProxy Proxy, SignInPlatform platform)
    {
        var user_login = new UserLoginv1(username, password, Proxy);
        if (fa2_code != null)
            user_login.TwoFactorCode = fa2_code;
        else if (email_code != null)
            user_login.EmailCode = email_code;
        var result = await user_login.DoLoginAsync(platform);
        if (result != LoginResultv1.LoginOkay)
            return (result, null);
        return (result, user_login.Session);
    }
    public static (LoginResultv1, SessionDatav1?) AuthV1(string username, string password, string fa2_code, string email_code, System.Net.IWebProxy Proxy, SignInPlatform platform)
    {
        var user_login = new UserLoginv1(username, password, Proxy);
        if (fa2_code != null)
            user_login.TwoFactorCode = fa2_code;
        else if (email_code != null)
            user_login.EmailCode = email_code;
        var result = user_login.DoLogin(platform);
        if (result != LoginResultv1.LoginOkay)
            return (result, null);
        return (result, user_login.Session);
    }
    public static (LoginResultv1, SessionDatav1?, long) AuthV1(string username, string password, long capthagid, string captchatext,
        SteamGuardAccuntv1 sda, System.Net.IWebProxy Proxy, SignInPlatform platform)
    {
        var user_login = new UserLoginv1(username, password, Proxy);
        if (sda != null)
        {
            user_login.Requires2FA = true;
            user_login.TwoFactorCode = sda.GenerateSteamGuardCode();
        }
        if (capthagid == -1)
        {
            user_login.RequiresCaptcha = true;
            user_login.CaptchaGID = capthagid;
            user_login.CaptchaText = captchatext;
        }
        var result = user_login.DoLogin(platform);
        if (result != LoginResultv1.LoginOkay)
            return (result, null, user_login.CaptchaGID);
        return (result, user_login.Session, -1);
    }
    public static (LoginResultv1, SessionDatav1?, long) AuthV1(UserLoginv1 user_login, SignInPlatform platform)
    {
        var result = user_login.DoLogin(platform);
        if (result != LoginResultv1.LoginOkay)
            return (result, null, user_login.CaptchaGID);
        return (result, user_login.Session, -1);
    }

    public static async Task<(LoginResultv2, SessionDatav2?)> AuthV2Async(UserLoginv2 user_login)
    {
        if (user_login.FullyEnrolled)
            return (user_login.Result, user_login.Session);
        if (user_login.NextStep == NEXT_STEP.Begin && !await user_login.BeginAuthSessionViaCredentialsAsync())
            return (user_login.Result, null);
        await Task.Delay(450);
        if (user_login.NextStep == NEXT_STEP.Update && !await user_login.UpdateAuthSessionWithSteamGuardCodeAsync(user_login.Data))
            return (user_login.Result, null);
        if (user_login.NextStep == NEXT_STEP.Poll && !await user_login.PollAuthSessionStatusAsync())
            return (user_login.Result, null);
        return (user_login.Result, user_login.Session);
    }
    public static async Task<(LoginResultv2, SessionDatav2?)> AuthV2Async(string username, string password, string guard_code, System.Net.IWebProxy proxy, EAuthTokenPlatformType platform)
    {
        var user_login = new UserLoginv2(username, password, platform, proxy);
        if (user_login.NextStep == NEXT_STEP.Begin && !await user_login.BeginAuthSessionViaCredentialsAsync())
            return (user_login.Result, null);
        await Task.Delay(450);
        if (user_login.NextStep == NEXT_STEP.Update && !await user_login.UpdateAuthSessionWithSteamGuardCodeAsync(guard_code))
            return (user_login.Result, null);
        if (user_login.NextStep == NEXT_STEP.Poll && !await user_login.PollAuthSessionStatusAsync())
            return (user_login.Result, null);
        return (user_login.Result, user_login.Session);
    }
    public static (LoginResultv2, SessionDatav2?) AuthV2(string username, string password, string guard_code, System.Net.IWebProxy proxy, EAuthTokenPlatformType platform)
    {
        var user_login = new UserLoginv2(username, password, platform, proxy);
        if (user_login.NextStep == NEXT_STEP.Begin && !user_login.BeginAuthSessionViaCredentials())
            return (user_login.Result, null);
        using var mres = new ManualResetEvent(false);
        mres.WaitOne(450);
        if (user_login.NextStep == NEXT_STEP.Update && !user_login.UpdateAuthSessionWithSteamGuardCode(guard_code))
            return (user_login.Result, null);
        if (user_login.NextStep == NEXT_STEP.Poll && !user_login.PollAuthSessionStatus())
            return (user_login.Result, null);
        return (user_login.Result, user_login.Session);
    }
    public static (LoginResultv2, SessionDatav2?) AuthV2(UserLoginv2 user_login)
    {
        if (user_login.FullyEnrolled)
            return (user_login.Result, user_login.Session);
        if (user_login.NextStep == NEXT_STEP.Begin && !user_login.BeginAuthSessionViaCredentials())
            return (user_login.Result, null);
        using var mres = new ManualResetEvent(false);
        mres.WaitOne(450);
        if (user_login.NextStep == NEXT_STEP.Update && !user_login.UpdateAuthSessionWithSteamGuardCode(user_login.Data))
            return (user_login.Result, null);
        if (user_login.NextStep == NEXT_STEP.Poll && !user_login.PollAuthSessionStatus())
            return (user_login.Result, null);
        return (user_login.Result, user_login.Session);
    }

    public static async Task<MemoryStream?> GetCaptchaImageToMemoryStreamAsync(string captchagid, Proxy? proxy = null, ISessionProvider? session = null)
    {
        var (success, bytes, _) = await Downloader.GetCaptchaAsync(captchagid, proxy, session);
        if (!success)
            return null;
        var stream = new MemoryStream(bytes);
        return stream;
    }
    public static async Task<byte[]> GetCaptchaImageToBytesAsync(string captchagid, Proxy? proxy = null, ISessionProvider? session = null)
    {
        var (success, bytes, _) = await Downloader.GetCaptchaAsync(captchagid, proxy, session);
        if (!success)
            return new byte[0];
        return bytes;
    }
    public static MemoryStream? GetCaptchaImageToMemoryStream(string captchagid, Proxy? proxy = null, ISessionProvider? session = null)
    {
        var (success, bytes, _) = Downloader.GetCaptcha(captchagid, proxy, session);
        if (!success)
            return null;
        var stream = new MemoryStream(bytes);
        return stream;
    }
    public static byte[] GetCaptchaImageToBytes(string captchagid, Proxy? proxy = null, ISessionProvider? session = null)
    {
        var (success, bytes, _) = Downloader.GetCaptcha(captchagid, proxy, session);
        if (!success)
            return new byte[0];
        return bytes;
    }
    
    /// <summary>
    /// Получает страницу предмета в steam
    /// </summary>
    /// <param name="appID">app id приложения, чей предмет парсим</param>
    /// <param name="market_hash_name">название предмет, который нам нужен</param>
    /// <returns>Полученные данные</returns>
    public static async Task<MarketItem> GetMarketItemAsync(ISessionProvider? session, System.Net.IWebProxy? proxy, uint appID, string market_hash_name)
    {
        if (string.IsNullOrEmpty(market_hash_name))
            return new MarketItem() { IsError = true, Data = "Не указан market_hash_name" };
        market_hash_name = Uri.EscapeDataString(market_hash_name).Replace("%27", "'").Replace("?", "%3F").Replace("%E2%98%85", "★").Replace("%E2%84%A2", "™");
        string url = $"{SteamCommunityUrls.Market_Listings}/{appID}/{market_hash_name}";
        var response = await Downloader.GetAsync(new(url, proxy, session));
        if (response.Data.IsEmpty())
            return new MarketItem() { IsError = true, Data = "data empty" };
        else if (response.StatusCode == 429)
            return new MarketItem() { IsError = true, IsTooManyRequests = true, Data = "TooManyRequests (429)" };
        else if (response.Data == "<!DOCTYPE html>")
            return new MarketItem() { IsError = true, Data = "bad data" };
        else if (!response.Success)
            return new MarketItem() { IsError = true, Data = response.ErrorMessage! };
        else if (response.Data!.Contains("There are no listings for this item.") && !response.Data.Contains("market_listing_largeimage"))
            return new MarketItem() { IsZeroItemsListed = true };

        return MarketItem.Deserialize(response.Data);
    }
    /// <summary>
    /// Получает страницу предмета в steam
    /// </summary>
    /// <param name="appID">app id приложения, чей предмет парсим</param>
    /// <param name="market_hash_name">название предмет, который нам нужен</param>
    /// <returns>Полученные данные</returns>
    public static MarketItem GetMarketItem(ISessionProvider? session, System.Net.IWebProxy? proxy, uint appID, string market_hash_name)
    {
        if (string.IsNullOrEmpty(market_hash_name))
            return new MarketItem() { IsError = true, Data = "Не указан market_hash_name" };
        market_hash_name = Uri.EscapeDataString(market_hash_name).Replace("%27", "'").Replace("?", "%3F").Replace("%E2%98%85", "★").Replace("%E2%84%A2", "™");
        string url = $"{SteamCommunityUrls.Market_Listings}/{appID}/{market_hash_name}";
        var response = Downloader.Get(new(url, proxy, session));
        if (response.Data.IsEmpty())
            return new MarketItem() { IsError = true, Data = "data empty" };
        else if (response.StatusCode == 429)
            return new MarketItem() { IsError = true, IsTooManyRequests = true, Data = "TooManyRequests (429)" };
        else if (response.Data == "<!DOCTYPE html>")
            return new MarketItem() { IsError = true, Data = "bad data" };
        else if (!response.Success)
            return new MarketItem() { IsError = true, Data = response.ErrorMessage! };
        else if (response.Data!.Contains("There are no listings for this item.") && !response.Data.Contains("market_listing_largeimage"))
            return new MarketItem() { IsZeroItemsListed = true };

        return MarketItem.Deserialize(response.Data);
    }

    /// <summary>
    /// Получает доступные инвентари аккаунта
    /// </summary>
    /// <returns>Коллекция доступных инвентарей, где Key=app_id</returns>
    public static Dictionary<string, AppContextData> GetAppContextData(ISessionProvider session, System.Net.IWebProxy proxy) => GetAppContextData(session, proxy, session.SteamID);
    /// <summary>
    /// Получает доступные инвентари аккаунта
    /// </summary>
    /// <returns>Коллекция доступных инвентарей, где Key=app_id</returns>
    public static Dictionary<string, AppContextData> GetAppContextData(ISessionProvider session, System.Net.IWebProxy proxy, ulong steamid64)
    {
        string url = $"https://steamcommunity.com/profiles/{steamid64}/inventory/";
        string referer = $"https://steamcommunity.com/profiles/{steamid64}/";
        var response = Downloader.Get(new(url, proxy, session, referer));
        if (!response.Success || response.Data.IsEmpty())
            return new(1);
        return AppContextData.Deserialize(response.Data);
    }
    /// <summary>
    /// Получает доступные инвентари аккаунта
    /// </summary>
    /// <returns>Коллекция доступных инвентарей, где Key=app_id</returns>
    public static async Task<Dictionary<string, AppContextData>> GetAppContextDataAsync(ISessionProvider session, System.Net.IWebProxy proxy) =>
        await GetAppContextDataAsync(session, proxy, session.SteamID);
    /// <summary>
    /// Получает доступные инвентари аккаунта
    /// </summary>
    /// <returns>Коллекция доступных инвентарей, где Key=app_id</returns>
    public static async Task<Dictionary<string, AppContextData>> GetAppContextDataAsync(ISessionProvider session, System.Net.IWebProxy proxy, ulong steamid64)
    {
        string url = $"https://steamcommunity.com/profiles/{steamid64}/inventory/";
        string referer = $"https://steamcommunity.com/profiles/{steamid64}/";
        var response = await Downloader.GetAsync(new(url, proxy, session, referer));
        if (!response.Success || response.Data.IsEmpty())
            return new(1);
        return AppContextData.Deserialize(response.Data);
    }

    /// <summary>
    /// Принимает трейд
    /// </summary>
    /// <param name="tradeofferid">id трейда для принятия</param>
    /// <param name="steamid_other">steamid32 аккаунта, который отослал трейд</param>
    /// <returns>False трейд не принят, но иногда может быть принят, для точности используйте API</returns>
    public static (ConfTradeOffer?, SteamTradeError?) AcceptTrade(ISessionProvider session, System.Net.IWebProxy proxy, long tradeofferid, uint steamid_other)
        => AcceptTrade(session, proxy, tradeofferid, Steam32ToSteam64(steamid_other));
    /// <summary>
    /// Принимает трейд
    /// </summary>
    /// <param name="tradeofferid">id трейда для принятия</param>
    /// <param name="steamid_other">steamid32 аккаунта, который отослал трейд</param>
    /// <returns>False трейд не принят, но иногда может быть принят, для точности используйте API</returns>
    public static (ConfTradeOffer?, SteamTradeError?) AcceptTrade(ISessionProvider session, System.Net.IWebProxy proxy, long tradeofferid, ulong steamid64)
    {
        var request = new PostRequest($"https://steamcommunity.com/tradeoffer/{tradeofferid}/accept", Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            IsAjax = true,
            Referer = $"https://steamcommunity.com/tradeoffer/{tradeofferid}/"
        }
        .AddPostData("sessionid", session.SessionID).AddPostData("serverid", 1).AddPostData("tradeofferid", tradeofferid)
        .AddPostData("partner", steamid64).AddPostData("captcha", "");
        var response = Downloader.Post(request);
        try
        {
            if (!response.Success)
            {
                var steamerror = SteamTradeError.Deserialize(response.Data);
                return (null, steamerror);
            }
            try
			{
				var options = new JsonSerializerOptions
				{
					NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
				};
				var conf = JsonSerializer.Deserialize<ConfTradeOffer>(response.Data!, options);
                return (conf, null);
            }
            catch (Exception ex)
            {
                return (null, new() { strError = ex.Message });
            }
        }
        catch (Exception ex)
        {
            return (null, new() { strError = ex.Message });
        }
    }
    public static (ConfTradeOffer?, SteamTradeError?) AcceptTrade(ISessionProvider session, System.Net.IWebProxy proxy, Trade trade) =>
        AcceptTrade(session, proxy, (long)trade.u_tradeofferid, trade.accountid_other);
    /// <summary>
    /// Принимает трейд
    /// </summary>
    /// <param name="tradeofferid">id трейда для принятия</param>
    /// <param name="steamid_other">steamid32 аккаунта, который отослал трейд</param>
    /// <returns>False трейд не принят, но иногда может быть принят, для точности используйте API</returns>
    public static async Task<(ConfTradeOffer?, SteamTradeError?)> AcceptTradeAsync(ISessionProvider session, System.Net.IWebProxy proxy, long tradeofferid, uint steamid_other)
        => await AcceptTradeAsync(session, proxy, tradeofferid, Steam32ToSteam64(steamid_other));
    /// <summary>
    /// Принимает трейд
    /// </summary>
    /// <param name="tradeofferid">id трейда для принятия</param>
    /// <param name="steamid_other">steamid32 аккаунта, который отослал трейд</param>
    /// <returns>False трейд не принят, но иногда может быть принят, для точности используйте API</returns>
    public static async Task<(ConfTradeOffer?, SteamTradeError?)> AcceptTradeAsync(ISessionProvider session, System.Net.IWebProxy proxy, long tradeofferid, ulong steamid64)
    {
        var request = new PostRequest($"https://steamcommunity.com/tradeoffer/{tradeofferid}/accept", Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            IsAjax = true,
            Referer = $"https://steamcommunity.com/tradeoffer/{tradeofferid}/"
        }
        .AddPostData("sessionid", session.SessionID).AddPostData("serverid", 1).AddPostData("tradeofferid", tradeofferid)
        .AddPostData("partner", steamid64).AddPostData("captcha", "");
        var response = await Downloader.PostAsync(request);
        try
        {
            if (!response.Success)
            {
                var steamerror = SteamTradeError.Deserialize(response.Data);
                return (null, steamerror);
            }
            try
			{
				var options = new JsonSerializerOptions
				{
					NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
				};
				var conf = JsonSerializer.Deserialize<ConfTradeOffer>(response.Data!, options);
                return (conf, null);
            }
            catch (Exception ex)
            {
                return (null, new() { strError = ex.Message });
            }
        }
        catch (Exception ex)
        {
            return (null, new() { strError = ex.Message });
        }
    }

    public static async Task<InvormationProfileResponse> SetAccountInfoAsync(ISessionProvider session, System.Net.IWebProxy proxy, InformationProfileRequest info)
    {
        string url = $"https://steamcommunity.com/profiles/{info.steamID}/edit/";
        var request = new PostRequest(url, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            Referer = $"https://steamcommunity.com/profiles/{info.steamID}/edit/info",
        };
        request.PostData.AddRange(info.GetPostData());
        var response = await Downloader.PostAsync(request);
        if (response.Data?.Contains("<div class=\"profile_fatalerror_message\">") == true)
            return new() { errmsg = "КТ" };
        if (!response.Success) return new() { errmsg = "Не удалось отправить запрос" };
        try
        {
            var obj = JsonSerializer.Deserialize<InvormationProfileResponse>(response.Data);
            return obj;
        }
        catch (Exception)
        {
            return new InvormationProfileResponse()
            {
                success = 0,
                errmsg = $"Ошибка при десерилизации данных '{response.Data}'"
            };
        }
    }
    public static async Task<UploadImageResponse> UploadAvatarAsync(ISessionProvider session, System.Net.IWebProxy proxy, string filename)
    {
        var request = new PostRequest(SteamCommunityUrls.Actions_FileUploader, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            Referer = $"https://steamcommunity.com/profiles/{session.SteamID}/edit/avatar",
        }
        .AddPostData("type", "player_avatar_image").AddPostData("sId", session.SteamID).AddPostData("sessionid", session.SessionID)
        .AddPostData("doSub", 1).AddPostData("json", 1);
        var response = await Downloader.UploadFilesToRemoteUrlAsync(request, filename);
        if (!response.Success)
            return new() { success = false, message = response.Data };
        try
        {
            var obj = JsonSerializer.Deserialize<UploadImageResponse>(response.Data);
            return obj;
        }
        catch (Exception)
        {
            return new UploadImageResponse() { success = false, message = $"Ошибка при десерилизации данных '{response.Data}'" };
        }
    }
    public static async Task<OperationProgress> CSGO_OperationProgressAsync(ISessionProvider session, System.Net.IWebProxy proxy)
    {
        string url = $"https://steamcommunity.com/profiles/{session.SteamID}/gcpd/730/?tab=operationquests";
        var response = await Downloader.GetAsync(new GetRequest(url, proxy, session));
        if (!response.Success)
            return new OperationProgress("Ошибка при запросе");
        else if (response.Data == "<!DOCTYPE html>")
            return new OperationProgress("Бан на запросы");
        HtmlParser html = new HtmlParser();
        var parser = await html.ParseDocumentAsync(response.Data);
        var personaldata_elements_container = parser.GetElementById("personaldata_elements_container");
        if (personaldata_elements_container == null)
        {
            var mainContent = parser.GetElementById("mainContents");
            if (mainContent != null)
            {
                var divs = mainContent.GetElementsByTagName("div");
                foreach (var item in divs)
                {
                    if (item.ClassName == null && item.Id == null && item.Children.Length == 0 && !string.IsNullOrEmpty(item.TextContent))
                    {
                        return new OperationProgress(item.TextContent.Replace("\n", "").Replace("\r", "").Replace("\t", ""));
                    }
                }
            }
            return new OperationProgress("Неудалось найти personaldata_elements_container");
        }
        var generic_kv_table = personaldata_elements_container.GetElementsByClassName("generic_kv_table");
        if (generic_kv_table.Length == 0)
            return new OperationProgress("Неудалось найти generic_kv_table");
        var tr = generic_kv_table[0].GetElementsByTagName("tr");
        if (tr.Length == 0)
            return new OperationProgress("Неудалось найти tr");
        if (tr.Length == 1)
            return new OperationProgress("Нет операций");
        var op_progress = new OperationProgress();
        op_progress.Success = true;
        op_progress.Operations = new Operation[tr.Length - 1];
        for (int i = 1; i < tr.Length; i++)
        {
            var td = tr[i].GetElementsByTagName("td");
            if (td.Length == 0)
            {
                op_progress.Error = "Неудалось найти td";
                continue;
            }
            if (td.Length < 6)
            {
                op_progress.Error = "Таблица td изменилась";
                continue;
            }

            var op = new Operation();
            op.Name = td[0].TextContent.GetClearWebString();

            var ms_comp = td[1].TextContent.GetClearWebString().GetOnlyDigit();
            op.Missions_Completed = ms_comp.IsEmpty() ? 0 : int.Parse(ms_comp);

            var st_prog = td[2].TextContent.GetClearWebString().GetOnlyDigit();
            op.Stars_Progress = st_prog.IsEmpty() ? 0 : int.Parse(st_prog);

            var st_pur = td[3].TextContent.GetClearWebString().GetOnlyDigit();
            op.Stars_Purchased = st_pur.IsEmpty() ? 0 : int.Parse(st_pur);

            var act_miss = td[4].TextContent.GetClearWebString().GetOnlyDigit();
            op.Active_Mission_Card = act_miss.IsEmpty() ? 0 : int.Parse(act_miss);

            var redeemable_bal = td[5].TextContent.GetClearWebString().GetOnlyDigit();
            op.Redeemable_Balance = redeemable_bal.IsEmpty() ? 0 : int.Parse(st_pur);

            var act_time = td[6].TextContent.GetClearWebString();
            op.Activation_Time = act_time.IsEmpty() ? "None" : act_time;

            op_progress.Operations[^1] = op;
        }
        return op_progress;
    }
    public static async Task<AccountMain> CSGO_AccountMainAsync(ISessionProvider session, System.Net.IWebProxy proxy)
    {
        string url = $"https://steamcommunity.com/profiles/{session.SteamID}/gcpd/730/?tab=accountmain";
        var response = await Downloader.GetAsync(new(url, proxy, session));
        if (!response.Success)
            return new("Ошибка при запросе");
        else if (response.Data == "<!DOCTYPE html>")
            return new("Бан на запросы");
        HtmlParser html = new HtmlParser();
        var parser = await html.ParseDocumentAsync(response.Data);
        var personaldata_container = parser.GetElementById("personaldata_elements_container");
        if (personaldata_container == null)
        {
            var mainContent = parser.GetElementById("mainContents");
            if (mainContent != null)
            {
                var divs = mainContent.GetElementsByTagName("div");
                foreach (var item in divs)
                {
                    if (item.ClassName == null && item.Id == null && item.Children.Length == 0 && !string.IsNullOrEmpty(item.TextContent))
                    {
                        return new(item.TextContent.Replace("\n", "").Replace("\r", "").Replace("\t", ""));
                    }
                }
            }
            return new("Неудалось найти personaldata_elements_container");
        }
        var generic_kv_tables = personaldata_container.GetElementsByClassName("generic_kv_table");
        if (generic_kv_tables.Length == 0)
            return new("Неудалось найти generic_kv_table");

        var accountMain = new AccountMain();
        for (int i = 0; i < generic_kv_tables.Length; i++)
        {
            var tbody = generic_kv_tables[i].GetElementsByTagName("tbody");
            if (tbody.Length == 0)
                continue;
            var tr = tbody[0].GetElementsByTagName("tr");
            if (tr.Length == 1)
            {
                var tds = tr[0].GetElementsByTagName("td");
                if (tds.Length == 0)
                    continue;
                var generic_kv_lines = tds[0].GetElementsByClassName("generic_kv_line");
                if (generic_kv_lines.Length == 0)
                    continue;
                var dict_mini = new Dictionary<string, string>();
                for (int x = 0; x < generic_kv_lines.Length; x++)
                {
                    var generic_kv_line = generic_kv_lines[x];
                    var splitted = generic_kv_line.TextContent.GetClearWebString().Replace(": ", "|").Split('|'); //.Replace("CS:GO", "CSGO")
                    if (splitted.Length == 1)
                        dict_mini.Add(splitted[0], "none");
                    else dict_mini.Add(splitted[0], splitted[1]);
                }
                if (dict_mini.ContainsKey("Last known IP address"))
                    accountMain.LastKnownIPAddress = dict_mini["Last known IP address"];
                if (dict_mini.ContainsKey("Earned a Service Medal"))
                    accountMain.EarnedAServiceMedal = dict_mini["Earned a Service Medal"];
                if (dict_mini.ContainsKey("CS:GO Profile Rank"))
                {
                    var str = dict_mini["CS:GO Profile Rank"].GetOnlyDigit();
                    if (str.IsEmpty())
                        str = "0";
                    accountMain.CSGOProfileRank = str.ParseUInt16();
                }
                if (dict_mini.ContainsKey("Experience points earned towards next rank"))
                {
                    var str = dict_mini["Experience points earned towards next rank"].GetOnlyDigit();
                    if (str.IsEmpty())
                        str = "0";
                    accountMain.ExperiencePointsEarnedTowardsNextRank = str.ParseUInt16();
                }
                if (dict_mini.ContainsKey("Anti-addiction online time"))
                    accountMain.AntiAddictionOnlineTime = dict_mini["Anti-addiction online time"];
            }
            if (tr.Length < 2)
                continue;
            var th = tr[0].GetElementsByTagName("th");
            var td = tr[1].GetElementsByTagName("td");
            if (th.Length == 0 || td.Length == 0)
                continue;

            var header1 = th[0].TextContent.GetClearWebString();
            if (th.Length >= 2 && header1 == "CS:GO Subscription Service Active Until" &&
                th[1].TextContent.GetClearWebString() == "Subscription Initiation Time" && td.Length >= 2)
            {
                accountMain.SubscriptionService.SubscriptionInitiationTime = td[1].TextContent.GetClearWebString();
            }
            if (th.Length >= 1 && header1 == "Prime Account Status Active Since" && td.Length >= 1)
            {
                accountMain.PrimeAccountStatusActiveSince = td[0].TextContent.GetClearWebString();
            }
            if ((tr.Length == 5 || tr.Length == 6) && header1 == "Recorded Activity")
            {
                var dict = new Dictionary<string, string>(10);
                var name0 = tr[1].GetElementsByTagName("td")[0].TextContent.GetClearWebString();
                var value0 = tr[1].GetElementsByTagName("td")[1].TextContent.GetClearWebString();
                dict.Add(name0, value0);

                var name1 = tr[2].GetElementsByTagName("td")[0].TextContent.GetClearWebString();
                var value1 = tr[2].GetElementsByTagName("td")[1].TextContent.GetClearWebString();
                dict.Add(name1, value1);

                var name2 = tr[3].GetElementsByTagName("td")[0].TextContent.GetClearWebString();
                var value2 = tr[3].GetElementsByTagName("td")[1].TextContent.GetClearWebString();
                dict.Add(name2, value2);

                var name3 = tr[4].GetElementsByTagName("td")[0].TextContent.GetClearWebString();
                var value3 = tr[4].GetElementsByTagName("td")[1].TextContent.GetClearWebString();
                dict.Add(name3, value3);

                if (tr.Length == 6)
                {
                    var name4 = tr[5].GetElementsByTagName("td")[0].TextContent.GetClearWebString();
                    var value4 = tr[5].GetElementsByTagName("td")[1].TextContent.GetClearWebString();
                    dict.Add(name4, value4);
                }
                if (dict.ContainsKey("Logged out of CS:GO"))
                    accountMain.LoggedOutOfCSGO = dict["Logged out of CS:GO"];
                if (dict.ContainsKey("Launched CS:GO using Steam Client"))
                    accountMain.LaunchedCSGOUsingSteamClient = dict["Launched CS:GO using Steam Client"];
                if (dict.ContainsKey("Started playing CS:GO"))
                    accountMain.StartedPlayingCSGO = dict["Started playing CS:GO"];
                if (dict.ContainsKey("First Counter-Strike franchise game"))
                    accountMain.FirstCounterStrikeFranchiseGame = dict["First Counter-Strike franchise game"];
                if (dict.ContainsKey("Launched CS:GO using Perfect World CS:GO Launcher"))
                    accountMain.LaunchedCSGOUsingPerfectWorldCSGOLauncher = dict["Launched CS:GO using Perfect World CS:GO Launcher"];
            }
        }
        return accountMain;
    }
    public static async Task<Matchmaking> CSGO_MatchmakingAsync(ISessionProvider session, System.Net.IWebProxy proxy)
    {
        string url = $"https://steamcommunity.com/profiles/{session.SteamID}/gcpd/730/?tab=matchmaking";
        var response = await Downloader.GetAsync(new(url, proxy, session));
        if (!response.Success)
            return new("Ошибка при запросе");
        else if (response.Data == "<!DOCTYPE html>")
            return new("Бан на запросы");
        HtmlParser html = new HtmlParser();
        var parser = await html.ParseDocumentAsync(response.Data);
        var personaldata_container = parser.GetElementById("personaldata_elements_container");
        if (personaldata_container == null)
        {
            var mainContent = parser.GetElementById("mainContents");
            if (mainContent != null)
            {
                var divs = mainContent.GetElementsByTagName("div");
                foreach (var item in divs)
                {
                    if (item.ClassName == null && item.Id == null && item.Children.Length == 0 && !string.IsNullOrEmpty(item.TextContent))
                    {
                        return new(item.TextContent.Replace("\n", "").Replace("\r", "").Replace("\t", ""));
                    }
                }
            }
            return new("Неудалось найти personaldata_elements_container");
        }
        var generic_kv_tables = personaldata_container.GetElementsByClassName("generic_kv_table");
        if (generic_kv_tables.Length == 0)
            return new("Неудалось найти generic_kv_table");

        var mm = new Matchmaking();
        for (int i = 0; i < generic_kv_tables.Length; i++)
        {
            var tbody = generic_kv_tables[i].GetElementsByTagName("tbody");
            if (tbody.Length == 0)
                continue;
            var tr = tbody[0].GetElementsByTagName("tr");
            if (tr.Length < 2)
                continue;
            var th = tr[0].GetElementsByTagName("th");
            var td = tr[1].GetElementsByTagName("td");
            if (th.Length == 0 || td.Length == 0)
                continue;

            var header1 = th[0].TextContent.GetClearWebString();
            if (header1 == "Matchmaking Mode" && tr.Length > 0)
            {
                var ths = tr[0].GetElementsByTagName("th");
                var tr10_header = tr[1].GetElementsByTagName("td")[0].TextContent.GetClearWebString();

                if (ths.Length == 6 && tr.Length >= 2 && tr10_header == "Competitive")
                {
                    var td_tmp = tr[1].GetElementsByTagName("td");
                    var wins = td_tmp[1].TextContent.GetClearWebString().GetOnlyDigit();
                    if (wins.IsEmpty())
                        wins = "0";
                    mm.Competitive.Wins = wins.ParseUInt16();

                    var ties = td_tmp[2].TextContent.GetClearWebString().GetOnlyDigit();
                    if (ties.IsEmpty())
                        ties = "0";
                    mm.Competitive.Ties = ties.ParseUInt16();

                    var losses = td_tmp[3].TextContent.GetClearWebString().GetOnlyDigit();
                    if (losses.IsEmpty())
                        losses = "0";
                    mm.Competitive.Losses = losses.ParseUInt16();

                    var rank = td_tmp[4].TextContent.GetClearWebString().GetOnlyDigit();
                    if (rank.IsEmpty())
                        rank = "0";
                    mm.Competitive.SkillGroup = rank.ParseInt16();

                    var last_match = td_tmp[5].TextContent.GetClearWebString();
                    mm.Competitive.LastMatch = last_match;
                }
                if (ths.Length == 6 && tr.Length >= 2 && tr10_header == "Wingman")
                {
                    var td_tmp = tr[1].GetElementsByTagName("td");
                    var wins = td_tmp[1].TextContent.GetClearWebString().GetOnlyDigit();
                    if (wins.IsEmpty())
                        wins = "0";
                    mm.Wingman.Wins = wins.ParseUInt16();

                    var ties = td_tmp[2].TextContent.GetClearWebString().GetOnlyDigit();
                    if (ties.IsEmpty())
                        ties = "0";
                    mm.Wingman.Ties = ties.ParseUInt16();

                    var losses = td_tmp[3].TextContent.GetClearWebString().GetOnlyDigit();
                    if (losses.IsEmpty())
                        losses = "0";
                    mm.Wingman.Losses = losses.ParseUInt16();

                    var rank = td_tmp[4].TextContent.GetClearWebString().GetOnlyDigit();
                    if (rank.IsEmpty())
                        rank = "0";
                    mm.Wingman.SkillGroup = rank.ParseInt16();

                    var last_match = td_tmp[5].TextContent.GetClearWebString();
                    mm.Wingman.LastMatch = last_match;
                }
                if (ths.Length == 6 && tr.Length >= 3 && tr[2].GetElementsByTagName("td")[0].TextContent.GetClearWebString() == "Wingman")
                {
                    var td_tmp = tr[2].GetElementsByTagName("td");
                    var wins = td_tmp[1].TextContent.GetClearWebString().GetOnlyDigit();
                    if (wins.IsEmpty())
                        wins = "0";
                    mm.Wingman.Wins = wins.ParseUInt16();

                    var ties = td_tmp[2].TextContent.GetClearWebString().GetOnlyDigit();
                    if (ties.IsEmpty())
                        ties = "0";
                    mm.Wingman.Ties = ties.ParseUInt16();

                    var losses = td_tmp[3].TextContent.GetClearWebString().GetOnlyDigit();
                    if (losses.IsEmpty())
                        losses = "0";
                    mm.Wingman.Losses = losses.ParseUInt16();

                    var rank = td_tmp[4].TextContent.GetClearWebString().GetOnlyDigit();
                    if (rank.IsEmpty())
                        rank = "0";
                    mm.Wingman.SkillGroup = rank.ParseInt16();

                    var last_match = td_tmp[5].TextContent.GetClearWebString();
                    mm.Wingman.LastMatch = last_match;
                }
                if (ths.Length == 5 && tr.Length >= 2 && tr10_header == "Danger Zone")
                {
                    var td_tmp = tr[1].GetElementsByTagName("td");
                    var solo = td_tmp[1].TextContent.GetClearWebString().GetOnlyDigit();
                    if (solo.IsEmpty())
                        solo = "0";
                    mm.DangerZone.SoloWins = solo.ParseUInt16();

                    var squad = td_tmp[2].TextContent.GetClearWebString().GetOnlyDigit();
                    if (squad.IsEmpty())
                        squad = "0";
                    mm.DangerZone.SquadWins = squad.ParseUInt16();

                    var matches = td_tmp[3].TextContent.GetClearWebString().GetOnlyDigit();
                    if (matches.IsEmpty())
                        matches = "0";
                    mm.DangerZone.MatchesPlayed = matches.ParseUInt16();

                    var last_match = td_tmp[4].TextContent.GetClearWebString();
                    mm.DangerZone.LastMatch = last_match;
                }

                var flag0 = ths.Length > 0 && ths[1].TextContent.GetClearWebString() == "Last Match";
                if (ths.Length == 2 && tr.Length >= 2 && flag0)
                {
                    for (int x = 1; x < tr.Length; x++)
                    {
                        var td_tmp = tr[x].GetElementsByTagName("td");
                        var mode = td_tmp[0].TextContent.GetClearWebString();
                        var last_match = td_tmp[1].TextContent.GetClearWebString();

                        var match = new ModeLastMatch(mode, last_match);
                        var arr = new ModeLastMatch[mm.LastMatches.Length + 1];
                        mm.LastMatches.CopyTo(arr, 0);
                        mm.LastMatches = arr;
                        mm.LastMatches[^1] = match;
                    }
                }
            }
        }
        return mm;
    }

    public static ulong Steam32ToSteam64(uint input) => SteamIDConverter + input;
    public static uint Steam64ToSteam32(ulong input) => (uint)(input - SteamIDConverter);
    /// <summary>
    /// </summary>
    /// <param name="communityId">SteamID64(76561197960265728)</param>
    /// <returns>String.empty if error, else the string SteamID2(STEAM_0:1:000000)</returns>
    public static string Steam64ToSteam2(ulong communityId)
    {
        if (communityId < 76561197960265729L || !Regex.IsMatch(communityId.ToString(CultureInfo.InvariantCulture), "^7656119([0-9]{10})$"))
            return string.Empty;
        communityId -= 76561197960265728L;
        ulong num = communityId % 2L;
        communityId -= num;
        string input = string.Format("STEAM_0:{0}:{1}", num, (communityId / 2L));
        if (!Regex.IsMatch(input, "^STEAM_0:[0-1]:([0-9]{1,10})$")) return string.Empty;
        return input;
    }
    /// <summary>
    /// </summary>
    /// <param name="communityId">SteamID64(76561197960265728)</param>
    /// <returns>String.empty if error, else the string SteamID3(U:1:000000)</returns>
    public static string Steam64ToSteam3(ulong communityId)
    {
        if (communityId < 76561197960265729L || !Regex.IsMatch(communityId.ToString(CultureInfo.InvariantCulture), "^7656119([0-9]{10})$"))
            return string.Empty;
        communityId -= 76561197960265728L;
        ulong num = communityId % 2L;
        communityId -= num;
        string input = string.Format("U:{0}:{1}", num, (uint)(communityId - SteamIDConverter));
        if (!Regex.IsMatch(input, "^U:[0-1]:([0-9]{1,10})$")) return string.Empty;
        return input;
    }
    /// <summary>
    /// Разбивает трейд ссылку на steamid32 и token
    /// </summary>
    /// <param name="tradeUrl">Полная трейд ссылка</param>
    /// <returns>true если трейд ссылка разбита</returns>
    public static (bool, uint, string?) SplitTradeURL(string tradeUrl)
    {
        if (!tradeUrl.IsEmpty() && rgxTradeurl1.IsMatch(tradeUrl))
        {
            var query = tradeUrl.Split('?')[1];
            var @params = query.Split('&');
            var partner = @params[0].Split('=')[1].ParseUInt32();
            var token = @params[1].Split('=')[1];
            return (true, partner, token);
        }
        return (false, 0, null);
    }

    /// <summary>
    /// Расчитать цены и комиссии по цене пользователя
    /// </summary>
    /// <param name="receivedAmount"></param>
    /// <param name="publisherFee"></param>
    /// <returns>(steam_fee - стим комса, publisher_fee - комиссия издателя, fees - общая комиссия, amount - сколько в общем должен заплатить пользователь)</returns>
    public static (int, int, int, int) CalculateAmountToSendForDesiredReceivedAmount(int receivedAmount, float publisherFee = 0.10f)
    {
        float wallet_fee_percent = 0.05f;
        float wallet_fee_minimum = 1f;
        int wallet_fee_base = 0;
        int nSteamFee = (int)Math.Floor(Math.Max(receivedAmount * wallet_fee_percent, wallet_fee_minimum + wallet_fee_base));
        int nPublisherFee = (int)Math.Floor((double)publisherFee > 0 ? Math.Max(receivedAmount * publisherFee, 1) : 0);
        int nAmountToSend = receivedAmount + nSteamFee + nPublisherFee;
        return (nSteamFee, nPublisherFee, nSteamFee + nPublisherFee, nAmountToSend);
    }
    /// <summary>
    /// Получить комиссии по указаной цене
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="publisherFee"></param>
    /// <returns>(steam_fee - стим комса, publisher_fee - комиссия издателя, fees - общая комиссия, amount - сколько в общем должен заплатить пользователь)</returns>
    public static (int, int, int, int) CalculateFeeAmount(int amount, float publisherFee = 0.10f)
    {
        float wallet_fee_percent = 0.05f;
        int wallet_fee_base = 0;
        // Since CalculateFeeAmount has a Math.floor, we could be off a cent or two. Let's check:
        var iterations = 0; // shouldn't be needed, but included to be sure nothing unforseen causes us to get stuck
        var nEstimatedAmountOfWalletFundsReceivedByOtherParty = (amount - wallet_fee_base) / (wallet_fee_percent + publisherFee + 1);

        var bEverUndershot = false;
        var fees = CalculateAmountToSendForDesiredReceivedAmount((int)nEstimatedAmountOfWalletFundsReceivedByOtherParty, publisherFee);
        int fees_steam = fees.Item1;
        int fees_publisher = fees.Item2;
        int fees_fees = fees.Item3;
        int fees_amount = fees.Item4;

        while (fees_amount != amount && iterations < 10)
        {
            if (fees_amount > amount)
            {
                if (bEverUndershot)
                {
                    fees = CalculateAmountToSendForDesiredReceivedAmount((int)nEstimatedAmountOfWalletFundsReceivedByOtherParty - 1, publisherFee);
                    fees_steam = fees.Item1;
                    fees_publisher = fees.Item2;
                    fees_fees = fees.Item3;
                    fees_amount = fees.Item4;

                    fees_steam += (amount - fees_amount);
                    fees_fees += (amount - fees_amount);
                    fees_amount = amount;
                    break;
                }
                else
                {
                    nEstimatedAmountOfWalletFundsReceivedByOtherParty--;
                }
            }
            else
            {
                bEverUndershot = true;
                nEstimatedAmountOfWalletFundsReceivedByOtherParty++;
            }

            fees = CalculateAmountToSendForDesiredReceivedAmount((int)nEstimatedAmountOfWalletFundsReceivedByOtherParty, publisherFee);
            fees_steam = fees.Item1;
            fees_publisher = fees.Item2;
            fees_fees = fees.Item3;
            fees_amount = fees.Item4;
            iterations++;
        }

        // fees.amount should equal the passed in amount

        return fees;
    }
}
