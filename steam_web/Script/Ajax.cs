using System.Text.Json;
using SteamWeb.Web;
using SteamWeb.Extensions;
using SteamWeb.Script.Enums;
using System.Web;
using SteamWeb.Script.DTO;
using SteamWeb.Script.DTO.CookiePreferences;
using SteamWeb.Script.DTO.Listinging;
using SteamWeb.Script.DTO.Historing;
using System.Text.RegularExpressions;
using ProtoBuf;
using System.Text.Json.Serialization;
using SteamWeb.Script.Models;
using SteamWeb.Models;
using SteamWeb.Models.PurchaseHistory;
using SteamWeb.API.Models.IEconService;
using SteamWeb.Models.Trade;
using SteamWeb.Script.DTO.ItemRender;

namespace SteamWeb.Script;
/// <summary>
/// Здесь собраны все http методы внутреннего api, кроме методов со поддомена help. - их реализация в классе <see cref="AjaxHelp"/>.
/// </summary>
public static class Ajax
{
    #region jwt refresh
	public static AjaxRefreshResponse jwt_ajaxrefresh(DefaultRequest defaultRequest, string steamRefresh_steam, string redir = "https://steamcommunity.com/market/")
	{
        if (!steamRefresh_steam.StartsWith('7'))
        {
            var jwt = JwtData.Deserialize(steamRefresh_steam);
			steamRefresh_steam = jwt.Subject + "%7C%7C" + steamRefresh_steam;
        }
        var redirUri = new Uri(redir);
        var request = new PostRequest(SteamPoweredUrls.Jwt_AjaxRefresh, Downloader.AppFormUrlEncoded)
		{
			Proxy = defaultRequest.Proxy,
			IsAjax = true,
			CancellationToken = defaultRequest.CancellationToken,
            SteamRefresh_Steam = steamRefresh_steam,
            Referer = redirUri.Scheme + "://" + redirUri.Host,
        };
        request.AddHeader("Origin", request.Referer);
		request.AddPostData("redir", Uri.EscapeDataString(redir!), false);
		var response = Downloader.Post(request);
		if (!response.Success)
			return new();
		try
		{
			// выполняем десерилизацию потому что присылают пустой объект
			// так мы понимаем что запрос точно прошёл
	        var obj = JsonSerializer.Deserialize<AjaxRefreshResponse>(response.Data!, Steam.JsonOptions)!;
			return obj;
		}
		catch (Exception)
		{
			return new();
		}
    }
    public static async Task<AjaxRefreshResponse> jwt_ajaxrefresh_async(DefaultRequest defaultRequest, string steamRefresh_steam, string redir = "https://steamcommunity.com/market/")
    {
        if (!steamRefresh_steam.StartsWith('7'))
        {
            var jwt = JwtData.Deserialize(steamRefresh_steam);
            steamRefresh_steam = jwt.Subject + "%7C%7C" + steamRefresh_steam;
        }
        var redirUri = new Uri(redir);
        var request = new PostRequest(SteamPoweredUrls.Jwt_AjaxRefresh, Downloader.AppFormUrlEncoded)
        {
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
            SteamRefresh_Steam = steamRefresh_steam,
            Referer = redirUri.Scheme + "://" + redirUri.Host,
        };
        request.AddHeader("Origin", request.Referer);
        request.AddPostData("redir", Uri.EscapeDataString(redir!), false);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            // выполняем десерилизацию потому что присылают пустой объект
            // так мы понимаем что запрос точно прошёл
            var obj = JsonSerializer.Deserialize<AjaxRefreshResponse>(response.Data!, Steam.JsonOptions)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    public static bool login_settoken(DefaultRequest defaultRequest, string login_settoken_url)
    {
        var request = new GetRequest(login_settoken_url)
        {
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
            Session = defaultRequest.Session,
        };
        var response = Downloader.Get(request);
        if (!response.Success)
            return new();
        return response.Success || response.StatusCode == 302;
    }
    public static async Task<bool> login_settoken_async(DefaultRequest defaultRequest, string login_settoken_url)
    {
        var request = new GetRequest(login_settoken_url)
        {
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
            Session = defaultRequest.Session,
        };
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        return response.Success || response.StatusCode == 302;
    }

    public static AjaxSetTokenResponse login_settoken(DefaultRequest defaultRequest, AjaxRefreshResponse jwtRefresh, string prior)
	{
        if (!jwtRefresh.Success)
            return new() { Result = EResult.InvalidParam };

		var request = new PostRequest(jwtRefresh.LoginUrl, Downloader.AppFormUrlEncoded)
		{
			Proxy = defaultRequest.Proxy,
			IsAjax = true,
			CancellationToken = defaultRequest.CancellationToken,
			Session = defaultRequest.Session,
		};
		request.AddPostData("success", true).AddPostData("login_url", Uri.EscapeDataString(jwtRefresh.LoginUrl!), false)
            .AddPostData("steamID", jwtRefresh.SteamId).AddPostData("nonce", jwtRefresh.Nonce!, false)
            .AddPostData("redir", Uri.EscapeDataString(jwtRefresh.Redir!), false).AddPostData("auth", jwtRefresh.Auth!, false)
            .AddPostData("prior", prior, false);
		var response = Downloader.Post(request);
		if (!response.Success)
			return new();
		try
		{
			var obj = JsonSerializer.Deserialize<AjaxSetTokenResponse>(response.Data!, Steam.JsonOptions)!;
			return obj;
		}
		catch (Exception)
		{
			return new();
		}
    }
    public static async Task<AjaxSetTokenResponse> login_settoken_async(DefaultRequest defaultRequest, AjaxRefreshResponse jwtRefresh, string prior)
    {
        if (!jwtRefresh.Success)
            return new() { Result = EResult.InvalidParam };

        var request = new PostRequest(jwtRefresh.LoginUrl, Downloader.AppFormUrlEncoded)
        {
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
            Session = defaultRequest.Session,
        };
        request.AddPostData("success", true).AddPostData("login_url", Uri.EscapeDataString(jwtRefresh.LoginUrl!), false)
            .AddPostData("steamID", jwtRefresh.SteamId).AddPostData("nonce", jwtRefresh.Nonce!, false)
            .AddPostData("redir", Uri.EscapeDataString(jwtRefresh.Redir!), false).AddPostData("auth", jwtRefresh.Auth!, false)
            .AddPostData("prior", prior, false);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<AjaxSetTokenResponse>(response.Data!, Steam.JsonOptions)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

	/// <summary>
	/// Обновляет истёкший steaLoginSecure на новый
	/// </summary>
	/// <param name="steamRefresh_steam">Не истёкшая кука steamRefresh_steam с login.steampowered.com</param>
	/// <param name="redir">Для обновления в магазине steam нужно использовать поддомен store, а для поддержки help, также и с поддоменом checkout</param>
	/// <returns>Данные о выполнении запроса. Новый токен находится в <see cref="Auth.v2.Models.SessionData.AccessToken"/></returns>
	public static (EResult, string?) jwt_refresh(DefaultRequest defaultRequest, string steamRefresh_steam, string redir = "https://store.steampowered.com/")
	{
		if (!steamRefresh_steam.StartsWith('7'))
		{
			var jwt = JwtData.Deserialize(steamRefresh_steam);
			steamRefresh_steam = jwt.Subject + "%7C%7C" + steamRefresh_steam;
		}
        var request = new GetRequest(SteamPoweredUrls.Jwt_Refresh)
		{
			Proxy = defaultRequest.Proxy,
			IsAjax = false,
			CancellationToken = defaultRequest.CancellationToken,
			SteamRefresh_Steam = steamRefresh_steam,
            Session = defaultRequest.Session,
            MaxRedirects = 0,
		};
		request.AddQuery("redir", Uri.EscapeDataString(redir!), false);
		var response = Downloader.Get(request);
        if (!response.Location.IsEmpty())
            return (EResult.OK, response.Location);
        if (!response.Success)
            return (EResult.BadResponse, response.Location);
        return (EResult.Expired, response.Location);
    }
	/// <summary>
	/// Обновляет истёкший steaLoginSecure на новый
	/// </summary>
	/// <param name="steamRefresh_steam">Не истёкшая кука steamRefresh_steam с login.steampowered.com</param>
	/// <param name="redir">Для обновления в магазине steam нужно использовать поддомен store, а для поддержки help, также и с поддоменом checkout</param>
	/// <returns>Данные о выполнении запроса. Новый токен находится в <see cref="SteamWeb.Auth.v2.Models.SessionData.AccessToken"/></returns>
	public static async Task<AjaxSetTokenResponse> jwt_refresh_async(DefaultRequest defaultRequest, string steamRefresh_steam, string redir = "https://store.steampowered.com/")
	{
		if (!steamRefresh_steam.StartsWith('7'))
		{
			var jwt = JwtData.Deserialize(steamRefresh_steam);
			steamRefresh_steam = jwt.Subject + "%7C%7C" + steamRefresh_steam;
		}
		var request = new PostRequest(SteamPoweredUrls.Jwt_Refresh, Downloader.AppFormUrlEncoded)
		{
			Proxy = defaultRequest.Proxy,
			IsAjax = false,
			CancellationToken = defaultRequest.CancellationToken,
			SteamRefresh_Steam = steamRefresh_steam,
			Session = defaultRequest.Session,
		};
		request.AddPostData("redir", Uri.EscapeDataString(redir!), false);
		var response = await Downloader.PostAsync(request);
		if (!response.Success)
			return new();
		try
		{
			var obj = JsonSerializer.Deserialize<AjaxSetTokenResponse>(response.Data!, Steam.JsonOptions)!;
			return obj;
		}
		catch (Exception)
		{
			return new() { Result = EResult.BadResponse };
		}
	}
	#endregion

	#region trade offer
	/// <summary>
	/// Принимает трейд
	/// </summary>
	/// <param name="tradeofferid">id трейда для принятия</param>
	/// <param name="steamid_other">steamid32 аккаунта, который отослал трейд</param>
	/// <returns>False трейд не принят, но иногда может быть принят, для точности используйте API</returns>
	public static (ConfTradeOffer?, SteamTradeError?) tradeoffer_accept(DefaultRequest ajaxRequest, ulong tradeofferid, uint steamid_other)
        => tradeoffer_accept(ajaxRequest, tradeofferid, steamid_other.ToSteamId64());
    /// <summary>
    /// Принимает трейд
    /// </summary>
    /// <param name="tradeofferid">id трейда для принятия</param>
    /// <param name="steamid_other">steamid32 аккаунта, который отослал трейд</param>
    /// <returns>False трейд не принят, но иногда может быть принят, для точности используйте API</returns>
    public static (ConfTradeOffer?, SteamTradeError?) tradeoffer_accept(DefaultRequest ajaxRequest, ulong tradeofferid, ulong steamid_other)
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
        .AddPostData("partner", steamid_other).AddPostData("captcha", string.Empty);
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
                var conf = JsonSerializer.Deserialize<ConfTradeOffer>(response.Data!, Steam.JsonOptions);
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
    public static (ConfTradeOffer?, SteamTradeError?) tradeoffer_accept(DefaultRequest ajaxRequest, Trade trade) =>
        tradeoffer_accept(ajaxRequest, trade.TradeOfferId, trade.AccountIdOther);
    /// <summary>
    /// Принимает трейд
    /// </summary>
    /// <param name="tradeofferid">id трейда для принятия</param>
    /// <param name="steamid_other">steamid32 аккаунта, который отослал трейд</param>
    /// <returns>False трейд не принят, но иногда может быть принят, для точности используйте API</returns>
    public static async Task<(ConfTradeOffer?, SteamTradeError?)> tradeoffer_accept_async(DefaultRequest ajaxRequest, ulong tradeofferid, uint steamid_other)
        => await tradeoffer_accept_async(ajaxRequest, tradeofferid, steamid_other.ToSteamId64());
    /// <summary>
    /// Принимает трейд
    /// </summary>
    /// <param name="tradeofferid">id трейда для принятия</param>
    /// <param name="steamid64">steamid64 аккаунта, который отослал трейд</param>
    /// <returns>False трейд не принят, но иногда может быть принят, для точности используйте API</returns>
    public static async Task<(ConfTradeOffer?, SteamTradeError?)> tradeoffer_accept_async(DefaultRequest ajaxRequest, ulong tradeofferid, ulong steamid64)
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
        .AddPostData("partner", steamid64).AddPostData("captcha", string.Empty);
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
                var conf = JsonSerializer.Deserialize<ConfTradeOffer>(response.Data!, Steam.JsonOptions);
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
    /// Отменяет трейд, который этот аккаунт создал
    /// </summary>
    /// <param name="tradeofferid">trade offer id</param>
    public static async Task<CancelTrade> tradeoffer_cancel_async(DefaultRequest defaultRequest, ulong tradeofferid)
    {
        string url = $"https://steamcommunity.com/tradeoffer/{tradeofferid}/cancel";
        var request = new PostRequest(url, Downloader.AppFormUrlEncoded)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
        };
        request.AddPostData("sessionid", defaultRequest.Session!.SessionID);
        var response = await Downloader.PostAsync(request);
		if (response.Data == null)
			return new();
		try
		{
			var options = new JsonSerializerOptions
			{
				NumberHandling = JsonNumberHandling.AllowReadingFromString
			};
			var obj = JsonSerializer.Deserialize<CancelTrade>(response.Data!, options)!;
			if (obj.TradeOfferId != 0)
				obj.Success = 1;
			return obj;
		}
		catch (Exception)
		{
			return new();
		}
	}
	/// <summary>
	/// Отменяет трейд, который этот аккаунт создал
	/// </summary>
	/// <param name="trade">trade offer id</param>
	public static async Task<CancelTrade> tradeoffer_cancel_async(DefaultRequest defaultRequest, Trade trade)
        => await tradeoffer_cancel_async(defaultRequest, trade.TradeOfferId);
	/// <summary>
	/// Отменяет трейд, который этот аккаунт создал
	/// </summary>
	/// <param name="tradeofferid">trade offer id</param>
	public static CancelTrade tradeoffer_cancel(DefaultRequest defaultRequest, ulong tradeofferid)
    {
        string url = $"https://steamcommunity.com/tradeoffer/{tradeofferid}/cancel";
        var request = new PostRequest(url, Downloader.AppFormUrlEncoded)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
		};
        request.AddPostData("sessionid", defaultRequest.Session!.SessionID);
        var response = Downloader.Post(request);
		if (response.Data == null)
			return new();
		try
		{
			var options = new JsonSerializerOptions
			{
				NumberHandling = JsonNumberHandling.AllowReadingFromString
			};
			var obj = JsonSerializer.Deserialize<CancelTrade>(response.Data!, options)!;
			if (obj.TradeOfferId != 0)
				obj.Success = 1;
			return obj;
		}
		catch (Exception)
		{
			return new();
		}
	}
	/// <summary>
	/// Отменяет трейд, который этот аккаунт создал
	/// </summary>
	/// <param name="trade">trade offer id</param>
	public static CancelTrade tradeoffer_cancel(DefaultRequest defaultRequest, Trade trade)
        => tradeoffer_cancel(defaultRequest, trade.TradeOfferId);

