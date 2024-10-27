using System.Text.Json.Serialization;
namespace SteamWeb.API.Models.IEconService;
public class TradesHistory: TradeStatus
{
	/// <summary>
	/// Есть значение если include_total == true
	/// </summary>
	[JsonPropertyName("total_trades")] public uint? TotalTrades { get; set; } = null;
	[JsonPropertyName("more")] public bool More { get; set; } = false;
    
    public List<TradeHistory> GetTradesForUser(ulong steamid64_other)
    {
        var list = new List<TradeHistory>(Trades.Length);
        foreach (var item in Trades)
			if (item.SteamIdOther == steamid64_other)
				list.Add(item);
		return list;
    }
}