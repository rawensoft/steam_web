using System.Text.Json;
using SteamWeb.Web;
using AngleSharp.Html.Parser;
using SteamWeb.Extensions;
using SteamWeb.Script.Enums;
using System.Web;
using SteamWeb.Script.DTO;
using SteamWeb.Script.DTO.CookiePreferences;
using SteamWeb.Script.DTO.Listinging;
using SteamWeb.Script.DTO.Historing;
using SteamWeb.Auth.Interfaces;
using System.Text.RegularExpressions;

namespace SteamWeb.Script;
public static class Ajax
{
    public static async Task<Success> sharedfiles_unsubscribeall_async(ISessionProvider session, System.Net.IWebProxy proxy)
    {
        var request = new PostRequest(SteamCommunityUrls.SharedFiles_UnsubscribeAll, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            IsAjax = true
        };
        request.AddPostData("sessionid", session.SessionID).AddPostData("appid", 0).AddPostData("filetype", 18);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Success>(response.Data!);
            obj!.success = 1;
            return obj;
        }
        catch (Exception)
		{
			return new();
		}
    }
    public static Success sharedfiles_unsubscribeall(ISessionProvider session, System.Net.IWebProxy proxy)
    {
        var request = new PostRequest(SteamCommunityUrls.SharedFiles_UnsubscribeAll, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            IsAjax = true
        };
        request.AddPostData("sessionid", session.SessionID).AddPostData("appid", 0).AddPostData("filetype", 18);
        var response = Downloader.Post(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Success>(response.Data!);
            obj!.success = 1;
            return obj;
        }
        catch (Exception)
		{
			return new();
		}
    }

    public static async Task<CancelTrade> tradeoffer_cancel_async(ISessionProvider session, System.Net.IWebProxy proxy, ulong tradeofferid)
    {
        string url = $"https://steamcommunity.com/tradeoffer/{tradeofferid}/cancel";
        var request = new PostRequest(url, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            IsAjax = true
        };
        request.AddPostData("sessionid", session.SessionID);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
		{
			var options = new JsonSerializerOptions
			{
				NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
			};
			var obj = JsonSerializer.Deserialize<CancelTrade>(response.Data!, options);
            obj!.success = true;
            return obj;
        }
        catch (Exception)
		{
			return new();
		}
    }
    public static async Task<CancelTrade> tradeoffer_cancel_async(ISessionProvider session, System.Net.IWebProxy proxy, API.Models.IEconService.Trade trade)
        => await tradeoffer_cancel_async(session, proxy, trade.u_tradeofferid);
    public static CancelTrade tradeoffer_cancel(ISessionProvider session, System.Net.IWebProxy proxy, ulong tradeofferid)
    {
        string url = $"https://steamcommunity.com/tradeoffer/{tradeofferid}/cancel";
        var request = new PostRequest(url, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            IsAjax = true
        };
        request.AddPostData("sessionid", session.SessionID);
        var response = Downloader.Post(request);
        if (!response.Success)
            return new();
        try
		{
			var options = new JsonSerializerOptions
			{
				NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
			};
			var obj = JsonSerializer.Deserialize<CancelTrade>(response.Data!, options);
            obj!.success = true;
            return obj;
        }
        catch (Exception)
		{
			return new();
		}
    }
    public static CancelTrade tradeoffer_cancel(ISessionProvider session, System.Net.IWebProxy proxy, API.Models.IEconService.Trade trade)
        => tradeoffer_cancel(session, proxy, trade.u_tradeofferid);

	public static Success market_cancelbuyorder(ISessionProvider session, System.Net.IWebProxy proxy, ulong buy_orderid)
	{
		var request = new PostRequest(SteamCommunityUrls.Market_CancelBuyOrder, Downloader.AppFormUrlEncoded)
		{
			Session = session,
			Proxy = proxy,
			IsAjax = true,
            Referer = SteamCommunityUrls.Market
		};
		request.AddPostData("sessionid", session.SessionID);
        request.AddPostData("buy_orderid", buy_orderid);
        request.AddHeader("X-Prototype-Version", "1.7");
		var response = Downloader.Post(request);
		if (!response.Success)
			return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Success>(response.Data!);
            return obj!;
        }
        catch (Exception)
        {
            return new();
        }
    }
	public static async Task<Success> market_cancelbuyorder_async(ISessionProvider session, System.Net.IWebProxy proxy, ulong buy_orderid)
	{
		var request = new PostRequest(SteamCommunityUrls.Market_CancelBuyOrder, Downloader.AppFormUrlEncoded)
		{
			Session = session,
			Proxy = proxy,
			IsAjax = true,
			Referer = SteamCommunityUrls.Market
		};
		request.AddPostData("sessionid", session.SessionID);
		request.AddPostData("buy_orderid", buy_orderid);
		request.AddHeader("X-Prototype-Version", "1.7");
		var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Success>(response.Data!);
            return obj!;
        }
        catch (Exception)
        {
            return new();
        }
    }

	public static DataOrder market_createbuyorder(ISessionProvider session, System.Net.IWebProxy proxy, int currency, uint appid, string market_hash_name, int price_total, ushort quantity)
	{
		var request = new PostRequest(SteamCommunityUrls.Market_CreateBuyOrder, Downloader.AppFormUrlEncoded)
		{
			Session = session,
			Proxy = proxy,
			IsAjax = true,
			Referer = SteamCommunityUrls.Market_Listings + $"/{appid}/" + Regex.Escape(market_hash_name),
		};
		request.AddPostData("sessionid", session.SessionID);
		request.AddPostData("currency", currency);
		request.AddPostData("appid", appid);
		request.AddPostData("market_hash_name", HttpUtility.UrlEncode(market_hash_name), false);
		request.AddPostData("price_total", price_total);
		request.AddPostData("quantity", quantity);
		request.AddPostData("billing_state", "");
		request.AddPostData("save_my_address", 0);
		var response = Downloader.Post(request);
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
	public static async Task<DataOrder> market_createbuyorder_async(ISessionProvider session, System.Net.IWebProxy proxy, int currency, uint appid, string market_hash_name, int price_total, ushort quantity)
	{
		var request = new PostRequest(SteamCommunityUrls.Market_CreateBuyOrder, Downloader.AppFormUrlEncoded)
		{
			Session = session,
			Proxy = proxy,
			IsAjax = true,
			Referer = SteamCommunityUrls.Market_Listings + $"/{appid}/" + Regex.Escape(market_hash_name),
            UseVersion2 = true,
		};
		request.AddPostData("sessionid", session.SessionID);
		request.AddPostData("currency", currency);
		request.AddPostData("appid", appid);
		request.AddPostData("market_hash_name", HttpUtility.UrlEncode(market_hash_name), false);
		request.AddPostData("price_total", price_total);
		request.AddPostData("quantity", quantity);
		request.AddPostData("billing_state", "");
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

	public static async Task<PriceOverview> market_priceoverview_async(ISessionProvider session, System.Net.IWebProxy proxy, uint appid, string country, ushort currency, string market_hash_name)
    {
        var request = new GetRequest(SteamCommunityUrls.Market_PriceOverview, proxy, session)
        {
            Session = session,
            Proxy = proxy,
            IsAjax = true
        };
        request.AddQuery("market_hash_name", market_hash_name).AddQuery("appid", appid).AddQuery("country", country).AddQuery("currency", currency);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<PriceOverview>(response.Data);
            return obj;
        }
        catch (Exception)
        { }
        return new();
    }
    public static PriceOverview market_priceoverview(ISessionProvider session, System.Net.IWebProxy proxy, uint appid, string country, ushort currency, string market_hash_name)
    {
        var request = new GetRequest(SteamCommunityUrls.Market_PriceOverview, proxy, session)
        {
            Session = session,
            Proxy = proxy,
            IsAjax = true
        };
        request.AddQuery("market_hash_name", market_hash_name).AddQuery("appid", appid).AddQuery("country", country).AddQuery("currency", currency);
        var response = Downloader.Get(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<PriceOverview>(response.Data);
            return obj;
        }
        catch (Exception)
        { }
        return new();
    }

    public static async Task<MarketSearchResponse> market_search_render_async(ISessionProvider session, System.Net.IWebProxy proxy, MarketSearchRequest request)
    {
        var getRequest = new GetRequest(SteamCommunityUrls.Market_Search_Render, proxy, session)
        {
            Session = session,
            Proxy = proxy,
            IsAjax = true
        };
        if (!request.Query.IsEmpty())
            getRequest.AddQuery("query", request.Query);
        getRequest.AddQuery("start", request.Start).AddQuery("count", request.Count).AddQuery("search_descriptions", request.Search_Descriptions)
            .AddQuery("norender", request.NoRender).AddQuery("sort_column", request.Sort_Column.ToString().ToLower())
            .AddQuery("sort_dir", request.Sort_Dir.ToString().ToLower()).AddQuery("appid", request.AppId);
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
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<MarketSearchResponse>(response.Data);
            return obj;
        }
        catch (Exception)
        { }
        return new();
    }
    public static MarketSearchResponse market_search_render(ISessionProvider session, System.Net.IWebProxy proxy, MarketSearchRequest request)
    {
        var getRequest = new GetRequest(SteamCommunityUrls.Market_Search_Render, proxy, session)
        {
            Session = session,
            Proxy = proxy,
            IsAjax = true
        };
        if (!request.Query.IsEmpty())
            getRequest.AddQuery("query", request.Query);
        getRequest.AddQuery("start", request.Start).AddQuery("count", request.Count).AddQuery("search_descriptions", request.Search_Descriptions)
            .AddQuery("norender", request.NoRender).AddQuery("sort_column", request.Sort_Column.ToString().ToLower())
            .AddQuery("sort_dir", request.Sort_Dir.ToString().ToLower()).AddQuery("appid", request.AppId);
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
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<MarketSearchResponse>(response.Data);
            return obj;
        }
        catch (Exception)
        { }
        return new();
    }

    public static async Task<Data<WebApiToken>> pointssummary_ajaxgetasyncconfig_async(ISessionProvider session, System.Net.IWebProxy proxy)
    {
        var request = new GetRequest(SteamCommunityUrls.PointsSummary_AjaxGetAsyncConfig, proxy, session, $"https://steamcommunity.com/profiles/{session?.SteamID}");
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Data<WebApiToken>>(response.Data);
            return obj;
        }
        catch (Exception ex)
        { }
        return new();
    }
    public static Data<WebApiToken> pointssummary_ajaxgetasyncconfig(ISessionProvider session, System.Net.IWebProxy proxy)
    {
        var request = new GetRequest(SteamCommunityUrls.PointsSummary_AjaxGetAsyncConfig, proxy, session, $"https://steamcommunity.com/profiles/{session?.SteamID}");
        var response = Downloader.Get(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Data<WebApiToken>>(response.Data);
            return obj;
        }
        catch (Exception ex)
        { }
        return new();
    }

    public static async Task<Listing> market_mylistings_async(ISessionProvider session, System.Net.IWebProxy proxy, int count = 100, int start = 0)
    {
        var request = new GetRequest(SteamCommunityUrls.Market_MyListings, proxy, session)
        {
            Session = session,
            Proxy = proxy,
            IsAjax = true,
            Referer = SteamCommunityUrls.Market
        };
        request.AddQuery("count", count);
        if (start > 0)
            request.AddQuery("start", start);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        return Listing.Deserialize(response.Data);
    }
    public static Listing market_mylistings(ISessionProvider session, System.Net.IWebProxy proxy, int count = 100, int start = 0)
    {
        var request = new GetRequest(SteamCommunityUrls.Market_MyListings, proxy, session)
        {
            Session = session,
            Proxy = proxy,
            IsAjax = true,
            Referer = SteamCommunityUrls.Market
        };
        request.AddQuery("count", count);
        if (start > 0)
            request.AddQuery("start", start);
        var response = Downloader.Get(request);
        if (!response.Success)
            return new();
        return Listing.Deserialize(response.Data);
    }

    public static async Task<Historing> market_myhistory_async(ISessionProvider session, System.Net.IWebProxy proxy, int start = 0, int count = 200)
    {
        // count - При любом значении, присылают не все данные
        var request = new GetRequest(SteamCommunityUrls.Market_MyHistory_Render, proxy, session, SteamCommunityUrls.Market).AddQuery("start", start).AddQuery("count", count);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new Historing() { IsError = true };
        else if (response.Data.Contains("There was an error loading your market history. Please try again later."))
            return new Historing() { IsError = true };
        else if (response.Data.Contains("<a href=\\\"https:\\/\\/steamcommunity.com\\/login\\/home\\/?goto=market%2F%2Frender%2F%3Fstart%3D0%26count%3D10\\\">Login<\\/a> to view your Community Market history."))
            return new Historing() { IsAuthtorized = false };
        return Historing.Deserialize(response.Data);
    }
    public static Historing market_myhistory(ISessionProvider session, System.Net.IWebProxy proxy, int start = 0, int count = 200)
    {
        // count - При любом значении, присылают не все данные
        var request = new GetRequest(SteamCommunityUrls.Market_MyHistory_Render, proxy, session, SteamCommunityUrls.Market).AddQuery("start", start).AddQuery("count", count);
        var response = Downloader.Get(request);
        if (!response.Success)
            return new Historing() { IsError = true };
        else if (response.Data.Contains("There was an error loading your market history. Please try again later."))
            return new Historing() { IsError = true };
        else if (response.Data.Contains("<a href=\\\"https:\\/\\/steamcommunity.com\\/login\\/home\\/?goto=market%2F%2Frender%2F%3Fstart%3D0%26count%3D10\\\">Login<\\/a> to view your Community Market history."))
            return new Historing() { IsAuthtorized = false };
        return Historing.Deserialize(response.Data);
    }

    public static async Task<SellItem> market_sellitem_async(ISessionProvider session, System.Net.IWebProxy proxy, uint appid, string contextid, string assetid, string amount, int price)
    {
        var request = new PostRequest(SteamCommunityUrls.Market_SellItem, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            Referer = $"https://steamcommunity.com/profiles/{session.SteamID}/inventory/",
            IsAjax = true
        };
        request.AddPostData("sessionid", session.SessionID).AddPostData("appid", appid).AddPostData("contextid", contextid)
            .AddPostData("assetid", assetid).AddPostData("amount", amount).AddPostData("price", price);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new SellItem();
        var obj = JsonSerializer.Deserialize<SellItem>(response.Data);
        return obj;
    }
    public static SellItem market_sellitem(ISessionProvider session, System.Net.IWebProxy proxy, uint appid, string contextid, string assetid, string amount, int price)
    {
        var request = new PostRequest(SteamCommunityUrls.Market_SellItem, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            Referer = $"https://steamcommunity.com/profiles/{session.SteamID}/inventory/",
            IsAjax = true
        };
        request.AddPostData("sessionid", session.SessionID).AddPostData("appid", appid).AddPostData("contextid", contextid)
            .AddPostData("assetid", assetid).AddPostData("amount", amount).AddPostData("price", price);
        var response = Downloader.Post(request);
        if (!response.Success)
            return new SellItem();
        var obj = JsonSerializer.Deserialize<SellItem>(response.Data);
        return obj;
    }

    public static async Task<bool> market_removelisting_async(ISessionProvider session, System.Net.IWebProxy proxy, string id, string market_hash_name, uint appid)
    {
        var request = new PostRequest($"https://steamcommunity.com/market/removelisting/{id}", Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            Referer = $"https://steamcommunity.com/market/listings/{appid}/{Uri.EscapeDataString(market_hash_name)}",
            IsAjax = true
        };
        request.AddPostData("sessionid", session.SessionID);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return false;
        else if (response.Data == "[]") return true;
        return false;
    }
    public static bool market_removelisting(ISessionProvider session, System.Net.IWebProxy proxy, string id, string market_hash_name, uint appid)
    {
        var request = new PostRequest($"https://steamcommunity.com/market/removelisting/{id}", Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            Referer = $"https://steamcommunity.com/market/listings/{appid}/{Uri.EscapeDataString(market_hash_name)}",
            IsAjax = true
        };
        request.AddPostData("sessionid", session.SessionID);
        var response = Downloader.Post(request);
        if (!response.Success)
            return false;
        else if (response.Data == "[]") return true;
        return false;
    }

    public static async Task<OrderHistogram> market_itemordershistogram_async(OrderHistogramRequest request)
    {
        var getRequest = new GetRequest(SteamCommunityUrls.Market_ItemOrdersHistogram, request.Proxy, request.Session)
        {
            Referer = $"https://steamcommunity.com/market/listings/{request.AppID}/{Uri.EscapeDataString(request.Market_Hash_Name)}",
            IsAjax = true
        };
        getRequest.AddQuery("country", request.Country).AddQuery("language", request.Language).AddQuery("currency", request.Currency)
            .AddQuery("item_nameid", request.Item_Nameid).AddQuery("two_factor", request.Two_Factor);
        if (request.Timeout > 0)
            getRequest.Timeout = request.Timeout;
        var response = await Downloader.GetAsync(getRequest);
        if (!response.Success)
            return new() { E502L3 = response.StatusCode == 429, InternalError = response.StatusCode == 0 };
        var obj = JsonSerializer.Deserialize<OrderHistogram>(response.Data);
        return obj;
    }
    public static OrderHistogram market_itemordershistogram(OrderHistogramRequest request)
    {
        var getRequest = new GetRequest(SteamCommunityUrls.Market_ItemOrdersHistogram, request.Proxy, request.Session)
        {
            Referer = $"https://steamcommunity.com/market/listings/{request.AppID}/{Uri.EscapeDataString(request.Market_Hash_Name)}",
            IsAjax = true
        };
        getRequest.AddQuery("country", request.Country).AddQuery("language", request.Language).AddQuery("currency", request.Currency)
            .AddQuery("item_nameid", request.Item_Nameid).AddQuery("two_factor", request.Two_Factor);
        if (request.Timeout > 0)
            getRequest.Timeout = request.Timeout;
        var response = Downloader.Get(getRequest);
        if (!response.Success)
            return new() { E502L3 = response.StatusCode == 429, InternalError = response.StatusCode == 0 };
        var obj = JsonSerializer.Deserialize<OrderHistogram>(response.Data);
        return obj;
    }

    public static async Task<PriceHistory> market_pricehistory_async(ISessionProvider session, System.Net.IWebProxy proxy, int appid, string market_hash_name)
    {
        market_hash_name = market_hash_name.Replace("%27", "'").Replace("?", "%3F").Replace("%E2%98%85", "★").Replace("%E2%84%A2", "™");
        var getRequest = new GetRequest(SteamCommunityUrls.Market_PriceHistory, proxy, session)
        {
            Referer = session == null ? "https://steamcommunity.com/my/inventory/" : $"https://steamcommunity.com/profiles/{session.SteamID}/inventory/",
            IsAjax = true
        };
        getRequest.AddQuery("appid", appid).AddQuery("market_hash_name", market_hash_name);
        var response = await Downloader.GetAsync(getRequest);
        if (!response.Success)
            return new() { e500 = response.StatusCode == 502 || response.StatusCode == 503 };
        return PriceHistory.Deserialize(response.Data);
    }
    public static PriceHistory market_pricehistory(ISessionProvider session, System.Net.IWebProxy proxy, uint appid, string market_hash_name)
    {
        market_hash_name = market_hash_name.Replace("%27", "'").Replace("?", "%3F").Replace("%E2%98%85", "★").Replace("%E2%84%A2", "™");
        var getRequest = new GetRequest(SteamCommunityUrls.Market_PriceHistory, proxy, session)
        {
            Referer = session == null ? "https://steamcommunity.com/my/inventory/" : $"https://steamcommunity.com/profiles/{session.SteamID}/inventory/",
            IsAjax = true
        };
        getRequest.AddQuery("appid", appid).AddQuery("market_hash_name", market_hash_name);
        var response = Downloader.Get(getRequest);
        if (!response.Success)
            return new() { e500 = response.StatusCode == 502 || response.StatusCode == 503 };
        return PriceHistory.Deserialize(response.Data);
    }

    public static async Task<ItemQueryLocations[]> action_QueryLocations_async(ISessionProvider session, System.Net.IWebProxy proxy)
    {
        string referer = $"https://steamcommunity.com/profiles/{session.SteamID}/edit/info";
        var request = new GetRequest(SteamCommunityUrls.Actions_QueryLocations, proxy, session, referer) { IsAjax = true };
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new ItemQueryLocations[0];
        var obj = JsonSerializer.Deserialize<ItemQueryLocations[]>(response.Data);
        return obj;
    }
    public static async Task<ItemQueryLocations[]> action_QueryLocations_async(ISessionProvider session, System.Net.IWebProxy proxy, ItemQueryLocations Loc)
    {
        string referer = $"https://steamcommunity.com/profiles/{session.SteamID}/edit/info";
        var request = new GetRequest(SteamCommunityUrls.Actions_QueryLocations + '/' + Loc.countrycode + (Loc.state_loaded ? Loc.statecode : ""), proxy, session, referer)
        {
            IsAjax = true
        };
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new ItemQueryLocations[0];
        var obj = JsonSerializer.Deserialize<ItemQueryLocations[]>(response.Data);
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

    public static async Task<ItemGroup[]> profiles_ajaxgroupinvite_async(ISessionProvider session, System.Net.IWebProxy proxy)
    {
        string url = $"https://steamcommunity.com/profiles/{session.SteamID}/ajaxgroupinvite?select_primary=1&json=1";
        string referer = $"https://steamcommunity.com/profiles/{session.SteamID}/edit/theme";
        var request = new GetRequest(url, proxy, session, referer) { IsAjax = true };
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new ItemGroup[0];
        var obj = JsonSerializer.Deserialize<ItemGroup[]>(response.Data);
        return obj;
    }

    /// <summary>
    /// Оставить комментарий в профиле
    /// </summary>
    /// <param name="session"></param>
    /// <param name="proxy"></param>
    /// <param name="comment"></param>
    /// <param name="count">Количество комментариев на странице</param>
    /// <param name="feature2"></param>
    /// <returns></returns>
    public static async Task<CommentResponse> profiles_post_comment_async(ISessionProvider session, System.Net.IWebProxy proxy, string comment, int count = 6, int feature2 = -1)
        => await profiles_post_comment_async(session, proxy, comment, session.SteamID, count, feature2);
    /// <summary>
    /// Оставить комментарий в профиле
    /// </summary>
    /// <param name="session"></param>
    /// <param name="proxy"></param>
    /// <param name="comment"></param>
    /// <param name="steamid"></param>
    /// <param name="count">Количество комментариев на странице</param>
    /// <param name="feature2"></param>
    /// <returns></returns>
    public static async Task<CommentResponse> profiles_post_comment_async(ISessionProvider session, System.Net.IWebProxy proxy, string comment, ulong steamid, int count = 6, int feature2 = -1)
    {
        string url = $"https://steamcommunity.com/comment/Profile/post/{steamid}/-1/";
        string referer = $"https://steamcommunity.com/profiles/{steamid}/";
        var request = new PostRequest(url, Downloader.AppFormUrlEncoded)
        {
            Session  = session,
            Proxy = proxy,
            Referer = referer,
            IsAjax = true,
        };
        //comment = HttpUtility.UrlEncode(comment);
        request.AddPostData("comment", comment).AddPostData("count", count).AddPostData("sessionid", session.SessionID).AddPostData("feature2", feature2);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<CommentResponse>(response.Data);
            return obj;
        }
        catch (Exception ex)
        {
            return new CommentResponse() { success = false };
        }
    }

    public static async Task<SteamLoyaltyStore> store_loyalty_store_async(ISessionProvider session, System.Net.IWebProxy proxy)
    {
        var request = new GetRequest(SteamPoweredUrls.Points_Shop, proxy, session);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        else if (response.Data == "<!DOCTYPE html>")
            return new();
        string data = response.Data.GetBetween("data-loyaltystore=\"", "\">");
        if (data == null)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<SteamLoyaltyStore>(HttpUtility.HtmlDecode(data));
            obj.success = true;
            return obj;
        }
        catch (Exception ex)
        {
            return new();
        }
    }
    public static async Task<SteamPurchases> account_history_async(ISessionProvider session, System.Net.IWebProxy proxy, SteamPurchaseCursor cursor = null)
    {
        if (session == null)
            return new();
        var request = new PostRequest(SteamPoweredUrls.Account_AjaxLoadMoreHistory, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            IsAjax = true
        };
        request.AddPostData("sessionid", session.SessionID);
        if (cursor != null)
        {
            request.AddPostData("cursor[wallet_txnid]", cursor.wallet_txnid).AddPostData("cursor[timestamp_newest]", cursor.timestamp_newest)
                .AddPostData("cursor[balance]", cursor.balance).AddPostData("cursor[currency]", cursor.currency);
        }
        var response = await Downloader.PostAsync(request);
        if (!response.Success || response.Data.IsEmpty() || response.Data == "<!DOCTYPE html>" ||
            response.Data.Contains("btn_blue_steamui btn_medium login_btn")) return new();
        return SteamPurchases.Deserialize(response.Data);
    }
    public static async Task<Success> account_ajaxsetcookiepreferences_async(ISessionProvider session, System.Net.IWebProxy proxy, CookiePreferences cookiepreferences)
    {
        if (session == null)
            return new();
        var request = new PostRequest(SteamPoweredUrls.Account_AjaxSetCookiePreferences, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            IsAjax = true
        };
        request.AddPostData("sessionid", session.SessionID).AddPostData("cookiepreferences", JsonSerializer.Serialize(cookiepreferences));
        var response = await Downloader.PostAsync(request);
        if (!response.Success || response.Data.IsEmpty() || response.Data == "<!DOCTYPE html>" ||
            response.Data.Contains("btn_blue_steamui btn_medium login_btn")) return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Success>(HttpUtility.HtmlDecode(response.Data));
            return obj;
        }
        catch (Exception ex)
        { }
        return new();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="session"></param>
    /// <param name="proxy"></param>
    /// <param name="steamid">SteamID аккаунта, у которого производят действия</param>
    /// <param name="action">Тип действия</param>
    /// <param name="steamids">С кем производим действия (не более 100)</param>
    /// <exception cref="ArgumentException">В steamids должен быть один или больше элементов, но не более 100</exception>
    /// <returns></returns>
    public static async Task<SuccessRgCounts> friends_action_async(ISessionProvider session, System.Net.IWebProxy proxy, ulong steamid, FriendsAction action, string[] steamids)
    {
        if (session == null)
            return new();
        if (steamids.Length == 0)
            throw new ArgumentException("steamids не должен быть пустой.");
        if (steamids.Length > 100)
            throw new ArgumentOutOfRangeException("steamids не может превышать 100 элементов.");

        string url = $"https://steamcommunity.com/profiles/{steamid}/friends/action";
        var request = new PostRequest(url, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            IsAjax = true
        };
        request.AddPostData("sessionid", session.SessionID).AddPostData("steamid", steamid).AddPostData("ajax", 1);
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
            response.Data.Contains("btn_blue_steamui btn_medium login_btn")) return new();
        try
        {
            var obj = JsonSerializer.Deserialize<SuccessRgCounts>(HttpUtility.HtmlDecode(response.Data));
            return obj;
        }
        catch (Exception ex)
        { }
        return new();
    }
    public static async Task<QueueApps> explore_generatenewdiscoveryqueue_async(ISessionProvider session, System.Net.IWebProxy proxy, ushort queuetype = 0)
    {
        if (session == null)
            return new();
        var request = new PostRequest(SteamPoweredUrls.Explore_GenerateNewDiscoveryQueue, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            IsAjax = true
        };
        request.AddPostData("sessionid", session.SessionID).AddPostData("queuetype", queuetype);
        var response = await Downloader.PostAsync(request);
        if (!response.Success || response.Data.IsEmpty() || response.Data == "<!DOCTYPE html>" ||
            response.Data.Contains("btn_blue_steamui btn_medium login_btn")) return new();
        try
        {
            var obj = JsonSerializer.Deserialize<QueueApps>(response.Data);
            obj.Success = true;
            return obj;
        }
        catch (Exception ex)
        { }
        return new();
    }
    public static QueueApps explore_generatenewdiscoveryqueue(ISessionProvider session, System.Net.IWebProxy proxy, ushort queuetype = 0)
    {
        if (session == null)
            return new();
        var request = new PostRequest(SteamPoweredUrls.Explore_GenerateNewDiscoveryQueue, Downloader.AppFormUrlEncoded)
        {
            Session = session,
            Proxy = proxy,
            IsAjax = true
        };
        request.AddPostData("sessionid", session.SessionID).AddPostData("queuetype", queuetype);
        var response = Downloader.Post(request);
        if (!response.Success || response.Data.IsEmpty() || response.Data == "<!DOCTYPE html>" ||
            response.Data.Contains("btn_blue_steamui btn_medium login_btn")) return new();
        try
        {
            var obj = JsonSerializer.Deserialize<QueueApps>(response.Data);
            obj.Success = true;
            return obj;
        }
        catch (Exception ex)
        { }
        return new();
    }
    public static async Task<InventoryHistory> inventory_history_async(ISessionProvider session, System.Net.IWebProxy proxy, InventoryHistoryCursor cursor, uint[] appid)
    {
        if (session == null)
            return new();
        var request = new GetRequest(SteamCommunityUrls.My_InventoryHistory, proxy, session) { IsAjax = true};
        request.AddQuery("ajax", 1).AddQuery("cursor%5Btime%5D", cursor.CursorTime).AddQuery("cursor%5Btime_frac%5D", cursor.CursorTime)
            .AddQuery("cursor%5Bs%5D", cursor.CursorS).AddQuery("sessionid", session.SessionID);
        for (int i = 0; i < appid.Length; i++)
            request.AddQuery("app%5B%5D", appid[i]);
        var response = await Downloader.GetAsync(request);
        if (!response.Success || response.Data.IsEmpty() || response.Data == "<!DOCTYPE html>" ||
            response.Data.Contains("btn_blue_steamui btn_medium login_btn")) return new();
        InventoryHistory obj;
        try
        {
            obj = JsonSerializer.Deserialize<InventoryHistory>(response.Data);
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }

        var html = new HtmlParser();
        var doc = html.ParseDocument($"<!DOCTYPE html><html lang=\"ru\">{obj.html}</html>");
        var els = doc.GetElementsByClassName("tradehistoryrow");
        foreach (var item in els)
        {
            var data_el = item.GetElementsByClassName("tradehistory_items_group")[0].GetElementsByTagName("span")[0];
            var date_el = item.GetElementsByClassName("tradehistory_date");
            var time_el = date_el[0].GetElementsByClassName("tradehistory_timestamp")[0];

            var date = DateOnly.ParseExact(date_el[0].TextContent.GetClearWebString(), "dd MMM yyyy");
            var time = DateOnly.ParseExact(time_el.TextContent.GetClearWebString(), "h:mmtt");
        }
    }
}