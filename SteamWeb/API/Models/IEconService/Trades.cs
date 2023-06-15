using System.Collections.Generic;

namespace SteamWeb.API.Models.IEconService;
public class Trades
{
    public Trade[] trade_offers_sent { get; init; } = new Trade[0];
    public Trade[] trade_offers_received { get; init; } = new Trade[0];
    public TradeHistoryDescription[] descriptions { get; init; } = new TradeHistoryDescription[0];
    public int next_cursor { get; init; }

    /// <summary>
    /// Доступно если get_descriptions == true
    /// </summary>
    /// <param name="classid"></param>
    /// <param name="instanceid"></param>
    /// <returns>Null если описание не найдено</returns>
    public TradeHistoryDescription GetDescriptionForItem(string classid, string instanceid)
    {
        foreach (var item in descriptions)
        {
            if (item.classid == classid && item.instanceid == instanceid) return item;
        }
        return null;
    }
    /// <summary>
    /// Доступно если get_descriptions == true
    /// </summary>
    /// <param name="classid"></param>
    /// <param name="instanceid"></param>
    /// <returns>Null если описание не найдено</returns>
    public TradeHistoryDescription GetDescriptionForItem(ulong classid, uint instanceid)
    {
        var @class = classid.ToString();
        var instance = instanceid.ToString();
        return GetDescriptionForItem(@class, instance);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="steamid_other">SteamID32</param>
    /// <returns>(Sent, Received)</returns>
    public (Trade[], Trade[]) GetTradeForUser(uint steamid_other)
    {
        var list_sent = new List<Trade>(trade_offers_sent.Length);
        var list_received = new List<Trade>(trade_offers_received.Length);
        foreach (var item in trade_offers_sent)
        {
            if (item.accountid_other == steamid_other) list_sent.Add(item);
        }
        foreach (var item in trade_offers_received)
        {
            if (item.accountid_other == steamid_other) list_received.Add(item);
        }
        return (list_sent.ToArray(), list_received.ToArray());
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="steamid64_other">SteamID64</param>
    /// <returns>(Sent, Received)</returns>
    public (Trade[], Trade[]) GetTradeForUser(ulong steamid64_other)
    {
        var list_sent = new List<Trade>(trade_offers_sent.Length);
        var list_received = new List<Trade>(trade_offers_received.Length);
        steamid64_other = Steam.Steam64ToSteam32(steamid64_other);
        foreach (var item in trade_offers_sent)
        {
            if (item.accountid_other == steamid64_other) list_sent.Add(item);
        }
        foreach (var item in trade_offers_received)
        {
            if (item.accountid_other == steamid64_other) list_received.Add(item);
        }
        return (list_sent.ToArray(), list_received.ToArray());
    }
}