	/// <summary>
	/// Отклоняет трейд, который был отправлен на этот аккаунт
	/// </summary>
	/// <param name="tradeofferid">trade offer id</param>
	public static async Task<CancelTrade> tradeoffer_decline_async(DefaultRequest defaultRequest, ulong tradeofferid)
	{
		string url = $"https://steamcommunity.com/tradeoffer/{tradeofferid}/decline";
		var request = new PostRequest(url, Downloader.AppFormUrlEncoded)
		{
			Session = defaultRequest.Session,
			Proxy = defaultRequest.Proxy,
			IsAjax = true,
			CancellationToken = defaultRequest.CancellationToken,
		};
		request.AddPostData("sessionid", defaultRequest.Session!.SessionID);
		var response = await Downloader.PostAsync(request);
		if (response.Data == null)
			return new();
		try
		{
			var options = new JsonSerializerOptions
			{
				NumberHandling = JsonNumberHandling.AllowReadingFromString
			};
			var obj = JsonSerializer.Deserialize<CancelTrade>(response.Data!, options)!;
			if (obj.TradeOfferId != 0)
				obj.Success = 1;
			return obj;
		}
		catch (Exception)
		{
			return new();
		}
	}
	/// <summary>
	/// Отклоняет трейд, который был отправлен на этот аккаунт
	/// </summary>
	/// <param name="trade">trade offer id</param>
	public static async Task<CancelTrade> tradeoffer_decline_async(DefaultRequest defaultRequest, Trade trade)
		=> await tradeoffer_decline_async(defaultRequest, trade.TradeOfferId);
	/// <summary>
	/// Отклоняет трейд, который был отправлен на этот аккаунт
	/// </summary>
	/// <param name="tradeofferid">trade offer id</param>
	public static CancelTrade tradeoffer_decline(DefaultRequest defaultRequest, ulong tradeofferid)
	{
		string url = $"https://steamcommunity.com/tradeoffer/{tradeofferid}/decline";
		var request = new PostRequest(url, Downloader.AppFormUrlEncoded)
		{
			Session = defaultRequest.Session,
			Proxy = defaultRequest.Proxy,
			IsAjax = true,
			CancellationToken = defaultRequest.CancellationToken,
		};
		request.AddPostData("sessionid", defaultRequest.Session!.SessionID);
		var response = Downloader.Post(request);
		if (response.Data == null)
			return new();
		try
		{
			var options = new JsonSerializerOptions
			{
				NumberHandling = JsonNumberHandling.AllowReadingFromString
			};
			var obj = JsonSerializer.Deserialize<CancelTrade>(response.Data!, options)!;
			if (obj.TradeOfferId != 0)
				obj.Success = 1;
			return obj;
		}
		catch (Exception)
		{
			return new();
		}
	}
	/// <summary>
	/// Отклоняет трейд, который был отправлен на этот аккаунт
	/// </summary>
	/// <param name="trade">trade offer id</param>
	public static CancelTrade tradeoffer_decline(DefaultRequest defaultRequest, Trade trade)
		=> tradeoffer_decline(defaultRequest, trade.TradeOfferId);
	#endregion

	#region market
	public static ResponseSuccess market_cancelbuyorder(DefaultRequest defaultRequest, ulong buy_orderid)
	{
		var request = new PostRequest(SteamCommunityUrls.Market_CancelBuyOrder, Downloader.AppFormUrlEncoded)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
            Referer = SteamCommunityUrls.Market,
		};
		request.AddPostData("sessionid", defaultRequest.Session!.SessionID);
        request.AddPostData("buy_orderid", buy_orderid);
        request.AddHeader("X-Prototype-Version", "1.7");
		var response = Downloader.Post(request);
		if (!response.Success)
			return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseSuccess>(response.Data!)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
	public static async Task<ResponseSuccess> market_cancelbuyorder_async(DefaultRequest defaultRequest, ulong buy_orderid)
	{
		var request = new PostRequest(SteamCommunityUrls.Market_CancelBuyOrder, Downloader.AppFormUrlEncoded)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
            Referer = SteamCommunityUrls.Market,
		};
		request.AddPostData("sessionid", defaultRequest.Session!.SessionID);
		request.AddPostData("buy_orderid", buy_orderid);
		request.AddHeader("X-Prototype-Version", "1.7");
		var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseSuccess>(response.Data!)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

