using System.Text.Json.Serialization;
using SteamWeb.Extensions;

namespace SteamWeb.API.Models.IEconService;
public class Trades
{
	[JsonPropertyName("trade_offers_sent")] public Trade[] TradeOffersSent { get; init; } = Array.Empty<Trade>();
	[JsonPropertyName("trade_offers_received")] public Trade[] TradeOffersReceived { get; init; } = Array.Empty<Trade>();
	[JsonPropertyName("descriptions")] public TradeHistoryDescription[] Descriptions { get; init; } = Array.Empty<TradeHistoryDescription>();
	[JsonPropertyName("next_cursor")] public int NextCursor { get; init; }

    /// <summary>
    /// Доступно если get_descriptions == true
    /// </summary>
    /// <param name="classid"></param>
    /// <param name="instanceid"></param>
    /// <returns>Null если описание не найдено</returns>
    public TradeHistoryDescription? GetDescriptionForItem(string classid, string instanceid)
    {
        var cid = classid.ParseUInt64();
        var iid = instanceid.ParseUInt64();
        var item = Descriptions
            .Where(x => x.ClassId == cid)
            .FirstOrDefault(x => x.InstanceId == iid);
        return item;
    }
    /// <summary>
    /// Доступно если get_descriptions == true
    /// </summary>
    /// <param name="classid"></param>
    /// <param name="instanceid"></param>
    /// <returns>Null если описание не найдено</returns>
    public TradeHistoryDescription? GetDescriptionForItem(ulong classid, ulong instanceid)
	{
		var item = Descriptions
			.Where(x => x.ClassId == classid)
			.FirstOrDefault(x => x.InstanceId == instanceid);
        return item;
    }

    /// <param name="steamid_other">SteamID32</param>
    /// <returns>(Sent, Received)</returns>
    public (Trade[], Trade[]) GetTradeForUser(uint steamid_other)
    {
        var list_sent = new List<Trade>(TradeOffersSent.Length);
        var list_received = new List<Trade>(TradeOffersReceived.Length);
        foreach (var item in TradeOffersSent)
			if (item.AccountIdOther == steamid_other)
				list_sent.Add(item);
		foreach (var item in TradeOffersReceived)
			if (item.AccountIdOther == steamid_other)
				list_received.Add(item);
		return (list_sent.ToArray(), list_received.ToArray());
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="steamid64_other">SteamID64</param>
    /// <returns>(Sent, Received)</returns>
    public (Trade[], Trade[]) GetTradeForUser(ulong steamid64_other)
	{
		uint steamid32_other = steamid64_other.ToSteamId32();
		return GetTradeForUser(steamid32_other);
    }
}