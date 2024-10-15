using System.Text.Json;
using SteamWeb.Web;
using SteamWeb.Models;
using SteamWeb.Extensions;
using AngleSharp.Html.Parser;
using System.Globalization;
using SteamWeb.Models.Trade;
using System.Text.RegularExpressions;
using System.Web;
using SteamWeb.Auth.v2.Enums;
using SteamWeb.API.Models.IEconService;
using System.Net;
using System.Text.Json.Serialization;
using System.Collections.Immutable;

using LoginResultv2 = SteamWeb.Auth.v2.Enums.LoginResult;
using SessionDatav2 = SteamWeb.Auth.v2.Models.SessionData;
using UserLoginv2 = SteamWeb.Auth.v2.UserLogin;
using SteamGuardAccuntv2 = SteamWeb.Auth.v2.SteamGuardAccount;
using SteamWeb.Models.PurchaseHistory;

namespace SteamWeb;

/// <summary>
/// Здесь собраны все методы, которые не вызывают голый http метод внутреннего api, а используют парсинг, либо свою реализацию
/// </summary>
public static partial class Steam
{
    /// <summary>
    /// Значение которое отнимается или прибавляется к steamid64\steamid32
    /// </summary>
    public const ulong SteamIDConverter = 76561197960265728;
    private static Regex _rgxTradeurl1 = new(@"^https://steamcommunity.com/tradeoffer/new/[?]partner=(\d{1,12})&token=(\S{4,10})$", RegexOptions.Compiled | RegexOptions.Singleline, TimeSpan.FromSeconds(1));
    internal static JsonSerializerOptions JsonOptions { get; } = new()
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        IgnoreReadOnlyFields = true,
        IgnoreReadOnlyProperties = true,
        IncludeFields = false,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        PropertyNameCaseInsensitive = false,
#if DEBUG
        WriteIndented = true,
#elif RELEASE
        WriteIndented = false,
#endif
    };

    public static async Task<bool> SwitchToMailCodeAsync(DefaultRequest ajaxRequest, SteamGuardAccuntv2? SDA)
    {
        var att_phone = await Script.AjaxHelp.PhoneAjaxAsync(ajaxRequest);
        if (att_phone.has_phone == null)
            return false;
        if (att_phone.has_phone == true && SDA != null)
        {
            SDA.Proxy = ajaxRequest.Proxy;
            var result = await SDA.RemoveAuthenticatorAsync(false);
            return result.success;
        }

        var request = new PostRequest(SteamPoweredUrls.TwoFactor_ManageAction, Downloader.AppFormUrlEncoded)
        {
            Proxy = ajaxRequest.Proxy,
            Session = ajaxRequest.Session,
            CancellationToken = ajaxRequest.CancellationToken,
        }
            .AddPostData("action", "email").AddPostData("sessionid", ajaxRequest.Session!.SessionID).AddPostData("email_authenticator_check", "on");
        var response = await Downloader.PostAsync(request);
        return response.Success;
    }
    public static bool SwitchToMailCode(DefaultRequest ajaxRequest, SteamGuardAccuntv2 SDA)
    {
        var att_phone = Script.AjaxHelp.PhoneAjax(ajaxRequest);
        if (att_phone.has_phone == null)
            return false;
        if (att_phone.has_phone == true && SDA != null)
        {
            SDA.Proxy = ajaxRequest.Proxy;
            var result = SDA.RemoveAuthenticator(false);
            return result.success;
        }

        var request = new PostRequest(SteamPoweredUrls.TwoFactor_ManageAction, Downloader.AppFormUrlEncoded)
        {
            Proxy = ajaxRequest.Proxy,
            Session = ajaxRequest.Session,
            CancellationToken = ajaxRequest.CancellationToken,
        }
            .AddPostData("action", "email").AddPostData("sessionid", ajaxRequest.Session!.SessionID).AddPostData("email_authenticator_check", "on");
        var response = Downloader.Post(request);
        return response.Success;
    }
    public static async Task<(bool, string)> SwitchToNonGuardAsync(DefaultRequest ajaxRequest)
    {
        var response = await Downloader.GetAsync(new(SteamPoweredUrls.TwoFactor_ManageAction, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken
        });
        if (!response.Success)
            return (false, response.ErrorMessage!);

        var request = new PostRequest(SteamPoweredUrls.TwoFactor_ManageAction, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            CancellationToken = ajaxRequest.CancellationToken,
        }
        .AddPostData("action", "none").AddPostData("sessionid", ajaxRequest.Session!.SessionID).AddPostData("none_authenticator_check", "on");
        response = await Downloader.PostAsync(request);
        if (!response.Success)
            return (false, response.ErrorMessage!);

        request.PostData.Clear();
        request.AddPostData("action", "actuallynone").AddPostData("sessionid", ajaxRequest.Session!.SessionID);
        response = await Downloader.PostAsync(request);
        if (!response.Success)
            return (false, response.ErrorMessage!);

        var doc = new HtmlParser().ParseDocument(response.Data!);
        var elements = doc.GetElementsByClassName("phone_box");
        if (elements.Length > 0)
        {
            string text = elements[0].TextContent.GetClearWebString()!;
            return (true, text);
        }
        return (false, response.Data!);
    }
    public static (bool, string) SwitchToNonGuard(DefaultRequest ajaxRequest)
    {
        var response = Downloader.Get(new(SteamPoweredUrls.TwoFactor_ManageAction, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken
        });
        if (!response.Success)
            return (false, response.ErrorMessage!);

        var request = new PostRequest(SteamPoweredUrls.TwoFactor_ManageAction, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            CancellationToken = ajaxRequest.CancellationToken,
        }
        .AddPostData("action", "none").AddPostData("sessionid", ajaxRequest.Session!.SessionID).AddPostData("none_authenticator_check", "on");
        response = Downloader.Post(request);
        if (!response.Success)
            return (false, response.ErrorMessage!);

        request.PostData.Clear();
        request.AddPostData("action", "actuallynone").AddPostData("sessionid", ajaxRequest.Session!.SessionID);
        response = Downloader.Post(request);
        if (!response.Success)
            return (false, response.ErrorMessage!);

        var doc = new HtmlParser().ParseDocument(response.Data!);
        var elements = doc.GetElementsByClassName("phone_box");
        if (elements.Length > 0)
        {
            string text = elements[0].TextContent.GetClearWebString()!;
            return (true, text);
        }
        return (false, response.Data!);
    }

    /// <summary>
    /// Проверяет имеется ли community ban на аккаунте
    /// </summary>
    /// <param name="session"></param>
    /// <param name="proxy"></param>
    /// <returns>True имеется</returns>
    public static async Task<bool> CheckOnKTAsync(DefaultRequest ajaxRequest)
    {
        var response = await Downloader.GetAsync(new(SteamCommunityUrls.My_Edit_Info, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken
        });
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
    public static bool CheckOnKT(DefaultRequest ajaxRequest)
    {
        var response = Downloader.Get(new(SteamCommunityUrls.My_Edit_Info, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken
        });
        if (response.Data?.Contains("profile_fatalerror") == true &&
            response.Data?.Contains("profile_fatalerror_message") == true)
            return true;
        return false;
    }

    /// <summary>
    /// Создаёт трейд
    /// </summary>
    /// <returns>ConfTradeOffer != null если трейд создался и SteamTradeError != null если трейд не создался</returns>
    public static (ConfTradeOffer?, SteamTradeError?) CreateTrade(DefaultRequest ajaxRequest, NewTradeOffer trade, Token token, string tradeoffermessage, uint offerpartner)
    {
        var request = new PostRequest(SteamCommunityUrls.TradeOffer_New_Send, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            CancellationToken = ajaxRequest.CancellationToken,
            IsAjax = true,
            Referer = SteamCommunityUrls.TradeOffer_New + "/?partner=" + offerpartner
        }
        .AddPostData("sessionid", ajaxRequest.Session!.SessionID).AddPostData("serverid", 1).AddPostData("partner", Steam32ToSteam64(offerpartner))
        .AddPostData("tradeoffermessage", HttpUtility.UrlEncode(tradeoffermessage), false)
        .AddPostData("json_tradeoffer", HttpUtility.UrlEncode(JsonSerializer.Serialize(trade)), false).AddPostData("captcha", string.Empty)
        .AddPostData("trade_offer_create_params", HttpUtility.UrlEncode(JsonSerializer.Serialize(token)), false);
        try
        {
            var response = Downloader.Post(request);
            if (!response.Success)
            {
                var steamerror = SteamTradeError.Deserialize(response.Data!);
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
                return (null, new() { StrError = ex.Message });
            }
        }
        catch (Exception ex)
        {
            return (null, new() { StrError = ex.Message });
        }
    }
    /// <summary>
    /// Создаёт трейд
    /// </summary>
    /// <returns>ConfTradeOffer != null если трейд создался и SteamTradeError != null если трейд не создался</returns>
    public static async Task<(ConfTradeOffer?, SteamTradeError?)> CreateTradeAsync(DefaultRequest ajaxRequest, NewTradeOffer trade, Token token, string tradeoffermessage, uint offerpartner)
    {
        var request = new PostRequest(SteamCommunityUrls.TradeOffer_New_Send, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            CancellationToken = ajaxRequest.CancellationToken,
            IsAjax = true,
            Referer = SteamCommunityUrls.TradeOffer_New + "/?partner=" + offerpartner
        }
        .AddPostData("sessionid", ajaxRequest.Session!.SessionID).AddPostData("serverid", 1).AddPostData("partner", Steam32ToSteam64(offerpartner))
        .AddPostData("tradeoffermessage", HttpUtility.UrlEncode(tradeoffermessage), false)
        .AddPostData("json_tradeoffer", HttpUtility.UrlEncode(JsonSerializer.Serialize(trade)), false).AddPostData("captcha", string.Empty)
        .AddPostData("trade_offer_create_params", HttpUtility.UrlEncode(JsonSerializer.Serialize(token)), false);
        try
        {
            var response = await Downloader.PostAsync(request);
            if (!response.Success)
            {
                var steamerror = SteamTradeError.Deserialize(response.Data!);
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
                return (null, new() { StrError = ex.Message });
            }
        }
        catch (Exception ex)
        {
            return (null, new() { StrError = ex.Message });
        }
    }

    /// <summary>
    /// Изменяет язык на steam аккаунте
    /// </summary>
    /// <param name="language">язык для смены, пример: russian, english</param>
    /// <returns>True язык изменён</returns>
    [Obsolete("Данный метод переехал в Script.Ajax.actions_setlanguage_async")]
    public static async Task<bool> ChangeLanguageAsync(DefaultRequest ajaxRequest, string language)
    {
        var request = new PostRequest(SteamCommunityUrls.Actions_SetLanguage, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            CancellationToken = ajaxRequest.CancellationToken,
        }
        .AddPostData("language", language).AddPostData("sessionid", ajaxRequest.Session!.SessionID);
        var response = await Downloader.PostAsync(request);
        return response.Success;
    }
    /// <summary>
    /// Изменяет язык на steam аккаунте
    /// </summary>
    /// <param name="language">язык для смены, пример: russian, english</param>
    /// <returns>True язык изменён</returns>
    [Obsolete("Данный метод переехал в Script.Ajax.actions_setlanguage")]
    public static bool ChangeLanguage(DefaultRequest ajaxRequest, string language)
    {
        var request = new PostRequest(SteamCommunityUrls.Actions_SetLanguage, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            CancellationToken = ajaxRequest.CancellationToken,
        }
        .AddPostData("language", language).AddPostData("sessionid", ajaxRequest.Session!.SessionID);
        var response = Downloader.Post(request);
        return response.Success;
    }

    /// <summary>
    /// Получает информацию со страницы <see href="https://store.steampowered.com/account/">store.steampowered.com/account</see>
    /// </summary>
    /// <returns>Полученные данные</returns>
    public static async Task<AboutProfile> Get2FAAsync(DefaultRequest ajaxRequest)
    {
        var response = await Downloader.GetAsync(new(SteamPoweredUrls.Account, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken,
        });
        if (!response.Success)
            return new();
        return AboutProfile.Deserialize(response.Data!);
    }
    /// <summary>
    /// Получает информацию со страницы <see href="https://store.steampowered.com/account/">store.steampowered.com/account</see>
    /// </summary>
    /// <returns>Полученные данные</returns>
    public static AboutProfile Get2FA(DefaultRequest ajaxRequest)
    {
        var response = Downloader.Get(new(SteamPoweredUrls.Account, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken,
        });
        if (!response.Success)
            return new();
        return AboutProfile.Deserialize(response.Data!);
    }

    /// <summary>
    /// Парсит Api ключ со страницы api ключа
    /// </summary>
    /// <returns>Спарсенные данные со страницы</returns>
    public static async Task<WebApiKey> GetWebAPIKeyAsync(DefaultRequest ajaxRequest)
    {
		var response = await Downloader.GetAsync(new(SteamCommunityUrls.Dev_APIKey, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken,
        });
		if (!response.Success)
			return new WebApiKey(response.Data!);
		else if (response.Data == "<!DOCTYPE html>")
		{
			var to1 = new WebApiKey("Бан\\блок на запросы.") { RawHtml = response.Data };
			return to1;
		}
		else if (response.Data!.Contains("<p><a style=\"font-size: 16px;\" href=\"https://support.steampowered.com/kb_article.php?ref=3330-IAGK-7663\">"))
		{
			var to1 = new WebApiKey(response.Data.GetBetween("<p>", "</p>")!) { RawHtml = response.Data };
			return to1;
		}
		else if (response.Data.Contains("Access Denied"))
		{
			var to1 = new WebApiKey("Access Denied") { RawHtml = response.Data };
			return to1;
		}

		HtmlParser html = new HtmlParser();
		var parser = await html.ParseDocumentAsync(response.Data!);
		var isNeedRegisterKey = parser.GetElementsByClassName("agree").Length > 0;
		if (isNeedRegisterKey)
			return new("Нужна регистрация api ключа") { RawHtml = response.Data };

		var isSignUp = parser.GetElementsByClassName("login_create_btn").Length > 0;
		if (isSignUp)
			return new("Необходимо войти в аккаунт") { RawHtml = response.Data };

		var children = parser.GetElementById("bodyContents_ex")?.Children;
		if (children == null)
			return new("Неверная страница") { RawHtml = response.Data };
		if (3 > children.Count())
			return new("Проблема в данных страницы:\n" + response.Data) { RawHtml = response.Data };

		var el1 = children[1].InnerHtml.Replace(" ", "");
		var split1 = el1.Split(':');
		if (2 > split1.Length)
			return new("Проблема в данных key:\n" + response.Data) { RawHtml = response.Data };

		var el2 = children[2].InnerHtml.Replace(" ", "");
		var split2 = el2.Split(':');
		if (2 > split2.Length)
			return new("Проблема в данных domain:\n" + response.Data) { RawHtml = response.Data };

		var to = new WebApiKey(split2[1], split1[1]);
		return to;
	}
	/// <summary>
	/// Парсит Api ключ со страницы api ключа
	/// </summary>
	/// <returns>Спарсенные данные со страницы</returns>
	public static WebApiKey GetWebAPIKey(DefaultRequest ajaxRequest)
    {
        var response = Downloader.Get(new(SteamCommunityUrls.Dev_APIKey, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken,
        });
        if (!response.Success)
            return new WebApiKey(response.Data!);
        else if (response.Data == "<!DOCTYPE html>")
        {
            var to1 = new WebApiKey("Бан\\блок на запросы.") { RawHtml = response.Data };
            return to1;
        }
        else if (response.Data!.Contains("<p><a style=\"font-size: 16px;\" href=\"https://support.steampowered.com/kb_article.php?ref=3330-IAGK-7663\">"))
        {
            var to1 = new WebApiKey(response.Data.GetBetween("<p>", "</p>")!) { RawHtml = response.Data };
            return to1;
        }
        else if (response.Data.Contains("Access Denied"))
        {
            var to1 = new WebApiKey("Access Denied") { RawHtml = response.Data };
            return to1;
        }

        HtmlParser html = new HtmlParser();
        var parser = html.ParseDocument(response.Data!);
        var isNeedRegisterKey = parser.GetElementsByClassName("agree").Length > 0;
        if (isNeedRegisterKey)
            return new("Нужна регистрация api ключа") { RawHtml = response.Data };

        var isSignUp = parser.GetElementsByClassName("login_create_btn").Length > 0;
        if (isSignUp)
            return new("Необходимо войти в аккаунт") { RawHtml = response.Data };

        var children = parser.GetElementById("bodyContents_ex")?.Children;
        if (children == null)
            return new("Неверная страница") { RawHtml = response.Data };
        if (3 > children.Count())
            return new("Проблема в данных страницы:\n" + response.Data) { RawHtml = response.Data };

        var el1 = children[1].InnerHtml.Replace(" ", "");
        var split1 = el1.Split(':');
        if (2 > split1.Length)
            return new("Проблема в данных key:\n" + response.Data) { RawHtml = response.Data };

        var el2 = children[2].InnerHtml.Replace(" ", "");
        var split2 = el2.Split(':');
        if (2 > split2.Length)
            return new("Проблема в данных domain:\n" + response.Data) { RawHtml = response.Data };

        var to = new WebApiKey(split2[1], split1[1]);
        return to;
    }

    /// <summary>
    /// Загружает текущее состояние доступа к маркету для аккаунта (webTradeEligibilityState)
    /// </summary>
    /// <returns>Null если нет данных</returns>
    public static WebTradeEligibility? GetWebTradeEligibility(DefaultRequest ajaxRequest)
    {
        var response = Downloader.Get(new(SteamCommunityUrls.Market_EligibilityCheck, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken,
        });
        if (!response.Success || response.CookieContainer == null)
            return null;
        else if ((!response.Data!.Contains("menuitem supernav username persona_name_text_content") &&
            !response.Data.Contains("whiteLink persona_name_text_content")) ||
            response.Data.Contains("see, edit, or remove your Community Market listings."))
            return null;
        var cookies = response.CookieContainer.GetAllCookies();
		foreach (Cookie item in cookies)
        {
            if (item.Name == "webTradeEligibility")
            {
                var web = HttpUtility.UrlDecode(item.Value);
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
    public static async Task<WebTradeEligibility?> GetWebTradeEligibilityAsync(DefaultRequest ajaxRequest)
    {
        var response = await Downloader.GetAsync(new(SteamCommunityUrls.Market_EligibilityCheck, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken,
        });
        if (!response.Success || response.CookieContainer == null)
            return null;
        else if ((!response.Data!.Contains("menuitem supernav username persona_name_text_content") &&
            !response.Data.Contains("whiteLink persona_name_text_content")) ||
            response.Data.Contains("see, edit, or remove your Community Market listings."))
            return null;
		var cookies = response.CookieContainer.GetAllCookies();
		foreach (Cookie item in cookies)
		{
			if (item.Name == "webTradeEligibility")
			{
				var web = HttpUtility.UrlDecode(item.Value);
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
    public static (bool, string?, WebTradeEligibility?) GetTradeURL(DefaultRequest ajaxRequest)
    {
        var response = Downloader.Get(new(SteamCommunityUrls.Market_EligibilityCheck, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken,
        });
        if (!response.Success)
            return (false, null, null);
        else if (response.Data!.Contains("link_forgot_password"))
            return (false, null, null);

        string? trade_url = null;
        HtmlParser html = new HtmlParser();
        var doc = html.ParseDocument(response.Data);
        var el = doc.GetElementById("trade_offer_access_url");
        if (el != null)
            trade_url = ((AngleSharp.Html.Dom.IHtmlInputElement)el).DefaultValue.GetClearWebString();

        WebTradeEligibility? webState = null;
		var cookies = response.CookieContainer?.GetAllCookies();
		if (cookies != null)
		{
			foreach (Cookie item in cookies)
			{
				if (item.Name == "webTradeEligibility")
				{
					var web = HttpUtility.UrlDecode(item.Value);
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
    public static async Task<(bool, string?, WebTradeEligibility?)> GetTradeURLAsync(DefaultRequest ajaxRequest)
    {
        var response = await Downloader.GetAsync(new(SteamCommunityUrls.Market_EligibilityCheck, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken,
        });
        if (!response.Success)
            return (false, null, null);
        else if (response.Data!.Contains("link_forgot_password"))
            return (false, null, null);

        string? trade_url = null;
        HtmlParser html = new HtmlParser();
        var doc = await html.ParseDocumentAsync(response.Data);
        var el = doc.GetElementById("trade_offer_access_url");
        if (el != null)
            trade_url = ((AngleSharp.Html.Dom.IHtmlInputElement)el).DefaultValue.GetClearWebString();

		WebTradeEligibility? webState = null;
		var cookies = response.CookieContainer?.GetAllCookies();
		if (cookies != null)
		{
			foreach (Cookie item in cookies)
			{
				if (item.Name == "webTradeEligibility")
				{
					var web = HttpUtility.UrlDecode(item.Value);
					webState = JsonSerializer.Deserialize<WebTradeEligibility>(web);
					break;
				}
			}
		}
		return (true, trade_url.IsEmpty() ? null : trade_url, webState);
    }

    /// <summary>
    /// Производит парсинг страницы трейда
    /// </summary>
    /// <param name="tradeofferid">Id трейда</param>
    /// <returns>Спарсенные данные со страницы</returns>
    public static TradeOfferData GetTradeData(DefaultRequest ajaxRequest, ulong tradeofferid)
    {
        var response = Downloader.Get(new(SteamCommunityUrls.TradeOffer + tradeofferid, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken,
        });
        if (!response.Success)
            return new() { TradeOfferId = tradeofferid };

        var html = new HtmlParser();
        var doc = html.ParseDocument(response.Data!);
        var errorMsg = doc.GetElementById("error_msg");
        if (errorMsg != null)
            return new() { TradeOfferId = tradeofferid, Error = errorMsg.InnerHtml.GetClearWebString()  };
        var linkForgotten = doc.GetElementsByClassName("login_create_btn");
        if (linkForgotten.Any())
            return new() { TradeOfferId = tradeofferid, Error = "Необходимо войти в аккаунт" };

        var isMarketAllowed = response.Data!.GetBetween("var g_bMarketAllowed = ", ";")!;
        var partnerSID64 = response.Data!.GetBetween("UserThem.SetSteamId( '", "'");
        var partnerProbation = response.Data!.GetBetween("var g_bTradePartnerProbation = ", ";")!;
        var partnerName = response.Data!.GetBetween("var g_strTradePartnerPersonaName = \"", "\";");
        var youSID64 = response.Data!.GetBetween("UserYou.SetSteamId( '", "'");
        var youName = response.Data!.GetBetween("var g_strYourPersonaName = \"", "\";");
        var sessionId = response.Data!.GetBetween("var g_sessionID = \"", "\";");
        var tradeStatus = response.Data!.GetBetween("var g_rgCurrentTradeStatus = ", ";");

        var tradeData = new TradeOfferData
        {
            IsSuccess = true,
            TradeOfferId = tradeofferid,
        };
        tradeData.SetTradeStatus(tradeStatus);
        tradeData.SetYou(youName, sessionId, isMarketAllowed, youSID64);
        tradeData.SetPartner(partnerName, partnerProbation, partnerSID64);
        return tradeData;
	}
	/// <summary>
	/// Производит парсинг страницы трейда
	/// </summary>
	/// <param name="tradeofferid">Id трейда</param>
	/// <returns>Спарсенные данные со страницы</returns>
	public static async Task<TradeOfferData> GetTradeDataAsync(DefaultRequest ajaxRequest, ulong tradeofferid)
	{
		var response = await Downloader.GetAsync(new(SteamCommunityUrls.TradeOffer + tradeofferid, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken,
        });
		if (!response.Success)
			return new() { TradeOfferId = tradeofferid };

		var html = new HtmlParser();
		var doc = await html.ParseDocumentAsync(response.Data!);
		var errorMsg = doc.GetElementById("error_msg");
		if (errorMsg != null)
			return new() { TradeOfferId = tradeofferid, Error = errorMsg.InnerHtml.GetClearWebString() };
		var linkForgotten = doc.GetElementsByClassName("login_create_btn");
		if (linkForgotten.Any())
			return new() { TradeOfferId = tradeofferid, Error = "Необходимо войти в аккаунт" };

        var isMarketAllowed = response.Data!.GetBetween("var g_bMarketAllowed = ", ";")!;
        var partnerSID64 = response.Data!.GetBetween("UserThem.SetSteamId( '", "'");
        var partnerProbation = response.Data!.GetBetween("var g_bTradePartnerProbation = ", ";")!;
        var partnerName = response.Data!.GetBetween("var g_strTradePartnerPersonaName = \"", "\";");
        var youSID64 = response.Data!.GetBetween("UserYou.SetSteamId( '", "'");
        var youName = response.Data!.GetBetween("var g_strYourPersonaName = \"", "\";");
        var sessionId = response.Data!.GetBetween("var g_sessionID = \"", "\";");
        var tradeStatus = response.Data!.GetBetween("var g_rgCurrentTradeStatus = ", ";");

        var tradeData = new TradeOfferData
		{
			IsSuccess = true,
			TradeOfferId = tradeofferid,
		};
		tradeData.SetTradeStatus(tradeStatus);
		tradeData.SetYou(youName, sessionId, isMarketAllowed, youSID64);
		tradeData.SetPartner(partnerName, partnerProbation, partnerSID64);
		return tradeData;
	}

    public static async Task<(LoginResultv2, SessionDatav2?)> AuthAsync(UserLoginv2 user_login)
    {
        if (user_login.FullyEnrolled)
            return (user_login.Result, user_login.Session);
        if (user_login.NextStep == NEXT_STEP.Begin && !await user_login.BeginAuthSessionViaCredentialsAsync())
            return (user_login.Result, null);
        if (user_login.NextStep == NEXT_STEP.Update && !await user_login.UpdateAuthSessionWithSteamGuardCodeAsync(user_login.Data!))
            return (user_login.Result, null);
        if (user_login.NextStep == NEXT_STEP.Poll && !await user_login.PollAuthSessionStatusAsync())
            return (user_login.Result, null);
        return (user_login.Result, user_login.Session);
    }
    public static async Task<(LoginResultv2, SessionDatav2?)> AuthAsync(string username, string password, string? guard_code, IWebProxy? proxy, EAuthTokenPlatformType platform)
    {
        var user_login = new UserLoginv2(username, password, platform, proxy);
        if (user_login.NextStep == NEXT_STEP.Begin && !await user_login.BeginAuthSessionViaCredentialsAsync())
            return (user_login.Result, null);
        if (user_login.NextStep == NEXT_STEP.Update && !await user_login.UpdateAuthSessionWithSteamGuardCodeAsync(guard_code))
            return (user_login.Result, null);
        if (user_login.NextStep == NEXT_STEP.Poll && !await user_login.PollAuthSessionStatusAsync())
            return (user_login.Result, null);
        return (user_login.Result, user_login.Session);
    }
    public static (LoginResultv2, SessionDatav2?) Auth(string username, string password, string? guard_code, IWebProxy? proxy, EAuthTokenPlatformType platform)
    {
        var user_login = new UserLoginv2(username, password, platform, proxy);
        if (user_login.NextStep == NEXT_STEP.Begin && !user_login.BeginAuthSessionViaCredentials())
            return (user_login.Result, null);
        if (user_login.NextStep == NEXT_STEP.Update && !user_login.UpdateAuthSessionWithSteamGuardCode(guard_code))
            return (user_login.Result, null);
        if (user_login.NextStep == NEXT_STEP.Poll && !user_login.PollAuthSessionStatus())
            return (user_login.Result, null);
        return (user_login.Result, user_login.Session);
    }
    public static (LoginResultv2, SessionDatav2?) Auth(UserLoginv2 user_login)
    {
        if (user_login.FullyEnrolled)
            return (user_login.Result, user_login.Session);
        if (user_login.NextStep == NEXT_STEP.Begin && !user_login.BeginAuthSessionViaCredentials())
            return (user_login.Result, null);
        if (user_login.NextStep == NEXT_STEP.Update && !user_login.UpdateAuthSessionWithSteamGuardCode(user_login.Data!))
            return (user_login.Result, null);
        if (user_login.NextStep == NEXT_STEP.Poll && !user_login.PollAuthSessionStatus())
            return (user_login.Result, null);
        return (user_login.Result, user_login.Session);
    }

    public static async Task<MemoryStream?> GetCaptchaImageToMemoryStreamAsync(string captchagid, DefaultRequest? ajaxRequest = null)
    {
        var (success, bytes, _) = await Downloader.GetCaptchaAsync(captchagid, ajaxRequest?.Proxy, ajaxRequest?.Session);
        if (!success)
            return null;
        var stream = new MemoryStream(bytes);
        return stream;
    }
    public static async Task<byte[]> GetCaptchaImageToBytesAsync(string captchagid, DefaultRequest? ajaxRequest = null)
    {
        var (success, bytes, _) = await Downloader.GetCaptchaAsync(captchagid, ajaxRequest?.Proxy, ajaxRequest?.Session);
        if (!success)
            return Array.Empty<byte>();
        return bytes;
    }
    public static MemoryStream? GetCaptchaImageToMemoryStream(string captchagid, DefaultRequest? ajaxRequest = null)
    {
        var (success, bytes, _) = Downloader.GetCaptcha(captchagid, ajaxRequest?.Proxy, ajaxRequest?.Session);
        if (!success)
            return null;
        var stream = new MemoryStream(bytes);
        return stream;
    }
    public static byte[] GetCaptchaImageToBytes(string captchagid, DefaultRequest? ajaxRequest = null)
    {
        var (success, bytes, _) = Downloader.GetCaptcha(captchagid, ajaxRequest?.Proxy, ajaxRequest?.Session);
        if (!success)
            return Array.Empty<byte>();
        return bytes;
    }
    
    /// <summary>
    /// Получает страницу предмета в steam
    /// </summary>
    /// <param name="appId">app id приложения, чей предмет парсим</param>
    /// <param name="market_hash_name">название предмет, который нам нужен</param>
    /// <returns>Полученные данные</returns>
    public static async Task<MarketItem> GetMarketItemAsync(DefaultRequest ajaxRequest, uint appId, string market_hash_name)
    {
        if (string.IsNullOrEmpty(market_hash_name))
            return new MarketItem() { IsError = true, Data = "Не указан market_hash_name" };

        market_hash_name = Uri.EscapeDataString(market_hash_name).Replace("%27", "'").Replace("?", "%3F").Replace("%E2%98%85", "★").Replace("%E2%84%A2", "™");
        string url = Path.Combine(SteamCommunityUrls.Market_Listings, appId.ToString(), market_hash_name);
        var response = await Downloader.GetAsync(new(url, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken
        });

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
    /// <param name="appId">app id приложения, чей предмет парсим</param>
    /// <param name="market_hash_name">название предмет, который нам нужен</param>
    /// <returns>Полученные данные</returns>
    public static MarketItem GetMarketItem(DefaultRequest ajaxRequest, uint appId, string market_hash_name)
    {
        if (string.IsNullOrEmpty(market_hash_name))
            return new MarketItem() { IsError = true, Data = "Не указан market_hash_name" };

        market_hash_name = Uri.EscapeDataString(market_hash_name).Replace("%27", "'").Replace("?", "%3F").Replace("%E2%98%85", "★").Replace("%E2%84%A2", "™");
		string url = Path.Combine(SteamCommunityUrls.Market_Listings, appId.ToString(), market_hash_name);
		var response = Downloader.Get(new(url, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken
        });

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
    public static Dictionary<uint, AppContextData> GetAppContextData(DefaultRequest ajaxRequest) => GetAppContextData(ajaxRequest, ajaxRequest.Session!.SteamID);
    /// <summary>
    /// Получает доступные инвентари аккаунта
    /// </summary>
    /// <returns>Коллекция доступных инвентарей, где Key=app_id</returns>
    public static Dictionary<uint, AppContextData> GetAppContextData(DefaultRequest ajaxRequest, ulong steamid64)
    {
        string url = "https://steamcommunity.com/profiles/" + steamid64 + "/inventory/";
        string referer = "https://steamcommunity.com/profiles/" + steamid64;
        var response = Downloader.Get(new(url, ajaxRequest.Proxy, ajaxRequest.Session, referer)
        {
            CancellationToken = ajaxRequest.CancellationToken,
        });
        if (!response.Success || response.Data.IsEmpty())
            return new(1);
        return AppContextData.Deserialize(response.Data!);
    }
    /// <summary>
    /// Получает доступные инвентари аккаунта
    /// </summary>
    /// <returns>Коллекция доступных инвентарей, где Key=app_id</returns>
    public static async Task<Dictionary<uint, AppContextData>> GetAppContextDataAsync(DefaultRequest ajaxRequest) =>
        await GetAppContextDataAsync(ajaxRequest, ajaxRequest.Session!.SteamID);
    /// <summary>
    /// Получает доступные инвентари аккаунта
    /// </summary>
    /// <returns>Коллекция доступных инвентарей, где Key=app_id</returns>
    public static async Task<Dictionary<uint, AppContextData>> GetAppContextDataAsync(DefaultRequest ajaxRequest, ulong steamid64)
	{
		string url = $"https://steamcommunity.com/profiles/" + steamid64 + "/inventory/";
		string referer = $"https://steamcommunity.com/profiles/" + steamid64;
		var response = await Downloader.GetAsync(new(url, ajaxRequest.Proxy, ajaxRequest.Session, referer));
        if (!response.Success || response.Data.IsEmpty())
            return new(1);
        return AppContextData.Deserialize(response.Data!);
    }

    /// <summary>
    /// Принимает трейд
    /// </summary>
    /// <param name="tradeofferid">id трейда для принятия</param>
    /// <param name="steamid_other">steamid32 аккаунта, который отослал трейд</param>
    /// <returns>False трейд не принят, но иногда может быть принят, для точности используйте API</returns>
    [Obsolete("Данный метод переехал в Script.Ajax.tradeoffer_accept")]
    public static (ConfTradeOffer?, SteamTradeError?) AcceptTrade(DefaultRequest ajaxRequest, ulong tradeofferid, uint steamid_other)
        => AcceptTrade(ajaxRequest, tradeofferid, Steam32ToSteam64(steamid_other));
    /// <summary>
    /// Принимает трейд
    /// </summary>
    /// <param name="tradeofferid">id трейда для принятия</param>
    /// <param name="steamid_other">steamid32 аккаунта, который отослал трейд</param>
    /// <returns>False трейд не принят, но иногда может быть принят, для точности используйте API</returns>
    [Obsolete("Данный метод переехал в Script.Ajax.tradeoffer_accept")]
    public static (ConfTradeOffer?, SteamTradeError?) AcceptTrade(DefaultRequest ajaxRequest, ulong tradeofferid, ulong steamid64)
    {
        var request = new PostRequest("https://steamcommunity.com/tradeoffer/" + tradeofferid + "/accept", Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            CancellationToken = ajaxRequest.CancellationToken,
            IsAjax = true,
            Referer = "https://steamcommunity.com/tradeoffer/" + tradeofferid + "/"
        }
        .AddPostData("sessionid", ajaxRequest.Session!.SessionID).AddPostData("serverid", 1).AddPostData("tradeofferid", tradeofferid)
        .AddPostData("partner", steamid64).AddPostData("captcha", "");
        var response = Downloader.Post(request);
        try
        {
            if (!response.Success)
            {
                var steamerror = SteamTradeError.Deserialize(response.Data!);
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
                return (null, new() { StrError = ex.Message });
            }
        }
        catch (Exception ex)
        {
            return (null, new() { StrError = ex.Message });
        }
    }
    [Obsolete("Данный метод переехал в Script.Ajax.tradeoffer_accept")]
    public static (ConfTradeOffer?, SteamTradeError?) AcceptTrade(DefaultRequest ajaxRequest, Trade trade) =>
        AcceptTrade(ajaxRequest, trade.u_tradeofferid, trade.accountid_other);
    /// <summary>
    /// Принимает трейд
    /// </summary>
    /// <param name="tradeofferid">id трейда для принятия</param>
    /// <param name="steamid_other">steamid32 аккаунта, который отослал трейд</param>
    /// <returns>False трейд не принят, но иногда может быть принят, для точности используйте API</returns>
    [Obsolete("Данный метод переехал в Script.Ajax.tradeoffer_accept_async")]
    public static async Task<(ConfTradeOffer?, SteamTradeError?)> AcceptTradeAsync(DefaultRequest ajaxRequest, ulong tradeofferid, uint steamid_other)
        => await AcceptTradeAsync(ajaxRequest, tradeofferid, Steam32ToSteam64(steamid_other));
    /// <summary>
    /// Принимает трейд
    /// </summary>
    /// <param name="tradeofferid">id трейда для принятия</param>
    /// <param name="steamid_other">steamid32 аккаунта, который отослал трейд</param>
    /// <returns>False трейд не принят, но иногда может быть принят, для точности используйте API</returns>
    [Obsolete("Данный метод переехал в Script.Ajax.tradeoffer_accept_async")]
    public static async Task<(ConfTradeOffer?, SteamTradeError?)> AcceptTradeAsync(DefaultRequest ajaxRequest, ulong tradeofferid, ulong steamid64)
    {
        var request = new PostRequest("https://steamcommunity.com/tradeoffer/" + tradeofferid + "/accept", Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            CancellationToken = ajaxRequest.CancellationToken,
            IsAjax = true,
			Referer = "https://steamcommunity.com/tradeoffer/" + tradeofferid + "/"
		}
        .AddPostData("sessionid", ajaxRequest.Session!.SessionID).AddPostData("serverid", 1).AddPostData("tradeofferid", tradeofferid)
        .AddPostData("partner", steamid64).AddPostData("captcha", "");
        var response = await Downloader.PostAsync(request);
        try
        {
            if (!response.Success)
            {
                var steamerror = SteamTradeError.Deserialize(response.Data!);
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
                return (null, new() { StrError = ex.Message });
            }
        }
        catch (Exception ex)
        {
            return (null, new() { StrError = ex.Message });
        }
    }

    public static async Task<InvormationProfileResponse> SetAccountInfoAsync(DefaultRequest ajaxRequest, InformationProfileRequest info)
    {
        string url = "https://steamcommunity.com/profiles/" + info.steamID + "/edit/";
        var request = new PostRequest(url, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = "https://steamcommunity.com/profiles/" + info.steamID + "/edit/info",
            CancellationToken = ajaxRequest.CancellationToken
        };
        request.PostData.AddRange(info.GetPostData());
        var response = await Downloader.PostAsync(request);
        if (response.Data?.Contains("<div class=\"profile_fatalerror_message\">") == true)
            return new() { errmsg = "КТ" };
        if (!response.Success) return new() { errmsg = "Не удалось отправить запрос" };
        try
        {
            var obj = JsonSerializer.Deserialize<InvormationProfileResponse>(response.Data!)!;
            return obj;
        }
        catch (Exception)
        {
            return new InvormationProfileResponse()
            {
                success = 0,
                errmsg = "Ошибка при десерилизации данных '" + response.Data + "'"
            };
        }
    }
    public static async Task<UploadImageResponse> UploadAvatarAsync(DefaultRequest ajaxRequest, string filename)
    {
        var request = new PostRequest(SteamCommunityUrls.Actions_FileUploader, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            Referer = $"https://steamcommunity.com/profiles/{ajaxRequest.Session!.SteamID}/edit/avatar",
            CancellationToken = ajaxRequest.CancellationToken
        }
        .AddPostData("type", "player_avatar_image").AddPostData("sId", ajaxRequest.Session!.SteamID).AddPostData("sessionid", ajaxRequest.Session!.SessionID)
        .AddPostData("doSub", 1).AddPostData("json", 1);
        var response = await Downloader.UploadFilesToRemoteUrlAsync(request, filename);
        if (!response.Success)
            return new() { success = false, message = response.Data! };
        try
        {
            var obj = JsonSerializer.Deserialize<UploadImageResponse>(response.Data!)!;
            return obj;
        }
        catch (Exception)
        {
            return new UploadImageResponse() { success = false, message = $"Ошибка при десерилизации данных '{response.Data}'" };
        }
    }
    [Obsolete("Данный метод более не может нормально парсить страницу CS2: operationquests")]
    public static async Task<OperationProgress> CSGO_OperationProgressAsync(DefaultRequest ajaxRequest)
    {
        string url = $"https://steamcommunity.com/profiles/{ajaxRequest.Session!.SteamID}/gcpd/730/?tab=operationquests";
        var response = await Downloader.GetAsync(new GetRequest(url, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken,
        });
        if (!response.Success)
            return new OperationProgress("Ошибка при запросе");
        else if (response.Data == "<!DOCTYPE html>")
            return new OperationProgress("Бан на запросы");
        HtmlParser html = new HtmlParser();
        var parser = await html.ParseDocumentAsync(response.Data!);
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
    [Obsolete("Данный метод более не может нормально парсить страницу CS2: accountmain")]
    public static async Task<AccountMain> CSGO_AccountMainAsync(DefaultRequest ajaxRequest)
    {
        string url = $"https://steamcommunity.com/profiles/{ajaxRequest.Session!.SteamID}/gcpd/730/?tab=accountmain";
        var response = await Downloader.GetAsync(new(url, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken,
        });
        if (!response.Success)
            return new("Ошибка при запросе");
        else if (response.Data == "<!DOCTYPE html>")
            return new("Бан на запросы");
        HtmlParser html = new HtmlParser();
        var parser = await html.ParseDocumentAsync(response.Data!);
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
    [Obsolete("Данный метод более не может нормально парсить страницу CS2: matchmaking")]
    public static async Task<Matchmaking> CSGO_MatchmakingAsync(DefaultRequest ajaxRequest)
    {
        string url = $"https://steamcommunity.com/profiles/{ajaxRequest.Session!.SteamID}/gcpd/730/?tab=matchmaking";
        var response = await Downloader.GetAsync(new(url, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken,
        });
        if (!response.Success)
            return new("Ошибка при запросе");
        else if (response.Data == "<!DOCTYPE html>")
            return new("Бан на запросы");
        HtmlParser html = new HtmlParser();
        var parser = await html.ParseDocumentAsync(response.Data!);
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

    public static SteamOfferResponse GetIncomingOffers(DefaultRequest ajaxRequest)
    {
        string url = "https://steamcommunity.com/profiles/" + ajaxRequest.Session!.SteamID + "/tradeoffers/";
        var response = Downloader.Get(new(url, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken,
        });
        if (!response.Success)
            return new() { Error = "Ошибка при запросе" };
        else if (response.Data == "<!DOCTYPE html>")
            return new() { Error = "Бан на запросы" };

        HtmlParser html = new HtmlParser();
        var parser = html.ParseDocument(response.Data!);
        var tradeoffers = parser.GetElementsByClassName("tradeoffer");
        var list = new List<SteamOffer>(tradeoffers.Length + 1);
        foreach (var tradeoffer in tradeoffers)
        {
            var tradeofferid = tradeoffer.Id?.Replace("tradeofferid_", string.Empty).ParseUInt64() ?? 0;
            uint partner_steamid32 = 0u;
            var partner_online = false;
            var partnerItems = Array.Empty<SteamOfferItem>();
            var ourItems = Array.Empty<SteamOfferItem>();

            var tradeoffer_items_ctn = tradeoffer.GetElementsByClassName("tradeoffer_items_ctn").FirstOrDefault();
            bool is_active = tradeoffer_items_ctn != default && tradeoffer_items_ctn.ClassList.Contains("active");

            var banner = tradeoffer_items_ctn?.GetElementsByClassName("tradeoffer_items_banner").FirstOrDefault();
            var accepted = banner?.ClassList.Contains("accepted") == true;

            var tradeofferItems = tradeoffer.GetElementsByClassName("tradeoffer_items");
            foreach (var items in tradeofferItems)
            {
                bool isPrimary = items.ClassList.Contains("primary");
                if (!isPrimary && !items.ClassList.Contains("secondary"))
                {
                    continue;
                }

                var trade_items = items.GetElementsByClassName("trade_item");
                var tradeItemsList = new List<SteamOfferItem>(trade_items.Length + 1);
                foreach (var trade_item in trade_items)
                {
                    var economyItem = trade_item.GetAttribute("data-economy-item");
                    var (success, appId, classId, instanceId) = SteamOfferItem.ParseEconomyData(economyItem);
                    if (!success)
                    {
                        return new() { Error = "Не удалось спарсить data-economy-item=" + economyItem };
                    }

                    var steamOfferItem = new SteamOfferItem
                    {
                        AppId = appId,
                        ClassId = classId,
                        InstanceId = instanceId,
                    };
                    tradeItemsList.Add(steamOfferItem);
                }

				uint steamid32 = 0u;
				var online = false;
				var partnerProfile = tradeoffer.GetElementsByClassName("tradeoffer_avatar").FirstOrDefault();
				if (partnerProfile != default)
				{
					steamid32 = partnerProfile.GetAttribute("data-miniprofile").ParseUInt32();
					online = partnerProfile.ClassList.Contains("online");
				}

				if (isPrimary)
                {
					partnerItems = tradeItemsList.ToArray();
                    partner_online = online;
                    partner_steamid32 = steamid32;
				}
                else
                {
					ourItems = tradeItemsList.ToArray();
				}
            }

            if (tradeofferid == 0)
            {
                return new() { Error = "Не удалось узнать tradeofferid" };
            }
            if (partner_steamid32 == 0)
            {
                return new() { Error = "Не удалось узнать partner_steamid32 у " + tradeofferid };
            }
            var (status, datetime) = SteamOffer.ParseBanner(banner?.TextContent.GetClearWebString());

            list.Add(new()
            {
                OurItems = ourItems,
                PartnerId = partner_steamid32,
                PartnerItems = partnerItems,
                TradeOfferId = tradeofferid,
                PartnerOnline = partner_online,
                ActiveOffer = is_active,
                Status = status,
                StatusTime = datetime,
                AcceptedOffer = accepted,
            });
        }
        return new()
        {
            Success = true,
            Trades = list.ToArray(),
        };
    }
    public static async Task<SteamOfferResponse> GetIncomingOffersAsync(DefaultRequest ajaxRequest)
    {
        string url = "https://steamcommunity.com/profiles/" + ajaxRequest.Session!.SteamID + "/tradeoffers/";
        var response = await Downloader.GetAsync(new(url, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken,
        });
        if (!response.Success)
            return new() { Error = "Ошибка при запросе" };
        else if (response.Data == "<!DOCTYPE html>")
            return new() { Error = "Бан на запросы" };

        HtmlParser html = new HtmlParser();
        var parser = await html.ParseDocumentAsync(response.Data!);
        var tradeoffers = parser.GetElementsByClassName("tradeoffer");
        var list = new List<SteamOffer>(tradeoffers.Length + 1);
		foreach (var tradeoffer in tradeoffers)
		{
			var tradeofferid = tradeoffer.Id?.Replace("tradeofferid_", string.Empty).ParseUInt64() ?? 0;
			uint partner_steamid32 = 0u;
			var partner_online = false;
			var partnerItems = Array.Empty<SteamOfferItem>();
			var ourItems = Array.Empty<SteamOfferItem>();

			var tradeoffer_items_ctn = tradeoffer.GetElementsByClassName("tradeoffer_items_ctn").FirstOrDefault();
			bool is_active = tradeoffer_items_ctn != default && tradeoffer_items_ctn.ClassList.Contains("active");

			var banner = tradeoffer_items_ctn?.GetElementsByClassName("tradeoffer_items_banner").FirstOrDefault();
			var accepted = banner?.ClassList.Contains("accepted") == true;

			var tradeofferItems = tradeoffer.GetElementsByClassName("tradeoffer_items");
			foreach (var items in tradeofferItems)
			{
				bool isPrimary = items.ClassList.Contains("primary");
				if (!isPrimary && !items.ClassList.Contains("secondary"))
				{
					continue;
				}

				var trade_items = items.GetElementsByClassName("trade_item");
				var tradeItemsList = new List<SteamOfferItem>(trade_items.Length + 1);
				foreach (var trade_item in trade_items)
				{
					var economyItem = trade_item.GetAttribute("data-economy-item");
					var (success, appId, classId, instanceId) = SteamOfferItem.ParseEconomyData(economyItem);
					if (!success)
					{
						return new() { Error = "Не удалось спарсить data-economy-item=" + economyItem };
					}

					var steamOfferItem = new SteamOfferItem
					{
						AppId = appId,
						ClassId = classId,
						InstanceId = instanceId,
					};
					tradeItemsList.Add(steamOfferItem);
				}

				uint steamid32 = 0u;
				var online = false;
				var partnerProfile = tradeoffer.GetElementsByClassName("tradeoffer_avatar").FirstOrDefault();
				if (partnerProfile != default)
				{
					steamid32 = partnerProfile.GetAttribute("data-miniprofile").ParseUInt32();
					online = partnerProfile.ClassList.Contains("online");
				}

				if (isPrimary)
				{
					partnerItems = tradeItemsList.ToArray();
					partner_online = online;
					partner_steamid32 = steamid32;
				}
				else
				{
					ourItems = tradeItemsList.ToArray();
				}
			}

			if (tradeofferid == 0)
			{
				return new() { Error = "Не удалось узнать tradeofferid" };
			}
			if (partner_steamid32 == 0)
			{
				return new() { Error = "Не удалось узнать partner_steamid32 у " + tradeofferid };
			}
			var (status, datetime) = SteamOffer.ParseBanner(banner?.TextContent.GetClearWebString());

			list.Add(new()
			{
				OurItems = ourItems,
				PartnerId = partner_steamid32,
				PartnerItems = partnerItems,
				TradeOfferId = tradeofferid,
				PartnerOnline = partner_online,
				ActiveOffer = is_active,
				Status = status,
				StatusTime = datetime,
				AcceptedOffer = accepted,
			});
		}
		return new()
        {
            Success = true,
            Trades = list.ToArray(),
        };
    }
    public static SteamOfferResponse GetSentOffers(DefaultRequest ajaxRequest)
    {
        string url = "https://steamcommunity.com/profiles/" + ajaxRequest.Session!.SteamID + "/tradeoffers/sent/";
        var response = Downloader.Get(new(url, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken,
        });
        if (!response.Success)
            return new() { Error = "Ошибка при запросе" };
        else if (response.Data == "<!DOCTYPE html>")
            return new() { Error = "Бан на запросы" };

        HtmlParser html = new HtmlParser();
        var parser = html.ParseDocument(response.Data!);
        var tradeoffers = parser.GetElementsByClassName("tradeoffer");
        var list = new List<SteamOffer>(tradeoffers.Length + 1);
        foreach (var tradeoffer in tradeoffers)
        {
            var tradeofferid = tradeoffer.Id?.Replace("tradeofferid_", string.Empty).ParseUInt64() ?? 0;
            uint partner_steamid32 = 0u;
            var partner_online = false;
            var partnerItems = Array.Empty<SteamOfferItem>();
            var ourItems = Array.Empty<SteamOfferItem>();

            var tradeoffer_items_ctn = tradeoffer.GetElementsByClassName("tradeoffer_items_ctn").FirstOrDefault();
            bool is_active = tradeoffer_items_ctn != default && tradeoffer_items_ctn.ClassList.Contains("active");

            var banner = tradeoffer_items_ctn?.GetElementsByClassName("tradeoffer_items_banner").FirstOrDefault();
            var accepted = banner?.ClassList.Contains("accepted") == true;

            var tradeofferItems = tradeoffer.GetElementsByClassName("tradeoffer_items");
            foreach (var items in tradeofferItems)
            {
                bool isPrimary = items.ClassList.Contains("primary");
                if (!isPrimary && !items.ClassList.Contains("secondary"))
                {
                    continue;
                }

                var trade_items = items.GetElementsByClassName("trade_item");

                var tradeItemsList = new List<SteamOfferItem>(trade_items.Length + 1);
                foreach (var trade_item in trade_items)
                {
                    var economyItem = trade_item.GetAttribute("data-economy-item");
                    var (success, appId, classId, instanceId) = SteamOfferItem.ParseEconomyData(economyItem);
                    if (!success)
                    {
                        return new() { Error = "Не удалось спарсить data-economy-item=" + economyItem };
                    }

                    var steamOfferItem = new SteamOfferItem
                    {
                        AppId = appId,
                        ClassId = classId,
                        InstanceId = instanceId,
                    };
                    tradeItemsList.Add(steamOfferItem);
                }

				uint steamid32 = 0u;
				var online = false;

				var tradeoffer_avatar = items.GetElementsByClassName("tradeoffer_avatar").FirstOrDefault();
				if (tradeoffer_avatar != default)
				{
					steamid32 = tradeoffer_avatar.GetAttribute("data-miniprofile").ParseUInt32();
					online = tradeoffer_avatar.ClassList.Contains("online");
				}

				if (isPrimary)
                {
					ourItems = tradeItemsList.ToArray();
				}
                else
                {
					partnerItems = tradeItemsList.ToArray();
					partner_steamid32 = steamid32;
					partner_online = online;
				}
            }

            if (tradeofferid == 0)
            {
                return new() { Error = "Не удалось узнать tradeofferid" };
            }
            if (partner_steamid32 == 0)
            {
                return new() { Error = "Не удалось узнать partner_steamid32 у " + tradeofferid };
            }
            var (status, datetime) = SteamOffer.ParseBanner(banner?.TextContent.GetClearWebString());
            list.Add(new()
            {
                OurItems = ourItems,
                PartnerId = partner_steamid32,
                PartnerItems = partnerItems,
                TradeOfferId = tradeofferid,
                PartnerOnline = partner_online,
                ActiveOffer = is_active,
                Status = status,
                StatusTime = datetime,
                AcceptedOffer = accepted,
            });
        }
        return new()
        {
            Success = true,
            Trades = list.ToArray(),
        };
    }
    public static async Task<SteamOfferResponse> GetSentOffersAsync(DefaultRequest ajaxRequest)
    {
        string url = "https://steamcommunity.com/profiles/" + ajaxRequest.Session!.SteamID + "/tradeoffers/sent/";
        var response = await Downloader.GetAsync(new(url, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken,
        });
        if (!response.Success)
            return new() { Error = "Ошибка при запросе" };
        else if (response.Data == "<!DOCTYPE html>")
            return new() { Error = "Бан на запросы" };

        HtmlParser html = new HtmlParser();
        var parser = await html.ParseDocumentAsync(response.Data!);
        var tradeoffers = parser.GetElementsByClassName("tradeoffer");
        var list = new List<SteamOffer>(tradeoffers.Length + 1);
		foreach (var tradeoffer in tradeoffers)
		{
			var tradeofferid = tradeoffer.Id?.Replace("tradeofferid_", string.Empty).ParseUInt64() ?? 0;
			uint partner_steamid32 = 0u;
			var partner_online = false;
			var partnerItems = Array.Empty<SteamOfferItem>();
			var ourItems = Array.Empty<SteamOfferItem>();

			var tradeoffer_items_ctn = tradeoffer.GetElementsByClassName("tradeoffer_items_ctn").FirstOrDefault();
			bool is_active = tradeoffer_items_ctn != default && tradeoffer_items_ctn.ClassList.Contains("active");

			var banner = tradeoffer_items_ctn?.GetElementsByClassName("tradeoffer_items_banner").FirstOrDefault();
			var accepted = banner?.ClassList.Contains("accepted") == true;

			var tradeofferItems = tradeoffer.GetElementsByClassName("tradeoffer_items");
			foreach (var items in tradeofferItems)
			{
				bool isPrimary = items.ClassList.Contains("primary");
				if (!isPrimary && !items.ClassList.Contains("secondary"))
				{
					continue;
				}

				var trade_items = items.GetElementsByClassName("trade_item");

				var tradeItemsList = new List<SteamOfferItem>(trade_items.Length + 1);
				foreach (var trade_item in trade_items)
				{
					var economyItem = trade_item.GetAttribute("data-economy-item");
					var (success, appId, classId, instanceId) = SteamOfferItem.ParseEconomyData(economyItem);
					if (!success)
					{
						return new() { Error = "Не удалось спарсить data-economy-item=" + economyItem };
					}

					var steamOfferItem = new SteamOfferItem
					{
						AppId = appId,
						ClassId = classId,
						InstanceId = instanceId,
					};
					tradeItemsList.Add(steamOfferItem);
				}

				uint steamid32 = 0u;
				var online = false;

				var tradeoffer_avatar = items.GetElementsByClassName("tradeoffer_avatar").FirstOrDefault();
				if (tradeoffer_avatar != default)
				{
					steamid32 = tradeoffer_avatar.GetAttribute("data-miniprofile").ParseUInt32();
					online = tradeoffer_avatar.ClassList.Contains("online");
				}

				if (isPrimary)
				{
					ourItems = tradeItemsList.ToArray();
				}
				else
				{
					partnerItems = tradeItemsList.ToArray();
					partner_steamid32 = steamid32;
					partner_online = online;
				}
			}

			if (tradeofferid == 0)
			{
				return new() { Error = "Не удалось узнать tradeofferid" };
			}
			if (partner_steamid32 == 0)
			{
				return new() { Error = "Не удалось узнать partner_steamid32 у " + tradeofferid };
			}
			var (status, datetime) = SteamOffer.ParseBanner(banner?.TextContent.GetClearWebString());
			list.Add(new()
			{
				OurItems = ourItems,
				PartnerId = partner_steamid32,
				PartnerItems = partnerItems,
				TradeOfferId = tradeofferid,
				PartnerOnline = partner_online,
				ActiveOffer = is_active,
				Status = status,
				StatusTime = datetime,
				AcceptedOffer = accepted,
			});
		}
		return new()
        {
            Success = true,
            Trades = list.ToArray(),
        };
    }

    /// <summary>
    /// Собирает информацию об игровых блокировках и vac банах на аккаунте
    /// </summary>
    /// <exception cref="InvalidOperationException"/>
    /// <returns>Данные о блокировка на этом аккаунте</returns>
    public static VacGameBansData GetVacAndGameBans(DefaultRequest ajaxRequest)
    {
        var response = Downloader.Get(new(SteamPoweredUrls.Wizard_VacBans, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken,
        });
        if (!response.Success)
            return new("Ошибка при запросе") { Apps = ImmutableList<VacGameBanModel>.Empty };
        else if (response.Data == "<!DOCTYPE html>")
            return new("Бан на запросы") { Apps = ImmutableList<VacGameBanModel>.Empty };
        return VacGameBansData.Deserialize(response.Data!);
    }
    /// <summary>
    /// Собирает информацию об игровых блокировках и vac банах на аккаунте
    /// </summary>
    /// <exception cref="InvalidOperationException"/>
    /// <returns>Данные о блокировка на этом аккаунте</returns>
    public static async Task<VacGameBansData> GetVacAndGameBansAsync(DefaultRequest ajaxRequest)
    {
        var response = await Downloader.GetAsync(new(SteamPoweredUrls.Wizard_VacBans, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken,
        });
        if (!response.Success)
            return new("Ошибка при запросе") { Apps = ImmutableList<VacGameBanModel>.Empty };
        else if (response.Data == "<!DOCTYPE html>")
            return new("Бан на запросы") { Apps = ImmutableList<VacGameBanModel>.Empty };
        return VacGameBansData.Deserialize(response.Data!);
    }

    /// <summary>
    /// Загружает начальную историю покупок аккаунта.
    /// <para/>
    /// Для дальнейшей загрузки нужно использовать встроенный метод <see cref="PurchaseHistoryData.LoadMoreHistory(DefaultRequest)"/>.
    /// 
    /// <para/>
    /// Пример полной загрузки истории покупок:
    /// <code>
    /// var history = Steam.GetPurchaseHistory(new(session));
    /// while(history.Success AND history.Cursor != null)
    /// {
    ///     history = history.LoadMoreHistory(new(session));
    /// }
    /// </code>
    /// 
    /// </summary>
    /// <returns>Класс с историей покупок, либо ошибки</returns>
    public static PurchaseHistoryData GetPurchaseHistory(DefaultRequest ajaxRequest)
    {
        var response = Downloader.Get(new(SteamPoweredUrls.Account_History, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken,
            Referer = SteamPoweredUrls.Account,
        });
        if (!response.Success)
            return new("Ошибка при запросе") { History = ImmutableList<PurchaseHistoryModel>.Empty };
        else if (response.Data == "<!DOCTYPE html>")
            return new("Бан на запросы") { History = ImmutableList<PurchaseHistoryModel>.Empty };
        return PurchaseHistoryData.Deserialize(response.Data!);
    }
    /// <summary>
    /// Загружает начальную историю покупок аккаунта.
    /// <para/>
    /// Для дальнейшей загрузки нужно использовать встроенный метод <see cref="PurchaseHistoryData.LoadMoreHistory(DefaultRequest)"/>.
    /// 
    /// <para/>
    /// Пример полной загрузки истории покупок:
    /// <code>
    /// var history = Steam.GetPurchaseHistory(new(session));
    /// while(history.Success AND history.Cursor != null)
    /// {
    ///     history = history.LoadMoreHistory(new(session));
    /// }
    /// </code>
    /// 
    /// </summary>
    /// <returns>Класс с историей покупок, либо ошибки</returns>
    public static async Task<PurchaseHistoryData> GetPurchaseHistoryAsync(DefaultRequest ajaxRequest)
    {
        var response = await Downloader.GetAsync(new(SteamPoweredUrls.Account_History, ajaxRequest.Proxy, ajaxRequest.Session)
        {
            CancellationToken = ajaxRequest.CancellationToken,
            Referer = SteamPoweredUrls.Account,
        });
        if (!response.Success)
            return new("Ошибка при запросе") { History = ImmutableList<PurchaseHistoryModel>.Empty };
        else if (response.Data == "<!DOCTYPE html>")
            return new("Бан на запросы") { History = ImmutableList<PurchaseHistoryModel>.Empty };
        return PurchaseHistoryData.Deserialize(response.Data!);
    }

    public static ulong Steam32ToSteam64(uint input) => SteamIDConverter + input;
    public static uint Steam64ToSteam32(ulong input) => (uint)(input - SteamIDConverter);
    /// <summary>
    /// </summary>
    /// <param name="communityId">SteamID64(76561197960265728)</param>
    /// <returns>String.empty if error, else the string SteamID2(STEAM_0:1:000000)</returns>
    public static string Steam64ToSteam2(ulong communityId)
    {
        if (communityId < 76561197960265729L || !Regex.IsMatch(communityId.ToString(CultureInfo.InvariantCulture), @"^7656119([0-9]{10})$", RegexOptions.Compiled | RegexOptions.Singleline))
            return string.Empty;
        communityId -= 76561197960265728L;
        ulong num = communityId % 2L;
        communityId -= num;
        string input = string.Format("STEAM_0:{0}:{1}", num, (communityId / 2L));
        if (!Regex.IsMatch(input, @"^STEAM_0:[0-1]:([0-9]{1,10})$", RegexOptions.Compiled | RegexOptions.Singleline))
            return string.Empty;
        return input;
    }
    /// <summary>
    /// </summary>
    /// <param name="communityId">SteamID64(76561197960265728)</param>
    /// <returns>String.empty if error, else the string SteamID3(U:1:000000)</returns>
    public static string Steam64ToSteam3(ulong communityId)
    {
        if (communityId < 76561197960265729L || !Regex.IsMatch(communityId.ToString(CultureInfo.InvariantCulture), @"^7656119([0-9]{10})$", RegexOptions.Compiled | RegexOptions.Singleline))
            return string.Empty;
        communityId -= SteamIDConverter;
        ulong num = communityId % 2L;
        communityId -= num;
        string input = string.Format("U:{0}:{1}", num, (uint)communityId);
        if (!Regex.IsMatch(input, @"^U:[0-1]:([0-9]{1,10})$", RegexOptions.Compiled | RegexOptions.Singleline))
            return string.Empty;
        return input;
    }
    /// <summary>
    /// Разбивает трейд ссылку на steamid32 и token
    /// </summary>
    /// <param name="tradeUrl">Полная трейд ссылка</param>
    /// <returns>true если трейд ссылка разбита</returns>
    public static (bool, uint, string?) SplitTradeURL(string tradeUrl)
    {
        if (tradeUrl.IsEmpty())
            return (false, 0, null);
        try
        {
            var match = _rgxTradeurl1.Match(tradeUrl);
            if (match.Success)
            {
                var partner = match.Groups[1].Value.ParseUInt32();
                var token = match.Groups[2].Value;
                return (true, partner, token);
            }
            return (false, 0, null);
        }
        catch (RegexMatchTimeoutException)
        {
            return (false, 0, null);
        }
    }

    /// <summary>
    /// Расчитать цены и комиссии по цене пользователя
    /// </summary>
    /// <param name="receivedAmount"></param>
    /// <param name="publisherFee"></param>
    /// <returns>(steam_fee - стим комса, publisher_fee - комиссия издателя, fees - общая комиссия, amount - сколько в общем должен заплатить пользователь)</returns>
    public static (uint, uint, uint, uint) CalculateAmountToSendForDesiredReceivedAmount(int receivedAmount, float publisherFee = 0.10f)
    {
        float wallet_fee_percent = 0.05f;
        float wallet_fee_minimum = 1f;
        int wallet_fee_base = 0;
        uint nSteamFee = (uint)Math.Floor(Math.Max(receivedAmount * wallet_fee_percent, wallet_fee_minimum + wallet_fee_base));
        uint nPublisherFee = (uint)Math.Floor((double)publisherFee > 0 ? Math.Max(receivedAmount * publisherFee, 1) : 0);
        long nAmountToSend = receivedAmount + nSteamFee + nPublisherFee;
        return (nSteamFee, nPublisherFee, nSteamFee + nPublisherFee, (uint)nAmountToSend);
    }
	/// <summary>
	/// Получить комиссии по указаной цене
	/// <code>
	/// var (_, _, fee, amount) = Steam.CalculateFeeAmount(price_with_coms);
	/// var price_to_sell_via_market_sellitem = amount - fee;
	/// </code>
	/// </summary>
	/// <param name="amount"></param>
	/// <param name="publisherFee"></param>
	/// <returns>(steam_fee - стим комса, publisher_fee - комиссия издателя, fees - общая комиссия, amount - сколько в общем должен заплатить пользователь)</returns>
	public static (uint, uint, uint, uint) CalculateFeeAmount(int amount, float publisherFee = 0.10f)
    {
        float wallet_fee_percent = 0.05f;
        int wallet_fee_base = 0;
        // Since CalculateFeeAmount has a Math.floor, we could be off a cent or two. Let's check:
        var iterations = 0; // shouldn't be needed, but included to be sure nothing unforseen causes us to get stuck
        var nEstimatedAmountOfWalletFundsReceivedByOtherParty = (amount - wallet_fee_base) / (wallet_fee_percent + publisherFee + 1);

        var bEverUndershot = false;
        var fees = CalculateAmountToSendForDesiredReceivedAmount((int)nEstimatedAmountOfWalletFundsReceivedByOtherParty, publisherFee);
        long fees_steam = fees.Item1;
		long fees_publisher = fees.Item2;
		long fees_fees = fees.Item3;
		long fees_amount = fees.Item4;

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
                    nEstimatedAmountOfWalletFundsReceivedByOtherParty--;
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