using SteamWeb.API.Models.IEconService;
using SteamWeb.Web;
using System.Text.Json;
using SteamWeb.API.Models;
using SteamWeb.Extensions;

namespace SteamWeb.API;
public static class IEconService
{
    /// <summary>
    /// Gets a history of trades
    /// </summary>
    /// <param name="key"></param>
    /// <param name="max_trades">The number of trades to return information for (min 1)</param>
    /// <param name="start_after_time">The time of the last trade shown on the previous page of results, or the time of the first trade if navigating back</param>
    /// <param name="start_after_tradeid">The tradeid shown on the previous page of results, or the ID of the first trade if navigating back</param>
    /// <param name="navigating_back">The user wants the previous page of results, so return the previous max_trades trades before the start time and ID</param>
    /// <param name="get_descriptions">If set, the item display data for the items included in the returned trades will also be returned</param>
    /// <param name="language">The language to use when loading item display data</param>
    /// <param name="include_failed"></param>
    /// <param name="include_total">If set, the total number of trades the account has participated in will be included in the response</param>
    /// <returns></returns>
    public static async Task<Response<TradesHistory>> GetTradeHistoryAsync(Proxy proxy, string key, uint max_trades, uint? start_after_time = null, ulong? start_after_tradeid = null,
        bool navigating_back = false, bool get_descriptions = false, string language = null, bool include_failed = false, bool include_total = false)
    {
        if (max_trades == 0)
            throw new ArgumentException("max_trades can't be zero");

        var request = new GetRequest(SteamPoweredUrls.IEconService_GetTradeHistory_v1, proxy);
        request.AddQuery("key", key).AddQuery("max_trades", max_trades);

        if (start_after_time.HasValue)
            request.AddQuery("start_after_time", start_after_time.Value);
        if (start_after_tradeid.HasValue)
            request.AddQuery("start_after_tradeid", start_after_tradeid.Value);
        if (navigating_back)
            request.AddQuery("navigating_back", 1);
        if (get_descriptions)
            request.AddQuery("get_descriptions", 1);
        if (!language.IsEmpty())
            request.AddQuery("language", language);
        if (include_failed)
            request.AddQuery("include_failed", 1);
        if (include_total)
            request.AddQuery("include_total", 1);

        var response = await Downloader.GetAsync(request);
        if (!response.Success) return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Response<TradesHistory>>(response.Data);
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    /// <summary>
    /// Returns the estimated hold duration and end date that a trade with a user would have
    /// </summary>
    /// <param name="key"></param>
    /// <param name="steamid_target">User you are trading with (SteamID64)</param>
    /// <param name="trade_offer_access_token">A special token that allows for trade offers from non-friends.</param>
    /// <returns></returns>
    public static async Task<Response<TradeHoldDuration>> GetTradeHoldDurationsAsync(Proxy proxy, string key, ulong steamid_target, string trade_offer_access_token)
    {
        var request = new GetRequest(SteamPoweredUrls.IEconService_GetTradeHoldDurations_v1, proxy).AddQuery("key", key)
            .AddQuery("steamid_target", steamid_target).AddQuery("trade_offer_access_token", trade_offer_access_token);
        var response = await Downloader.GetAsync(request);
        if (!response.Success) return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Response<TradeHoldDuration>>(response.Data);
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
	
    /// <summary>
	/// Returns the estimated hold duration and end date that a trade with a user would have
	/// </summary>
	/// <param name="key"></param>
	/// <param name="steamid_target">User you are trading with (SteamID64)</param>
	/// <param name="trade_offer_access_token">A special token that allows for trade offers from non-friends.</param>
	/// <returns></returns>
	public static Response<TradeHoldDuration> GetTradeHoldDurations(Proxy? proxy, string key, ulong steamid_target, string trade_offer_access_token)
	{
		var request = new GetRequest(SteamPoweredUrls.IEconService_GetTradeHoldDurations_v1, proxy).AddQuery("key", key)
			.AddQuery("steamid_target", steamid_target).AddQuery("trade_offer_access_token", trade_offer_access_token);
		var response = Downloader.Get(request);
		if (!response.Success) return new();
		try
		{
			var obj = JsonSerializer.Deserialize<Response<TradeHoldDuration>>(response.Data!);
			obj!.success = true;
			return obj;
		}
		catch (Exception)
		{
			return new();
		}
	}

	/// <summary>
	/// Gets a specific trade offer
	/// </summary>
	/// <param name="key"></param>
	/// <param name="tradeofferid"></param>
	/// <param name="get_descriptions">If set, the item display data for the items included in the returned trade offers will also be returned. If one or more descriptions can't be retrieved, then your request will fail.</param>
	/// <param name="language"></param>
	/// <returns></returns>
	public static Response<TradeOffer> GetTradeOffer(Proxy proxy, string key, ulong tradeofferid, bool get_descriptions = false, string language = null)
    {
        var request = new GetRequest(SteamPoweredUrls.IEconService_GetTradeOffer_v1, proxy).AddQuery("key", key).AddQuery("tradeofferid", tradeofferid);
        if (get_descriptions)
            request.AddQuery("get_descriptions", 1);
        if (!string.IsNullOrEmpty(language))
            request.AddQuery("language", language);
        var response = Downloader.Get(request);
        if (!response.Success) return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Response<TradeOffer>>(response.Data);
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    /// <summary>
    /// Gets a specific trade offer
    /// </summary>
    /// <param name="key"></param>
    /// <param name="tradeofferid"></param>
    /// <param name="get_descriptions">If set, the item display data for the items included in the returned trade offers will also be returned. If one or more descriptions can't be retrieved, then your request will fail.</param>
    /// <param name="language"></param>
    /// <returns></returns>
    public static async Task<Response<TradeOffer>> GetTradeOfferAsync(Proxy proxy, string key, ulong tradeofferid, bool get_descriptions = false, string language = null)
    {
        var request = new GetRequest(SteamPoweredUrls.IEconService_GetTradeOffer_v1, proxy).AddQuery("key", key).AddQuery("tradeofferid", tradeofferid);
        if (get_descriptions)
            request.AddQuery("get_descriptions", 1);
        if (!string.IsNullOrEmpty(language))
            request.AddQuery("language", language);
        var response = await Downloader.GetAsync(request);
        if (!response.Success) return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Response<TradeOffer>>(response.Data);
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    /// <summary>
    /// Get a list of sent or received trade offers
    /// </summary>
    /// <param name="key"></param>
    /// <param name="get_sent_offers">Request the list of sent offers.</param>
    /// <param name="get_received_offers">Request the list of received offers.</param>
    /// <param name="get_descriptions">If set, the item display data for the items included in the returned trade offers will also be returned. If one or more descriptions can't be retrieved, then your request will fail.</param>
    /// <param name="active_only">Indicates we should only return offers which are still active, or offers that have changed in state since the time_historical_cutoff</param>
    /// <param name="historical_only">Indicates we should only return offers which are not active.</param>
    /// <param name="time_historical_cutoff">When active_only is set, offers updated since this time will also be returned</param>
    /// <param name="cursor">Cursor aka start index</param>
    /// <param name="language">The language to use when loading item display data.</param>
    /// <returns></returns>
    public static async Task<Response<Trades>> GetTradeOffersAsync(Proxy proxy, string key, bool get_sent_offers = false, bool get_received_offers = false,
        bool get_descriptions = false, bool active_only = false, bool historical_only = false, uint? time_historical_cutoff = null,
        uint? cursor = null, string? language = null)
    {
        var request = new GetRequest(SteamPoweredUrls.IEconService_GetTradeOffers_v1, proxy).AddQuery("key", key);
        if (time_historical_cutoff.HasValue)
            request.AddQuery("time_historical_cutoff", time_historical_cutoff.Value);
        if (cursor.HasValue)
            request.AddQuery("cursor", cursor.Value);
        if (active_only)
            request.AddQuery("active_only", 1);
        if (get_descriptions)
            request.AddQuery("get_descriptions", 1);
        if (!string.IsNullOrEmpty(language))
            request.AddQuery("language", language);
        if (historical_only)
            request.AddQuery("historical_only", 1);
        if (get_sent_offers)
            request.AddQuery("get_sent_offers", 1);
        if (get_received_offers)
            request.AddQuery("get_received_offers", 1);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Response<Trades>>(response.Data);
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    /// <summary>
    /// Get a list of sent or received trade offers
    /// </summary>
    /// <param name="key"></param>
    /// <param name="get_sent_offers">Request the list of sent offers.</param>
    /// <param name="get_received_offers">Request the list of received offers.</param>
    /// <param name="get_descriptions">If set, the item display data for the items included in the returned trade offers will also be returned. If one or more descriptions can't be retrieved, then your request will fail.</param>
    /// <param name="active_only">Indicates we should only return offers which are still active, or offers that have changed in state since the time_historical_cutoff</param>
    /// <param name="historical_only">Indicates we should only return offers which are not active.</param>
    /// <param name="time_historical_cutoff">When active_only is set, offers updated since this time will also be returned</param>
    /// <param name="cursor">Cursor aka start index</param>
    /// <param name="language">The language to use when loading item display data.</param>
    /// <returns></returns>
    public static Response<Trades> GetTradeOffers(Proxy proxy, string key, bool get_sent_offers = false, bool get_received_offers = false,
        bool get_descriptions = false, bool active_only = false, bool historical_only = false, uint? time_historical_cutoff = null,
        uint? cursor = null, string language = null)
    {
        var request = new GetRequest(SteamPoweredUrls.IEconService_GetTradeOffers_v1, proxy).AddQuery("key", key);
        if (time_historical_cutoff.HasValue)
            request.AddQuery("time_historical_cutoff", time_historical_cutoff.Value);
        if (cursor.HasValue)
            request.AddQuery("cursor", cursor.Value);
        if (active_only)
            request.AddQuery("active_only", 1);
        if (get_descriptions)
            request.AddQuery("get_descriptions", 1);
        if (!string.IsNullOrEmpty(language))
            request.AddQuery("language", language);
        if (historical_only)
            request.AddQuery("historical_only", 1);
        if (get_sent_offers)
            request.AddQuery("get_sent_offers", 1);
        if (get_received_offers)
            request.AddQuery("get_received_offers", 1);
        var response = Downloader.Get(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Response<Trades>>(response.Data);
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    /// <summary>
    /// Get counts of pending and new trade offers
    /// </summary>
    /// <param name="key"></param>
    /// <param name="time_last_visit">The time the user last visited. If not passed, will use the time the user last visited the trade offer page.</param>
    /// <returns></returns>
    public static async Task<Response<TradeOffersSummary>> GetTradeOffersSummaryAsync(Proxy proxy, string key, uint? time_last_visit = null)
    {
        var request = new GetRequest(SteamPoweredUrls.IEconService_GetTradeOffersSummary_v1, proxy).AddQuery("key", key);
        if (time_last_visit.HasValue)
            request.AddQuery("time_last_visit", time_last_visit.Value);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Response<TradeOffersSummary>>(response.Data);
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    /// <summary>
    /// Gets status for a specific trade
    /// </summary>
    /// <param name="key"></param>
    /// <param name="tradeid"></param>
    /// <param name="get_descriptions">If set, the item display data for the items included in the returned trades will also be returned</param>
    /// <param name="language">The language to use when loading item display data</param>
    /// <returns></returns>
    public static async Task<Response<TradeStatus>> GetTradeStatusAsync(Proxy proxy, string key, ulong tradeid, bool get_descriptions = false, string language = null)
    {
        var request = new GetRequest(SteamPoweredUrls.IEconService_GetTradeStatus_v1, proxy).AddQuery("key", key).AddQuery("tradeid", tradeid);
        if (get_descriptions)
            request.AddQuery("get_descriptions", 1);
        if (!string.IsNullOrEmpty(language))
            request.AddQuery("language", language);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Response<TradeStatus>>(response.Data);
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    /// <summary>
    /// Cancel a trade offer we sent
    /// </summary>
    /// <param name="key"></param>
    /// <param name="tradeid"></param>
    /// <returns></returns>
    public static async Task<Response<object>> CancelTradeOfferAsync(Proxy proxy, string key, ulong tradeofferid)
    {
        var request = new GetRequest(SteamPoweredUrls.IEconService_CancelTradeOffer_v1, proxy).AddQuery("key", key).AddQuery("tradeofferid", tradeofferid);
        var response = await Downloader.GetAsync(request);
        if (!response.Success) return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Response<object>>(response.Data);
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    /// <summary>
    /// Decline a trade offer someone sent to us
    /// </summary>
    /// <param name="key"></param>
    /// <param name="tradeid"></param>
    /// <returns></returns>
    public static async Task<Response<object>> DeclineTradeOfferAsync(Proxy proxy, string key, ulong tradeofferid)
    {
        var request = new GetRequest(SteamPoweredUrls.IEconService_DeclineTradeOffer_v1, proxy).AddQuery("key", key).AddQuery("tradeofferid", tradeofferid);
        var response = await Downloader.GetAsync(request);
        if (!response.Success) return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Response<object>>(response.Data);
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
}