	public static DataOrder market_createbuyorder(DefaultRequest defaultRequest, byte currency, uint appid, string market_hash_name, int price_total, ushort quantity, int tradefee_tax)
	{
		var request = new PostRequest(SteamCommunityUrls.Market_CreateBuyOrder, Downloader.AppFormUrlEncoded)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
            Referer = SteamCommunityUrls.Market_Listings + "/" + appid + "/" + Uri.EscapeDataString(market_hash_name),
            UseVersion2 = true
		};
		request.AddPostData("sessionid", defaultRequest.Session!.SessionID);
		request.AddPostData("currency", currency);
		request.AddPostData("appid", appid);
		request.AddPostData("market_hash_name", Uri.EscapeDataString(market_hash_name), false);
		request.AddPostData("price_total", price_total);
		request.AddPostData("tradefee_tax", tradefee_tax);
        request.AddPostData("quantity", quantity);
        request.AddPostData("billing_state", string.Empty);
		request.AddPostData("save_my_address", 0);
		var response = Downloader.Post(request);
		if (!response.Success)
			return new();
		try
		{
			var obj = JsonSerializer.Deserialize<DataOrder>(response.Data!, Steam.JsonOptions);
			return obj!;
		}
		catch (Exception)
		{
			return new();
		}
	}
	public static async Task<DataOrder> market_createbuyorder_async(DefaultRequest defaultRequest, int currency, uint appid, string market_hash_name, int price_total, ushort quantity, int tradefee_tax)
	{
		var request = new PostRequest(SteamCommunityUrls.Market_CreateBuyOrder, Downloader.AppFormUrlEncoded)
		{
			Session = defaultRequest.Session,
			Proxy = defaultRequest.Proxy,
			IsAjax = true,
			CancellationToken = defaultRequest.CancellationToken,
			Referer = SteamCommunityUrls.Market_Listings + "/" + appid + "/" + Uri.EscapeDataString(market_hash_name),
			UseVersion2 = true
		};
        request.AddPostData("sessionid", defaultRequest.Session!.SessionID);
        request.AddPostData("currency", currency);
        request.AddPostData("appid", appid);
        request.AddPostData("market_hash_name", Uri.EscapeDataString(market_hash_name), false);
        request.AddPostData("price_total", price_total);
        request.AddPostData("tradefee_tax", tradefee_tax);
        request.AddPostData("quantity", quantity);
        request.AddPostData("billing_state", string.Empty);
        request.AddPostData("save_my_address", 0);
        var response = await Downloader.PostAsync(request);
		if (!response.Success)
			return new();
		try
		{
            var options = new JsonSerializerOptions
            {
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
            };
			var obj = JsonSerializer.Deserialize<DataOrder>(response.Data!, options);
			return obj!;
		}
		catch (Exception)
		{
			return new();
		}
	}

    public static async Task<MarketSearchResponse> market_search_render_async(DefaultRequest defaultRequest, MarketSearchRequest request)
    {
        var getRequest = new GetRequest(SteamCommunityUrls.Market_Search_Render)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
            Timeout = 90000,
        };
        if (!request.Query.IsEmpty())
            getRequest.AddQuery("query", request.Query);
        getRequest.AddQuery("start", request.Offset).AddQuery("count", request.Limit).AddQuery("search_descriptions", request.SearchDescriptions)
            .AddQuery("norender", request.NoRender).AddQuery("sort_column", request.SortColumn.ToString().ToLower())
            .AddQuery("sort_dir", request.SortDir.ToString().ToLower()).AddQuery("appid", request.AppId);
        if (request.Category730Types.Count > 0 || request.Category730Weapons.Count > 0)
        {
            getRequest.AddQuery("category_730_ItemSet[]", "any").AddQuery("category_730_ProPlayer[]", "any");
            getRequest.AddQuery("category_730_StickerCapsule[]", "any").AddQuery("category_730_TournamentTeam[]", "any");
            if (request.Category730Weapons.Count == 0)
                getRequest.AddQuery("category_730_TournamentTeam[]", "any");
            if (request.Category730Types.Count == 0)
                getRequest.AddQuery("category_730_Type[]", "any");
            for (int i = 0; i < request.Category730Types.Count; i++)
                getRequest.AddQuery("category_730_Type[]", request.Category730Types[i].ToString());
            for (int i = 0; i < request.Category730Weapons.Count; i++)
                getRequest.AddQuery("category_730_Weapon[]", request.Category730Weapons[i].ToString());
        }
        var response = await Downloader.GetAsync(getRequest);
        if (!response.Success)
			return new() { IsTooManyRequests = response.StatusCode == 429 };
		try
        {
            var obj = JsonSerializer.Deserialize<MarketSearchResponse>(response.Data!)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    public static MarketSearchResponse market_search_render(DefaultRequest defaultRequest, MarketSearchRequest request)
    {
        var getRequest = new GetRequest(SteamCommunityUrls.Market_Search_Render)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
            Timeout = 90000,
        };
        if (!request.Query.IsEmpty())
            getRequest.AddQuery("query", request.Query);
        getRequest.AddQuery("start", request.Offset).AddQuery("count", request.Limit).AddQuery("search_descriptions", request.SearchDescriptions)
            .AddQuery("norender", request.NoRender).AddQuery("sort_column", request.SortColumn.ToString().ToLower())
            .AddQuery("sort_dir", request.SortDir.ToString().ToLower()).AddQuery("appid", request.AppId);
        if (request.Category730Types.Count > 0 || request.Category730Weapons.Count > 0)
        {
            getRequest.AddQuery("category_730_ItemSet[]", "any").AddQuery("category_730_ProPlayer[]", "any");
            getRequest.AddQuery("category_730_StickerCapsule[]", "any").AddQuery("category_730_TournamentTeam[]", "any");
            if (request.Category730Weapons.Count == 0)
                getRequest.AddQuery("category_730_TournamentTeam[]", "any");
            if (request.Category730Types.Count == 0)
                getRequest.AddQuery("category_730_Type[]", "any");
            for (int i = 0; i < request.Category730Types.Count; i++)
                getRequest.AddQuery("category_730_Type[]", request.Category730Types[i].ToString());
            for (int i = 0; i < request.Category730Weapons.Count; i++)
                getRequest.AddQuery("category_730_Weapon[]", request.Category730Weapons[i].ToString());
        }
        var response = Downloader.Get(getRequest);
        if (!response.Success)
            return new() { IsTooManyRequests = response.StatusCode == 429 };
        try
        {
            var obj = JsonSerializer.Deserialize<MarketSearchResponse>(response.Data!)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    /// <summary>
    /// Выполняет запрос на получение предметов в листинге и ордеров
    /// </summary>
    /// <exception cref="RegexMatchTimeoutException"/>
    /// <exception cref="InvalidOperationException"/>
    public static async Task<Listing> market_mylistings_async(DefaultRequest defaultRequest, int count = 100, int start = 0)
    {
        var request = new GetRequest(SteamCommunityUrls.Market_MyListings)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
            Referer = SteamCommunityUrls.Market,
		};
        request.AddQuery("count", count);
        if (start > 0)
            request.AddQuery("start", start);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        return Listing.Deserialize(response.Data!);
    }
    /// <summary>
    /// Выполняет запрос на получение предметов в листинге и ордеров
    /// </summary>
    /// <exception cref="RegexMatchTimeoutException"/>
    /// <exception cref="InvalidOperationException"/>
    public static Listing market_mylistings(DefaultRequest defaultRequest, int count = 100, int start = 0)
    {
        var request = new GetRequest(SteamCommunityUrls.Market_MyListings)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
            Referer = SteamCommunityUrls.Market,
		};
        request.AddQuery("count", count);
        if (start > 0)
            request.AddQuery("start", start);
        var response = Downloader.Get(request);
        if (!response.Success)
            return new();
        return Listing.Deserialize(response.Data!);
	}

	/// <exception cref="JsonException"/>
	/// <exception cref="NotSupportedException"/>
	public static ItemRenderResponse market_listings_render(ItemRenderRequest renderRequest)
	{
        var url = Path.Join(SteamCommunityUrls.Market_Listings, renderRequest.AppId.ToString(), Uri.EscapeDataString(renderRequest.MarketHashName), "render");
		var request = new GetRequest(url)
		{
			Session = renderRequest.DefaultRequest.Session,
			Proxy = renderRequest.DefaultRequest.Proxy,
			IsAjax = true,
			CancellationToken = renderRequest.DefaultRequest.CancellationToken,
			Referer = SteamCommunityUrls.Market,
		};
		if (!renderRequest.Query.IsEmpty())
			request.AddQuery("query", Uri.EscapeDataString(renderRequest.Query!), false);
		if (!renderRequest.Country.IsEmpty())
			request.AddQuery("country", Uri.EscapeDataString(renderRequest.Country!), false);
		if (!renderRequest.Language.IsEmpty())
			request.AddQuery("language", Uri.EscapeDataString(renderRequest.Language!), false);
		request.AddQuery("count", renderRequest.Count);
		request.AddQuery("start", renderRequest.Start);
		request.AddQuery("currency", renderRequest.Currency);

		var response = Downloader.Get(request);
		if (!response.Success)
			return new();

		var obj = JsonSerializer.Deserialize<ItemRenderResponse>(response.Data!, Steam.JsonOptions)!;
		return obj;
	}
	/// <exception cref="JsonException"/>
	/// <exception cref="NotSupportedException"/>
	public static async Task<ItemRenderResponse> market_listings_render_async(ItemRenderRequest renderRequest)
	{
		var url = Path.Join(SteamCommunityUrls.Market_Listings, renderRequest.AppId.ToString(), Uri.EscapeDataString(renderRequest.MarketHashName), "render");
		var request = new GetRequest(url)
		{
			Session = renderRequest.DefaultRequest.Session,
			Proxy = renderRequest.DefaultRequest.Proxy,
			IsAjax = true,
			CancellationToken = renderRequest.DefaultRequest.CancellationToken,
			Referer = SteamCommunityUrls.Market,
		};
		if (!renderRequest.Query.IsEmpty())
			request.AddQuery("query", Uri.EscapeDataString(renderRequest.Query!), false);
		if (!renderRequest.Country.IsEmpty())
			request.AddQuery("country", Uri.EscapeDataString(renderRequest.Country!), false);
		if (!renderRequest.Language.IsEmpty())
			request.AddQuery("language", Uri.EscapeDataString(renderRequest.Language!), false);
		request.AddQuery("count", renderRequest.Count);
		request.AddQuery("start", renderRequest.Start);
		request.AddQuery("currency", renderRequest.Currency);

		var response = await Downloader.GetAsync(request);
		if (!response.Success)
			return new();

        var obj = JsonSerializer.Deserialize<ItemRenderResponse>(response.Data!, Steam.JsonOptions)!;
		return obj;
	}

	/// <exception cref="JsonException"/>
	/// <exception cref="NotSupportedException"/>
	public static WalletInfo market_buylisting(DefaultRequest defaultRequest, ItemRenderListing renderRequest, ushort quantity = 1, string? billing_state = null, bool save_my_address = false)
	{
		var request = new PostRequest(SteamCommunityUrls.Market_BuyListing + renderRequest.ListingId, Downloader.AppFormUrlEncoded)
		{
			Session = defaultRequest.Session,
			Proxy = defaultRequest.Proxy,
			IsAjax = true,
			CancellationToken = defaultRequest.CancellationToken,
			Referer = SteamCommunityUrls.Market_Listings + "/" + renderRequest.PublisherFeeApp + "/",
		};
		request.AddPostData("sessionid", defaultRequest.Session!.SessionID, false)
            .AddPostData("currency", renderRequest.CurrencyId - 2000)
            .AddPostData("subtotal", renderRequest.Price)
            .AddPostData("fee", renderRequest.Fee)
            .AddPostData("total", renderRequest.TotalPrice)
			.AddPostData("quantity", quantity)
            .AddPostData("save_my_address", save_my_address ? 1 : 0)
            .AddPostData("billing_state", !billing_state.IsEmpty() ? Uri.EscapeDataString(billing_state!) : string.Empty, false);

        var response = Downloader.Post(request);
		if (!response.Success)
			return new();

		var obj = JsonSerializer.Deserialize<WalletInfo>(response.Data!, Steam.JsonOptions)!;
		return obj;
	}
	/// <exception cref="JsonException"/>
	/// <exception cref="NotSupportedException"/>
	public static async Task<WalletInfo> market_buylisting_async(DefaultRequest defaultRequest, ItemRenderListing renderRequest, ushort quantity = 1, string? billing_state = null, bool save_my_address = false)
	{
		var request = new PostRequest(SteamCommunityUrls.Market_BuyListing + renderRequest.ListingId, Downloader.AppFormUrlEncoded)
		{
			Session = defaultRequest.Session,
			Proxy = defaultRequest.Proxy,
			IsAjax = true,
			CancellationToken = defaultRequest.CancellationToken,
			Referer = SteamCommunityUrls.Market_Listings + "/" + renderRequest.PublisherFeeApp + "/",
		};
        request.AddPostData("sessionid", defaultRequest.Session!.SessionID, false)
            .AddPostData("currency", renderRequest.CurrencyId - 2000)
            .AddPostData("subtotal", renderRequest.Price)
            .AddPostData("fee", renderRequest.Fee)
            .AddPostData("total", renderRequest.TotalPrice)
            .AddPostData("quantity", quantity)
            .AddPostData("save_my_address", save_my_address ? 1 : 0)
            .AddPostData("billing_state", !billing_state.IsEmpty() ? Uri.EscapeDataString(billing_state!) : string.Empty, false);

        var response = await Downloader.PostAsync(request);
		if (!response.Success)
			return new();

		var obj = JsonSerializer.Deserialize<WalletInfo>(response.Data!, Steam.JsonOptions)!;
		return obj;
	}

	/// <exception cref="JsonException"/>
	/// <exception cref="NotSupportedException"/>
	public static BuyOrderStatusResponse market_getbuyorderstatus(DefaultRequest defaultRequest, ulong buy_orderid)
	{
		var request = new GetRequest(SteamCommunityUrls.Market_GetBuyOrderStatus)
		{
			Session = defaultRequest.Session,
			Proxy = defaultRequest.Proxy,
			IsAjax = true,
			CancellationToken = defaultRequest.CancellationToken,
			Referer = SteamCommunityUrls.Market_Listings,
		};
		request.AddQuery("sessionid", defaultRequest.Session!.SessionID, false).AddQuery("buy_orderid", buy_orderid);

		var response = Downloader.Get(request);
		if (!response.Success)
			return new();

		var obj = JsonSerializer.Deserialize<BuyOrderStatusResponse>(response.Data!, Steam.JsonOptions)!;
		return obj;
	}
	/// <exception cref="JsonException"/>
	/// <exception cref="NotSupportedException"/>
	public static async Task<BuyOrderStatusResponse> market_getbuyorderstatus_async(DefaultRequest defaultRequest, ulong buy_orderid)
	{
		var request = new GetRequest(SteamCommunityUrls.Market_GetBuyOrderStatus)
		{
			Session = defaultRequest.Session,
			Proxy = defaultRequest.Proxy,
			IsAjax = true,
			CancellationToken = defaultRequest.CancellationToken,
			Referer = SteamCommunityUrls.Market_Listings,
		};
		request.AddQuery("sessionid", defaultRequest.Session!.SessionID, false).AddQuery("buy_orderid", buy_orderid);

		var response = await Downloader.GetAsync(request);
		if (!response.Success)
			return new();

		var obj = JsonSerializer.Deserialize<BuyOrderStatusResponse>(response.Data!, Steam.JsonOptions)!;
		return obj;
	}

	public static async Task<Historing> market_myhistory_async(DefaultRequest defaultRequest, int start = 0, int count = 200)
    {
        // count - При любом значении, присылают не все данные
        var request = new GetRequest(SteamCommunityUrls.Market_MyHistory_Render)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            CancellationToken = defaultRequest.CancellationToken,
            Referer = SteamCommunityUrls.Market,
        };
        request.AddQuery("start", start).AddQuery("count", count);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new Historing() { IsError = true };
        else if (response.Data!.Contains("There was an error loading your market history. Please try again later."))
            return new Historing() { IsError = true };
        else if (response.Data.Contains("<a href=\\\"https:\\/\\/steamcommunity.com\\/login\\/home\\/?goto=market%2F%2Frender%2F%3Fstart%3D0%26count%3D10\\\">Login<\\/a> to view your Community Market history."))
            return new Historing() { IsAuthtorized = false };
        return Historing.Deserialize(response.Data);
    }
    public static Historing market_myhistory(DefaultRequest defaultRequest, int start = 0, int count = 200)
    {
		// count - При любом значении, присылают не все данные
		var request = new GetRequest(SteamCommunityUrls.Market_MyHistory_Render)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            CancellationToken = defaultRequest.CancellationToken,
            Referer = SteamCommunityUrls.Market,
        };
		request.AddQuery("start", start).AddQuery("count", count);
		var response = Downloader.Get(request);
        if (!response.Success)
            return new Historing() { IsError = true };
        else if (response.Data!.Contains("There was an error loading your market history. Please try again later."))
            return new Historing() { IsError = true };
        else if (response.Data.Contains("<a href=\\\"https:\\/\\/steamcommunity.com\\/login\\/home\\/?goto=market%2F%2Frender%2F%3Fstart%3D0%26count%3D10\\\">Login<\\/a> to view your Community Market history."))
            return new Historing() { IsAuthtorized = false };
        return Historing.Deserialize(response.Data);
    }

    public static async Task<SellItem> market_sellitem_async(DefaultRequest defaultRequest, uint appid, ulong contextid, ulong assetid, uint amount, uint price)
    {
        var request = new PostRequest(SteamCommunityUrls.Market_SellItem, Downloader.AppFormUrlEncoded)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
            Referer = $"https://steamcommunity.com/profiles/{defaultRequest.Session!.SteamID}/inventory/",
		};
        request.AddPostData("sessionid", defaultRequest.Session!.SessionID).AddPostData("appid", appid).AddPostData("contextid", contextid)
            .AddPostData("assetid", assetid).AddPostData("amount", amount).AddPostData("price", price);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new SellItem();
        var obj = JsonSerializer.Deserialize<SellItem>(response.Data!)!;
        return obj;
    }
    public static SellItem market_sellitem(DefaultRequest defaultRequest, uint appid, ulong contextid, ulong assetid, uint amount, uint price)
    {
        var request = new PostRequest(SteamCommunityUrls.Market_SellItem, Downloader.AppFormUrlEncoded)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
            Referer = $"https://steamcommunity.com/profiles/{defaultRequest.Session!.SteamID}/inventory/",
        };
        request.AddPostData("sessionid", defaultRequest.Session!.SessionID).AddPostData("appid", appid).AddPostData("contextid", contextid)
            .AddPostData("assetid", assetid).AddPostData("amount", amount).AddPostData("price", price);
        var response = Downloader.Post(request);
        if (!response.Success)
            return new SellItem();
        var obj = JsonSerializer.Deserialize<SellItem>(response.Data!)!;
        return obj;
    }

    public static async Task<bool> market_removelisting_async(DefaultRequest defaultRequest, ulong id, string market_hash_name, uint appid)
    {
        var request = new PostRequest(SteamCommunityUrls.Market_RemoveListing + id, Downloader.AppFormUrlEncoded)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
            Referer = $"https://steamcommunity.com/market/listings/{appid}/" + Uri.EscapeDataString(market_hash_name),
		};
        request.AddPostData("sessionid", defaultRequest.Session!.SessionID);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return false;
        else if (response.Data == "[]")
            return true;
        return false;
    }
    public static bool market_removelisting(DefaultRequest defaultRequest, ulong id, string market_hash_name, uint appid)
    {
        var request = new PostRequest(SteamCommunityUrls.Market_RemoveListing + id, Downloader.AppFormUrlEncoded)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
            Referer = $"https://steamcommunity.com/market/listings/{appid}/" + Uri.EscapeDataString(market_hash_name),
        };
        request.AddPostData("sessionid", defaultRequest.Session!.SessionID);
        var response = Downloader.Post(request);
        if (!response.Success)
            return false;
        else if (response.Data == "[]")
            return true;
        return false;
    }

    public static async Task<OrderHistogram> market_itemordershistogram_async(OrderHistogramRequest request)
    {
        var getRequest = new GetRequest(SteamCommunityUrls.Market_ItemOrdersHistogram, request.Proxy, request.Session)
        {
            Referer = $"https://steamcommunity.com/market/listings/{request.AppId}/{Uri.EscapeDataString(request.MarketHashName)}",
            IsAjax = true,
            CancellationToken = request.CancellationToken
		};
        getRequest.AddQuery("country", request.Country).AddQuery("language", request.Language).AddQuery("currency", request.Currency)
            .AddQuery("item_nameid", request.Item_NameId).AddQuery("two_factor", request.TwoFactor);
        if (request.Timeout > 0)
            getRequest.Timeout = request.Timeout;
        var response = await Downloader.GetAsync(getRequest);
        if (!response.Success)
            return new() { E502L3 = response.StatusCode == 429, InternalError = response.StatusCode == 0 };
        var obj = JsonSerializer.Deserialize<OrderHistogram>(response.Data!)!;
        return obj;
    }
    public static OrderHistogram market_itemordershistogram(OrderHistogramRequest request)
    {
        var getRequest = new GetRequest(SteamCommunityUrls.Market_ItemOrdersHistogram, request.Proxy, request.Session)
        {
            Referer = $"https://steamcommunity.com/market/listings/{request.AppId}/{Uri.EscapeDataString(request.MarketHashName)}",
            IsAjax = true,
			CancellationToken = request.CancellationToken
		};
        getRequest.AddQuery("country", request.Country).AddQuery("language", request.Language).AddQuery("currency", request.Currency)
            .AddQuery("item_nameid", request.Item_NameId).AddQuery("two_factor", request.TwoFactor);
        if (request.Timeout > 0)
            getRequest.Timeout = request.Timeout;
        var response = Downloader.Get(getRequest);
        if (!response.Success)
            return new() { E502L3 = response.StatusCode == 429, InternalError = response.StatusCode == 0 };
        var obj = JsonSerializer.Deserialize<OrderHistogram>(response.Data!)!;
        return obj;
    }

    public static async Task<PriceHistory> market_pricehistory_async(DefaultRequest defaultRequest, int appid, string market_hash_name)
    {
        market_hash_name = market_hash_name.Replace("%27", "'").Replace("?", "%3F").Replace("%E2%98%85", "★").Replace("%E2%84%A2", "™");
        var referer = defaultRequest.Session == null ?
            SteamCommunityUrls.My_Inventory :
            $"https://steamcommunity.com/profiles/{defaultRequest.Session.SteamID}/inventory/";
        var getRequest = new GetRequest(SteamCommunityUrls.Market_PriceHistory)
        {
            Referer = referer,
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
        };
        getRequest.AddQuery("appid", appid).AddQuery("market_hash_name", market_hash_name);
        var response = await Downloader.GetAsync(getRequest);
        if (!response.Success)
            return new() { e500 = response.StatusCode == 502 || response.StatusCode == 503 };
        return PriceHistory.Deserialize(response.Data!);
    }
    public static PriceHistory market_pricehistory(DefaultRequest defaultRequest, uint appid, string market_hash_name)
    {
        market_hash_name = market_hash_name.Replace("%27", "'").Replace("?", "%3F").Replace("%E2%98%85", "★").Replace("%E2%84%A2", "™");
        var referer = defaultRequest.Session == null ?
            SteamCommunityUrls.My_Inventory :
            $"https://steamcommunity.com/profiles/{defaultRequest.Session.SteamID}/inventory/";
        var getRequest = new GetRequest(SteamCommunityUrls.Market_PriceHistory)
        {
            Referer = referer,
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
        };
        getRequest.AddQuery("appid", appid).AddQuery("market_hash_name", market_hash_name);
        var response = Downloader.Get(getRequest);
        if (!response.Success)
            return new() { e500 = response.StatusCode == 502 || response.StatusCode == 503 };
        return PriceHistory.Deserialize(response.Data!);
    }

    public static async Task<PriceOverview> market_priceoverview_async(DefaultRequest defaultRequest, uint appid, string country, ushort currency, string market_hash_name)
    {
        var request = new GetRequest(SteamCommunityUrls.Market_PriceOverview)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
        };
        request.AddQuery("market_hash_name", market_hash_name).AddQuery("appid", appid).AddQuery("country", country).AddQuery("currency", currency);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<PriceOverview>(response.Data!, Steam.JsonOptions)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    public static PriceOverview market_priceoverview(DefaultRequest defaultRequest, uint appid, string country, ushort currency, string market_hash_name)
    {
        var request = new GetRequest(SteamCommunityUrls.Market_PriceOverview)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
        };
        request.AddQuery("market_hash_name", market_hash_name).AddQuery("appid", appid).AddQuery("country", country).AddQuery("currency", currency);
        var response = Downloader.Get(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<PriceOverview>(response.Data!, Steam.JsonOptions)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    #endregion

    #region actions
    public static async Task<ItemQueryLocations[]> action_QueryLocations_async(DefaultRequest defaultRequest)
    {
        string referer = $"https://steamcommunity.com/profiles/{defaultRequest.Session!.SteamID}/edit/info";
        var request = new GetRequest(SteamCommunityUrls.Actions_QueryLocations)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
            Referer = referer
        };
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return Array.Empty<ItemQueryLocations>();
        var obj = JsonSerializer.Deserialize<ItemQueryLocations[]>(response.Data!)!;
        return obj;
    }
    public static async Task<ItemQueryLocations[]> action_QueryLocations_async(DefaultRequest defaultRequest, ItemQueryLocations Loc)
    {
        string referer = $"https://steamcommunity.com/profiles/{defaultRequest.Session!.SteamID}/edit/info";
        string url = SteamCommunityUrls.Actions_QueryLocations + '/' + Loc.countrycode + (Loc.state_loaded ? Loc.statecode : string.Empty);
        var request = new GetRequest(url)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
            Referer = referer
        };
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return Array.Empty<ItemQueryLocations>();
        var obj = JsonSerializer.Deserialize<ItemQueryLocations[]>(response.Data!)!;
        var length = obj.Length;
        for (int i = 0; i < length; i++)
        {
            var item = obj[i];
            item.countryname = Loc.countryname;
            item.hasstates = Loc.hasstates;
            item.state_loaded = true;
            if (Loc.state_loaded)
            {
                item.statename = Loc.statename;
                item.city_loaded = true;
            }
        }
        return obj;
    }

    /// <summary>
    /// Изменяет язык на steam аккаунте
    /// </summary>
    /// <param name="language">язык для смены, пример: russian, english</param>
    /// <returns>true язык изменён</returns>
    public static async Task<bool> actions_setlanguage_async(DefaultRequest ajaxRequest, string language)
    {
        var request = new PostRequest(SteamCommunityUrls.Actions_SetLanguage, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            CancellationToken = ajaxRequest.CancellationToken,
        }
        .AddPostData("language", language).AddPostData("sessionid", ajaxRequest.Session!.SessionID);
        var response = await Downloader.PostAsync(request);
        return response.Data == "true";
    }
    /// <summary>
    /// Изменяет язык на steam аккаунте
    /// </summary>
    /// <param name="language">язык для смены, пример: russian, english</param>
    /// <returns>true язык изменён</returns>
    public static bool actions_setlanguage(DefaultRequest ajaxRequest, string language)
    {
        var request = new PostRequest(SteamCommunityUrls.Actions_SetLanguage, Downloader.AppFormUrlEncoded)
        {
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
            CancellationToken = ajaxRequest.CancellationToken,
        }
        .AddPostData("language", language).AddPostData("sessionid", ajaxRequest.Session!.SessionID);
        var response = Downloader.Post(request);
        return response.Data == "true";
    }
    #endregion

    #region profile
    public static async Task<ItemGroup[]> profiles_ajaxgroupinvite_async(DefaultRequest defaultRequest)
    {
        string url = $"https://steamcommunity.com/profiles/{defaultRequest.Session!.SteamID}/ajaxgroupinvite?select_primary=1&json=1";
        string referer = $"https://steamcommunity.com/profiles/{defaultRequest.Session!.SteamID}/edit/theme";
        var request = new GetRequest(url)
		{
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
            Referer = referer,
        };
		var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return Array.Empty<ItemGroup>();
        var obj = JsonSerializer.Deserialize<ItemGroup[]>(response.Data!)!;
        return obj;
    }
    public static ItemGroup[] profiles_ajaxgroupinvite(DefaultRequest defaultRequest)
    {
        string url = $"https://steamcommunity.com/profiles/{defaultRequest.Session!.SteamID}/ajaxgroupinvite?select_primary=1&json=1";
        string referer = $"https://steamcommunity.com/profiles/{defaultRequest.Session!.SteamID}/edit/theme";
        var request = new GetRequest(url)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
            Referer = referer,
        };
        var response = Downloader.Get(request);
        if (!response.Success)
            return Array.Empty<ItemGroup>();
        var obj = JsonSerializer.Deserialize<ItemGroup[]>(response.Data!)!;
        return obj;
    }

    /// <summary>
    /// Оставить комментарий в профиле
    /// </summary>
    /// <param name="comment"></param>
    /// <param name="count">Количество комментариев на странице</param>
    /// <param name="feature2"></param>
    /// <returns></returns>
    public static async Task<CommentResponse> profiles_post_comment_async(DefaultRequest defaultRequest, string comment, int count = 6, int feature2 = -1)
        => await profiles_post_comment_async(defaultRequest, comment, defaultRequest.Session!.SteamID, count, feature2);
    /// <summary>
    /// Оставить комментарий в профиле
    /// </summary>
    /// <param name="comment"></param>
    /// <param name="steamid"></param>
    /// <param name="count">Количество комментариев на странице</param>
    /// <param name="feature2"></param>
    /// <returns></returns>
    public static async Task<CommentResponse> profiles_post_comment_async(DefaultRequest defaultRequest, string comment, ulong steamid, int count = 6, int feature2 = -1)
    {
        string url = $"https://steamcommunity.com/comment/Profile/post/{steamid}/-1/";
        string referer = $"https://steamcommunity.com/profiles/{steamid}/";
        var request = new PostRequest(url, Downloader.AppFormUrlEncoded)
        {
            Referer = referer,
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
        };
        request.AddPostData("comment", comment).AddPostData("count", count).AddPostData("sessionid", defaultRequest.Session!.SessionID).AddPostData("feature2", feature2);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<CommentResponse>(response.Data!)!;
            return obj;
        }
        catch (Exception)
        {
            return new CommentResponse() { success = false };
        }
    }
    #endregion

    #region account
    public static async Task<ResponseSuccess> account_ajaxsetcookiepreferences_async(DefaultRequest defaultRequest, CookiePreferences cookiepreferences)
    {
        if (defaultRequest.Session == null)
            return new();
        var request = new PostRequest(SteamPoweredUrls.Account_AjaxSetCookiePreferences, Downloader.AppFormUrlEncoded)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
        };
        request.AddPostData("sessionid", defaultRequest.Session!.SessionID).AddPostData("cookiepreferences", JsonSerializer.Serialize(cookiepreferences));
        var response = await Downloader.PostAsync(request);
        if (!response.Success || response.Data.IsEmpty() || response.Data == "<!DOCTYPE html>" ||
            response.Data!.Contains("btn_blue_steamui btn_medium login_btn")) return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseSuccess>(HttpUtility.HtmlDecode(response.Data))!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    public static ResponseSuccess account_ajaxsetcookiepreferences(DefaultRequest defaultRequest, CookiePreferences cookiepreferences)
    {
        if (defaultRequest.Session == null)
            return new();
        var request = new PostRequest(SteamPoweredUrls.Account_AjaxSetCookiePreferences, Downloader.AppFormUrlEncoded)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
        };
        request.AddPostData("sessionid", defaultRequest.Session!.SessionID).AddPostData("cookiepreferences", JsonSerializer.Serialize(cookiepreferences));
        var response = Downloader.Post(request);
        if (!response.Success || response.Data.IsEmpty() || response.Data == "<!DOCTYPE html>" ||
            response.Data!.Contains("btn_blue_steamui btn_medium login_btn"))
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseSuccess>(HttpUtility.HtmlDecode(response.Data))!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    public static MoreHistoryModel? account_loadmorehistory(DefaultRequest defaultRequest, PurchaseHistoryCursorModel cursor)
    {
        var request = new PostRequest(SteamPoweredUrls.Account_AjaxLoadMoreHistory, Downloader.AppFormUrlEncoded)
        {
            UseVersion2 = true,
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            CancellationToken = defaultRequest.CancellationToken,
            Referer = SteamPoweredUrls.Account_History,
        };
        request.AddPostData("cursor[wallet_txnid]", cursor.WalletTxnId).AddPostData("cursor[timestamp_newest]", cursor.TimestampNewest)
            .AddPostData("cursor[balance]", cursor.Balance).AddPostData("cursor[currency]", cursor.Currency).AddPostData("sessionid", request.Session!.SessionID);
        var response = Downloader.Post(request);
        if (!response.Success)
            return null;
        var obj = JsonSerializer.Deserialize<MoreHistoryModel>(response.Data!, Steam.JsonOptions)!;
        return obj;
    }
    public static async Task<MoreHistoryModel?> account_loadmorehistory_async(DefaultRequest defaultRequest, PurchaseHistoryCursorModel cursor)
    {
        var request = new PostRequest(SteamPoweredUrls.Account_AjaxLoadMoreHistory, Downloader.AppFormUrlEncoded)
        {
            UseVersion2 = true,
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            CancellationToken = defaultRequest.CancellationToken,
            Referer = SteamPoweredUrls.Account_History,
        };
        request.AddPostData("cursor[wallet_txnid]", cursor.WalletTxnId).AddPostData("cursor[timestamp_newest]", cursor.TimestampNewest)
            .AddPostData("cursor[balance]", cursor.Balance).AddPostData("cursor[currency]", cursor.Currency).AddPostData("sessionid", request.Session!.SessionID);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return null;
        var obj = JsonSerializer.Deserialize<MoreHistoryModel>(response.Data!, Steam.JsonOptions)!;
        return obj;
    }

    /// <summary>
    /// Производит регистрацию ключа на аккаунте
    /// </summary>
    /// <param name="product_key">Ключ для регистрации</param>
    /// <returns>Информация о статусе регистрации ключа</returns>
    public static async Task<AjaxLicense> account_registerkey_async(DefaultRequest ajaxRequest, string product_key)
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
    public static AjaxLicense account_registerkey(DefaultRequest ajaxRequest, string product_key)
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

	public static AjaxWalletCode account_redeemwalletcode(DefaultRequest ajaxRequest, string walllet_code)
	{
		var request = new PostRequest(SteamPoweredUrls.Account_AjaxRedeemWalletCode, Downloader.AppFormUrlEncoded)
		{
			Session = ajaxRequest.Session,
			Proxy = ajaxRequest.Proxy,
			Referer = SteamPoweredUrls.Account_RedeemWalletCode,
			IsAjax = true,
			CancellationToken = ajaxRequest.CancellationToken,
		};
		request.AddPostData("walllet_code", walllet_code).AddPostData("sessionid", ajaxRequest.Session!.SessionID);
		var response = Downloader.Post(request);
		if (!response.Success)
			return new();
		try
		{
			var obj = JsonSerializer.Deserialize<AjaxWalletCode>(response.Data!)!;
			return obj;
		}
		catch (Exception)
		{
			return new();
		}
	}
	public static async Task<AjaxWalletCode> account_redeemwalletcode_async(DefaultRequest ajaxRequest, string walllet_code)
	{
		var request = new PostRequest(SteamPoweredUrls.Account_AjaxRedeemWalletCode, Downloader.AppFormUrlEncoded)
		{
			Session = ajaxRequest.Session,
			Proxy = ajaxRequest.Proxy,
			Referer = SteamPoweredUrls.Account_RedeemWalletCode,
			IsAjax = true,
			CancellationToken = ajaxRequest.CancellationToken,
		};
		request.AddPostData("walllet_code", walllet_code).AddPostData("sessionid", ajaxRequest.Session!.SessionID);
		var response = await Downloader.PostAsync(request);
		if (!response.Success)
			return new();
		try
		{
			var obj = JsonSerializer.Deserialize<AjaxWalletCode>(response.Data!)!;
			return obj;
		}
		catch (Exception)
		{
			return new();
		}
	}
	#endregion

	[Obsolete("Этот метод работает не так хорошо, как мог бы. Рекомендуется использовать API.ILoyaltyRewardsService.GetSummaryAsync().")]
    public static async Task<SteamLoyaltyStore> store_loyalty_store_async(DefaultRequest defaultRequest)
    {
        var request = new GetRequest(SteamPoweredUrls.Points_Shop)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            CancellationToken = defaultRequest.CancellationToken,
        };
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        else if (response.Data == "<!DOCTYPE html>")
            return new();
        string data = response.Data!.GetBetween("data-loyaltystore=\"", "\">")!;
        if (data == null)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<SteamLoyaltyStore>(HttpUtility.HtmlDecode(data))!;
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    /// <summary>
    /// Выполняет отписку от всех файлов в мастерской
    /// </summary>
    /// <returns>Класс содержащий информацию о выполнении запроса</returns>
    public static async Task<DTO.Response> sharedfiles_unsubscribeall_async(DefaultRequest defaultRequest)
    {
        var request = new PostRequest(SteamCommunityUrls.SharedFiles_UnsubscribeAll, Downloader.AppFormUrlEncoded)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
        };
        request.AddPostData("sessionid", defaultRequest.Session!.SessionID).AddPostData("appid", 0).AddPostData("filetype", 18);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            // выполняем десерилизацию потому что присылают пустой объект
            // так мы понимаем что запрос точно прошёл
            _ = JsonSerializer.Deserialize<ResponseSuccess>(response.Data!)!;
            return new() { Success = true };
        }
        catch (Exception)
        {
            return new();
        }
    }
    /// <summary>
    /// Выполняет отписку от всех файлов в мастерской
    /// </summary>
    /// <returns>Класс содержащий информацию о выполнении запроса</returns>
    public static DTO.Response sharedfiles_unsubscribeall(DefaultRequest defaultRequest)
    {
        var request = new PostRequest(SteamCommunityUrls.SharedFiles_UnsubscribeAll, Downloader.AppFormUrlEncoded)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
        };
        request.AddPostData("sessionid", defaultRequest.Session!.SessionID).AddPostData("appid", 0).AddPostData("filetype", 18);
        var response = Downloader.Post(request);
        if (!response.Success)
            return new();
        try
        {
            // выполняем десерилизацию потому что присылают пустой объект
            // так мы понимаем что запрос точно прошёл
            _ = JsonSerializer.Deserialize<ResponseSuccess>(response.Data!)!;
            return new() { Success = true };
        }
        catch (Exception)
        {
            return new();
        }
    }

    public static async Task<Data<StoreUserConfig>> pointssummary_ajaxgetasyncconfig_async(DefaultRequest defaultRequest)
    {
        var request = new GetRequest(SteamCommunityUrls.PointsSummary_AjaxGetAsyncConfig)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            CancellationToken = defaultRequest.CancellationToken,
            Referer = $"https://steamcommunity.com/profiles/" + (defaultRequest.Session?.SteamID ?? 0)
        };
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Data<StoreUserConfig>>(response.Data!)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    public static Data<StoreUserConfig> pointssummary_ajaxgetasyncconfig(DefaultRequest defaultRequest)
    {
        var request = new GetRequest(SteamCommunityUrls.PointsSummary_AjaxGetAsyncConfig)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            CancellationToken = defaultRequest.CancellationToken,
            Referer = $"https://steamcommunity.com/profiles/" + (defaultRequest.Session?.SteamID ?? 0)
        };
        var response = Downloader.Get(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Data<StoreUserConfig>>(response.Data!)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    /// <summary>
    /// Выполняет указанное действие с указанным список друзей
    /// </summary>
    /// <param name="steamid">SteamId аккаунта, у которого производят действия</param>
    /// <param name="action">Тип действия</param>
    /// <param name="steamids">С кем производим действия (не более 100)</param>
    /// <exception cref="ArgumentException">steamids не может быть пустым</exception>
    /// <exception cref="ArgumentOutOfRangeException">steamids не может превышать 100 элементов</exception>
    /// <returns></returns>
    public static async Task<SuccessRgCounts> friends_action_async(DefaultRequest defaultRequest, ulong steamid, FriendsAction action, string[] steamids)
    {
        if (defaultRequest.Session == null)
            return new();
        if (steamids.Length == 0)
            throw new ArgumentException("steamids не должен быть пустой");
        if (steamids.Length > 100)
            throw new ArgumentOutOfRangeException(nameof(steamids), "steamids не может превышать 100 элементов");

        string url = $"https://steamcommunity.com/profiles/{steamid}/friends/action";
        var request = new PostRequest(url, Downloader.AppFormUrlEncoded)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
        };
        request.AddPostData("sessionid", defaultRequest.Session!.SessionID).AddPostData("steamid", steamid).AddPostData("ajax", 1);
        switch (action)
        {
            case FriendsAction.Block:
                request.AddPostData("action", "block");
                break;
            case FriendsAction.UnBlock:
                request.AddPostData("action", "unblock");
                break;
            case FriendsAction.UnFriend:
                request.AddPostData("action", "remove");
                break;
            case FriendsAction.LeaveFromGroup:
                request.AddPostData("action", "leave_group");
                break;
            case FriendsAction.IgnoreFriendInvite:
                request.AddPostData("action", "ignore_invite");
                break;
            case FriendsAction.AcceptFriend:
                request.AddPostData("action", "accept");
                break;
            case FriendsAction.UnFollow:
                request.AddPostData("action", "unfollow");
                break;
            case FriendsAction.AcceptGroup:
                request.AddPostData("action", "group_accept");
                break;
            case FriendsAction.IgnoreGroup:
                request.AddPostData("action", "group_ignore");
                break;
        }
        for (int i = 0; i < steamids.Length; i++)
            request.AddPostData("steamids%5B%5D", steamids[i]);
        var response = await Downloader.PostAsync(request);
        if (!response.Success || response.Data.IsEmpty() || response.Data == "<!DOCTYPE html>" ||
            response.Data!.Contains("btn_blue_steamui btn_medium login_btn"))
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<SuccessRgCounts>(HttpUtility.HtmlDecode(response.Data))!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    /// <summary>
    /// Выполняет указанное действие с указанным список друзей
    /// </summary>
    /// <param name="steamid">SteamId аккаунта, у которого производят действия</param>
    /// <param name="action">Тип действия</param>
    /// <param name="steamids">С кем производим действия (не более 100)</param>
    /// <exception cref="ArgumentException">steamids не может быть пустым</exception>
    /// <exception cref="ArgumentOutOfRangeException">steamids не может превышать 100 элементов</exception>
    /// <returns></returns>
    public static SuccessRgCounts friends_action(DefaultRequest defaultRequest, ulong steamid, FriendsAction action, string[] steamids)
    {
        if (defaultRequest.Session == null)
            return new();
        if (steamids.Length == 0)
            throw new ArgumentException("steamids не должен быть пустой");
        if (steamids.Length > 100)
            throw new ArgumentOutOfRangeException(nameof(steamids), "steamids не может превышать 100 элементов");

        string url = $"https://steamcommunity.com/profiles/{steamid}/friends/action";
        var request = new PostRequest(url, Downloader.AppFormUrlEncoded)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
        };
        request.AddPostData("sessionid", defaultRequest.Session!.SessionID).AddPostData("steamid", steamid).AddPostData("ajax", 1);
        switch (action)
        {
            case FriendsAction.Block:
                request.AddPostData("action", "block");
                break;
            case FriendsAction.UnBlock:
                request.AddPostData("action", "unblock");
                break;
            case FriendsAction.UnFriend:
                request.AddPostData("action", "remove");
                break;
            case FriendsAction.LeaveFromGroup:
                request.AddPostData("action", "leave_group");
                break;
            case FriendsAction.IgnoreFriendInvite:
                request.AddPostData("action", "ignore_invite");
                break;
            case FriendsAction.AcceptFriend:
                request.AddPostData("action", "accept");
                break;
            case FriendsAction.UnFollow:
                request.AddPostData("action", "unfollow");
                break;
            case FriendsAction.AcceptGroup:
                request.AddPostData("action", "group_accept");
                break;
            case FriendsAction.IgnoreGroup:
                request.AddPostData("action", "group_ignore");
                break;
        }
        for (int i = 0; i < steamids.Length; i++)
            request.AddPostData("steamids%5B%5D", steamids[i]);
        var response = Downloader.Post(request);
        if (!response.Success || response.Data.IsEmpty() || response.Data == "<!DOCTYPE html>" ||
            response.Data!.Contains("btn_blue_steamui btn_medium login_btn"))
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<SuccessRgCounts>(HttpUtility.HtmlDecode(response.Data))!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    public static async Task<QueueApps> explore_generatenewdiscoveryqueue_async(DefaultRequest defaultRequest, ushort queuetype = 0)
    {
        if (defaultRequest.Session == null)
            return new();
        var request = new PostRequest(SteamPoweredUrls.Explore_GenerateNewDiscoveryQueue, Downloader.AppFormUrlEncoded)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
            Referer = SteamPoweredUrls.Explore,
		};
        request.AddPostData("sessionid", defaultRequest.Session!.SessionID).AddPostData("queuetype", queuetype);
        var response = await Downloader.PostAsync(request);
        if (!response.Success || response.Data.IsEmpty() || response.Data == "<!DOCTYPE html>" ||
            response.Data!.Contains("btn_blue_steamui btn_medium login_btn")) return new();
        try
        {
            var obj = JsonSerializer.Deserialize<QueueApps>(response.Data)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    public static QueueApps explore_generatenewdiscoveryqueue(DefaultRequest defaultRequest, ushort queuetype = 0)
    {
        if (defaultRequest.Session == null)
            return new();
        var request = new PostRequest(SteamPoweredUrls.Explore_GenerateNewDiscoveryQueue, Downloader.AppFormUrlEncoded)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
            Referer = SteamPoweredUrls.Explore,
		};
        request.AddPostData("sessionid", defaultRequest.Session!.SessionID).AddPostData("queuetype", queuetype);
        var response = Downloader.Post(request);
        if (!response.Success || response.Data.IsEmpty() || response.Data == "<!DOCTYPE html>" ||
            response.Data!.Contains("btn_blue_steamui btn_medium login_btn")) return new();
        try
        {
            var obj = JsonSerializer.Deserialize<QueueApps>(response.Data)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    [Obsolete]
    public static async Task<InventoryHistory> inventory_history_async(DefaultRequest defaultRequest, InventoryHistoryCursor cursor, uint[] appid)
    {
        if (defaultRequest.Session == null)
            return new();
        var request = new GetRequest(SteamCommunityUrls.My_InventoryHistory)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
        };
        request.AddQuery("ajax", 1).AddQuery("cursor%5Btime%5D", cursor.CursorTime).AddQuery("cursor%5Btime_frac%5D", cursor.CursorTime)
            .AddQuery("cursor%5Bs%5D", cursor.CursorS).AddQuery("sessionid", defaultRequest.Session!.SessionID);
        for (int i = 0; i < appid.Length; i++)
            request.AddQuery("app%5B%5D", appid[i]);
        var response = await Downloader.GetAsync(request);
        if (!response.Success || response.Data.IsEmpty() || response.Data == "<!DOCTYPE html>" ||
            response.Data!.Contains("btn_blue_steamui btn_medium login_btn"))
            return new();
        InventoryHistory obj;
        try
        {
            obj = JsonSerializer.Deserialize<InventoryHistory>(response.Data)!;
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    [Obsolete]
	public static bool salevote(DefaultRequest defaultRequest, int voteId, int appId)
	{
		var postRequest = new PostRequest(SteamPoweredUrls.SaleVote, Downloader.AppFormUrlEncoded)
		{
            Referer = SteamPoweredUrls.SteamAwards,
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
        };
        postRequest.AddPostData("sessionid", defaultRequest.Session!.SessionID, false).AddPostData("voteid", voteId)
			.AddPostData("appid", appId).AddPostData("developerid", 0);
		var response = Downloader.Post(postRequest);
		return response.Success;
	}
	[Obsolete]
	public static async Task<bool> salevote_async(DefaultRequest defaultRequest, int voteId, int appId)
	{
		var postRequest = new PostRequest(SteamPoweredUrls.SaleVote, Downloader.AppFormUrlEncoded)
		{
			Referer = SteamPoweredUrls.SteamAwards,
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
        };
		postRequest.AddPostData("sessionid", defaultRequest.Session!.SessionID, false).AddPostData("voteid", voteId)
			.AddPostData("appid", appId).AddPostData("developerid", 0);
		var response = await Downloader.PostAsync(postRequest);
		return response.Success;
	}

    [Obsolete("Этот метод работает не так хорошо, как мог бы. Рекомендуется использовать pointssummary_ajaxgetasyncconfig().")]
    public static StoreUserConfig? get_web_token(DefaultRequest defaultRequest)
    {
        var request = new GetRequest(SteamPoweredUrls.SteamAwards)
        {
            UseVersion2 = true,
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            CancellationToken = defaultRequest.CancellationToken,
        };
        var response = Downloader.Get(request);
        if (!response.Success)
            return null;
        var data = response.Data?.GetBetween("data-store_user_config=\"", "\">");
        if (data == null)
            return null;
        data = HttpUtility.HtmlDecode(data);
        var obj = JsonSerializer.Deserialize<StoreUserConfig>(data!);
		return obj;
    }

    [Obsolete("Этот метод теперь находится в API.IStoreSalesService.SetVote и будет скоро удалён")]
    public static CStoreSalesService_SetVote_Response? set_vote(string web_token, Proxy? proxy, uint voteid, uint appid, uint sale_appid, CancellationToken? token = null)
	{
        var requestDetails = new CStoreSalesService_SetVote_Request
        {
            AppId = appid,
            SaleAppId = sale_appid,
            VoteId = voteid,
        };
		using var memStream1 = new MemoryStream();
		Serializer.Serialize(memStream1, requestDetails);
        string url = "https://api.steampowered.com/IStoreSalesService/SetVote/v1";
		var request = new ProtobufRequest(url, Convert.ToBase64String(memStream1.ToArray()))
		{
			UserAgent = KnownUserAgents.WindowsBrowser,
			Proxy = proxy,
			AccessToken = web_token,
            IsMobile = true,
            CancellationToken = token,
		};
		using var response = Downloader.PostProtobuf(request);
		if (response.EResult != EResult.OK)
			return null;
        var obj = Serializer.Deserialize<CStoreSalesService_SetVote_Response>(response.Stream);
		return obj;
	}

    /// <summary>
    /// Используется для создания запроса на регистрацию api ключа
    /// </summary>
    /// <param name="domain">Указанный domain в ключе</param>
    /// <returns>Ответ на запрос</returns>
    public static RequestKeyResponse dev_requestkey(DefaultRequest defaultRequest, string domain)
    {
        var request = new PostRequest(SteamCommunityUrls.Dev_RequestKey, Downloader.AppFormUrlEncoded)
        {
            UseVersion2 = true,
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
            Referer = SteamCommunityUrls.Dev_APIKey
        }.AddPostData("domain", domain).AddPostData("request_id", "0", false)
        .AddPostData("sessionid", defaultRequest.Session!.SessionID).AddPostData("agreeToTerms", "true", false);
        var response = Downloader.Post(request);
        if (!response.Success)
            return new();
        var options = new JsonSerializerOptions
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        };
        var obj = JsonSerializer.Deserialize<RequestKeyResponse>(response.Data!, options)!;
        return obj;
	}
	/// <summary>
	/// Используется для проверки состояния подтверждения создания ключа и получение его в текстовом виде
    /// <para/>
    /// Рекомендуется отправлять запрос раз в 4-е секунды
	/// </summary>
	/// <param name="domain">Указанный domain в ключе</param>
	/// <param name="request_id">Id запроса, указан в <see cref="RequestKeyResponse.RequestId"/></param>
	/// <returns>Ответ на запрос</returns>
	public static RequestKeyResponse dev_requestkey(DefaultRequest defaultRequest, string domain, ulong request_id)
    {
        var request = new PostRequest(SteamCommunityUrls.Dev_RequestKey, Downloader.AppFormUrlEncoded)
        {
            Session = defaultRequest.Session,
            Proxy = defaultRequest.Proxy,
            IsAjax = true,
            CancellationToken = defaultRequest.CancellationToken,
            UseVersion2 = true,
            Referer = SteamCommunityUrls.Dev_APIKey
        }.AddPostData("domain", domain).AddPostData("request_id", request_id)
        .AddPostData("sessionid", defaultRequest.Session!.SessionID).AddPostData("agreeToTerms", "true", false);
        var response = Downloader.Post(request);
        if (!response.Success)
            return new();
        var options = new JsonSerializerOptions
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        };
        var obj = JsonSerializer.Deserialize<RequestKeyResponse>(response.Data!, options)!;
        return obj;
    }
	/// <summary>
	/// Используется для проверки состояния подтверждения создания ключа и получение его в текстовом виде
	/// <para/>
	/// Рекомендуется отправлять запрос раз в 4-е секунды
	/// </summary>
	/// <param name="domain">Указанный domain в ключе</param>
	/// <param name="request">Оригинальный ответ на запроса</param>
	/// <returns>Ответ на запрос</returns>
	public static RequestKeyResponse dev_requestkey(DefaultRequest defaultRequest, string domain, RequestKeyResponse request)
        => dev_requestkey(defaultRequest, domain, request.RequestId!.Value);

	public static GooValue auction_ajaxgetgoovalueforitemtype(DefaultRequest ajaxRequest, uint appid, uint item_type, byte border_color)
	{
		var request = new GetRequest(SteamCommunityUrls.Auction_AjaxGetGooValueForItemType)
		{
			CancellationToken = ajaxRequest.CancellationToken,
			Referer = SteamCommunityUrls.My_Inventory,
			IsAjax = true,
			Session = ajaxRequest.Session,
			Proxy = ajaxRequest.Proxy,
		};
		request.AddQuery("appid", appid).AddQuery("appid", appid).AddQuery("item_type", item_type).AddQuery("border_color", border_color);
		var response = Downloader.Get(request);
		try
		{
			var obj = JsonSerializer.Deserialize<GooValue>(response.Data!, Steam.JsonOptions)!;
			return obj;
		}
		catch (Exception)
		{
			return new() { Success = EResult.BadResponse };
		}
	}
	public static async Task<GooValue> auction_ajaxgetgoovalueforitemtype_async(DefaultRequest ajaxRequest, uint appid, uint item_type, byte border_color)
	{
		var request = new GetRequest(SteamCommunityUrls.Auction_AjaxGetGooValueForItemType)
		{
			CancellationToken = ajaxRequest.CancellationToken,
			Referer = SteamCommunityUrls.My_Inventory,
			IsAjax = true,
			Session = ajaxRequest.Session,
			Proxy = ajaxRequest.Proxy,
		};
		request.AddQuery("appid", appid).AddQuery("appid", appid).AddQuery("item_type", item_type).AddQuery("border_color", border_color);
		var response = await Downloader.GetAsync(request);
		try
		{
			var obj = JsonSerializer.Deserialize<GooValue>(response.Data!, Steam.JsonOptions)!;
			return obj;
		}
		catch (Exception)
		{
			return new() { Success = EResult.BadResponse };
		}
	}


    /// <summary>
    /// Запаковывает и распаковывает гемы из пака.
    ///
    /// <br/>
    /// Пример кода для запаковки в пак 2000 гемов:
    /// <code>
    /// var assetid = 12345678;
    /// var totalPacks = 2;
    /// var amounts = totalPacks * 1000;
    /// var response = ExchangeGoo(new(Session), 753, assetid, totalPacks, amounts, totalPacks, amounts);
    /// var result = response.Success; // check on EResult.OK
    /// </code>
    ///
    /// <br/>
    /// Пример кода для распаковки 2х паков гемов:
    /// <code>
    /// var assetid = 12345678;
    /// var totalPacks = 2;
    /// var amounts = totalPacks * 1000;
    /// var response = ExchangeGoo(new(Session), 753, assetid, amounts, totalPacks, amounts, totalPacks);
    /// var result = response.Success; // check on EResult.OK
    /// </code>
    /// </summary>
    /// <param name="goo_denomination_in">Сколько хочется получить паков</param>
    /// <param name="goo_amount_in">Сколько отдаём предметов</param>
    /// <param name="goo_denomination_out">Сколько паков получаем | Сколько в одном паке amount</param>
    /// <param name="goo_amount_out_expected">Сколько паков ожидаем на выходе</param>
    public static Data exchange_goo(DefaultRequest ajaxRequest, uint appid, ulong assetid, uint goo_denomination_in, uint goo_amount_in, uint goo_denomination_out, uint goo_amount_out_expected)
    {
        var request = new PostRequest(SteamCommunityUrls.My_AjaxExchangeGoo, Downloader.AppFormUrlEncoded)
        {
            CancellationToken = ajaxRequest.CancellationToken,
            Referer = SteamCommunityUrls.My_Inventory,
            IsAjax = true,
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
        };
        request.AddPostData("sessionid", ajaxRequest.Session?.SessionID!, false).AddPostData("appid", appid)
            .AddPostData("assetid", assetid).AddPostData("goo_denomination_in", goo_denomination_in).AddPostData("goo_amount_in", goo_amount_in)
            .AddPostData("goo_denomination_out", goo_denomination_out).AddPostData("goo_amount_out_expected", goo_amount_out_expected);
        var response = Downloader.Post(request);
        try
        {
            var obj = JsonSerializer.Deserialize<Data>(response.Data!, Steam.JsonOptions)!;
            return obj;
        }
        catch (Exception)
        {
            return new() { Success = EResult.BadResponse };
        }
    }
    /// <summary>
    /// Запаковывает и распаковывает гемы из пака.
    ///
    /// <br/>
    /// Пример кода для запаковки в пак 2000 гемов:
    /// <code>
    /// var assetid = 12345678;
    /// var totalPacks = 2;
    /// var amounts = totalPacks * 1000;
    /// var response = ExchangeGoo(new(Session), 753, assetid, totalPacks, amounts, totalPacks, amounts);
    /// var result = response.Success; // check on EResult.OK
    /// </code>
    ///
    /// <br/>
    /// Пример кода для распаковки 2х паков гемов:
    /// <code>
    /// var assetid = 12345678;
    /// var totalPacks = 2;
    /// var amounts = totalPacks * 1000;
    /// var response = ExchangeGoo(new(Session), 753, assetid, amounts, totalPacks, amounts, totalPacks);
    /// var result = response.Success; // check on EResult.OK
    /// </code>
    /// </summary>
    /// <param name="goo_denomination_in">Сколько хочется получить паков</param>
    /// <param name="goo_amount_in">Сколько отдаём предметов</param>
    /// <param name="goo_denomination_out">Сколько паков получаем | Сколько в одном паке amount</param>
    /// <param name="goo_amount_out_expected">Сколько паков ожидаем на выходе</param>
    public static async Task<Data> exchange_goo_async(DefaultRequest ajaxRequest, uint appid, ulong assetid, uint goo_denomination_in, uint goo_amount_in, uint goo_denomination_out, uint goo_amount_out_expected)
    {
        var request = new PostRequest(SteamCommunityUrls.My_AjaxExchangeGoo, Downloader.AppFormUrlEncoded)
        {
            CancellationToken = ajaxRequest.CancellationToken,
            Referer = SteamCommunityUrls.My_Inventory,
            IsAjax = true,
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
        };
        request.AddPostData("sessionid", ajaxRequest.Session?.SessionID!, false).AddPostData("appid", appid)
            .AddPostData("assetid", assetid).AddPostData("goo_denomination_in", goo_denomination_in).AddPostData("goo_amount_in", goo_amount_in)
            .AddPostData("goo_denomination_out", goo_denomination_out).AddPostData("goo_amount_out_expected", goo_amount_out_expected);
        var response = await Downloader.PostAsync(request);
        try
        {
            var obj = JsonSerializer.Deserialize<Data>(response.Data!, Steam.JsonOptions)!;
            return obj;
        }
        catch (Exception)
        {
            return new() { Success = EResult.BadResponse };
        }
    }

    /// <summary>
    /// Конвертирует предмет в gem.
    ///
    /// <br/>
    /// Пример кода для конвертирования (steam inv 753):
    /// <code>
    /// var (appid, amount_expected, _) = inv_asset.GetGooValue();
    /// var assetid = 12345678;
    /// var contextid = 6;
    /// var response = GrindIntoGoo(new(Session), appid, assetid, contextid, amount_expected);
    /// var result = response.Success; // check on EResult.OK
    /// </code>
    /// </summary>
    /// <param name="appid">Id приложения, к которому относится предмет</param>
    /// <param name="assetid">Id ассета, в инвентаре</param>
    /// <param name="contextid">Id конекста, где находится ассет</param>
    /// <param name="goo_value_expected">Какое количество должно быть получено</param>
    public static GridIntoGoo grind_into_goo(DefaultRequest ajaxRequest, uint appid, ulong assetid, ulong contextid, uint goo_value_expected)
    {
        var request = new PostRequest(SteamCommunityUrls.My_AjaxGrindIntoGoo, Downloader.AppFormUrlEncoded)
        {
            CancellationToken = ajaxRequest.CancellationToken,
            Referer = SteamCommunityUrls.My_Inventory,
            IsAjax = true,
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
        };
        request.AddPostData("sessionid", ajaxRequest.Session?.SessionID!, false).AddPostData("appid", appid)
            .AddPostData("assetid", assetid).AddPostData("contextid", contextid).AddPostData("goo_value_expected", goo_value_expected);
        var response = Downloader.Post(request);
        try
        {
            var obj = JsonSerializer.Deserialize<GridIntoGoo>(response.Data!, Steam.JsonOptions)!;
            return obj;
        }
        catch (Exception)
        {
            return new() { Success = EResult.BadResponse };
        }
    }
    /// <summary>
    /// Конвертирует предмет в gem.
    ///
    /// <br/>
    /// Пример кода для конвертирования (steam inv 753):
    /// <code>
    /// var (appid, amount_expected, _) = inv_asset.GetGooValue();
    /// var assetid = 12345678;
    /// var contextid = 6;
    /// var response = GrindIntoGoo(new(Session), appid, assetid, contextid, amount_expected);
    /// var result = response.Success; // check on EResult.OK
    /// </code>
    /// </summary>
    /// <param name="appid">Id приложения, к которому относится предмет</param>
    /// <param name="assetid">Id ассета, в инвентаре</param>
    /// <param name="contextid">Id конекста, где находится ассет</param>
    /// <param name="goo_value_expected">Какое количество должно быть получено</param>
    public static async Task<GridIntoGoo> grind_into_goo_async(DefaultRequest ajaxRequest, uint appid, ulong assetid, ulong contextid, uint goo_value_expected)
    {
        var request = new PostRequest(SteamCommunityUrls.My_AjaxGrindIntoGoo, Downloader.AppFormUrlEncoded)
        {
            CancellationToken = ajaxRequest.CancellationToken,
            Referer = SteamCommunityUrls.My_Inventory,
            IsAjax = true,
            Session = ajaxRequest.Session,
            Proxy = ajaxRequest.Proxy,
        };
        request.AddPostData("sessionid", ajaxRequest.Session?.SessionID!, false).AddPostData("appid", appid)
            .AddPostData("assetid", assetid).AddPostData("contextid", contextid).AddPostData("goo_value_expected", goo_value_expected);
        var response = await Downloader.PostAsync(request);
        try
        {
            var obj = JsonSerializer.Deserialize<GridIntoGoo>(response.Data!, Steam.JsonOptions)!;
            return obj;
        }
        catch (Exception)
        {
            return new() { Success = EResult.BadResponse };
        }
    }
}
