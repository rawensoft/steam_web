using System.Text.Json.Serialization;
namespace SteamWeb.API.Models.IEconService;
public class TradeHistory
{
	/// <summary>
	/// a unique identifier for the trade/ Пример этого 3749786070834469105.
	/// </summary>
	[JsonPropertyName("tradeid")] public ulong TradeId { get; init; }
	/// <summary>
	/// the SteamID64 of your trade partner
	/// </summary>
	[JsonPropertyName("steamid_other")] public ulong SteamIdOther { get; init; }
	/// <summary>
	/// unix timestamp of the time the trade started to commit
	/// </summary>
	[JsonPropertyName("time_init")] public int TimeInit { get; init; }
    [JsonPropertyName("time_mod")] public int TimeMod { get; init; }
    /// <summary>
    /// unix timestamp of the time the trade will leave escrow
    /// </summary>
    [JsonPropertyName("time_escrow_end")] public int? TimeEscrowEnd { get; init; }
	[JsonPropertyName("status")] public Enums.ETradeStatus Status { get; init; } = Enums.ETradeStatus.k_ETradeStatus_Failed;
	[JsonPropertyName("assets_given")] public List<TradeHistoryAssets> AssetsGiven { get; init; } = new(3);
	[JsonPropertyName("assets_received")] public List<TradeHistoryAssets> AssetsReceived { get; init; } = new(3);
    /// <summary>
    /// Время, когда trade protected items перейдут во free инвентарь (разблокируются)
    /// </summary>
    [JsonPropertyName("time_settlement")] public int TimeSettlement { get; init; }

    /// <summary>
    /// Трейд содержит предметы только для отправки
    /// </summary>
    [JsonIgnore] public bool OnlyGiven => AssetsGiven.Count != 0 && AssetsReceived.Count == 0;
    /// <summary>
    /// Трейд содержит предметы только для меня
    /// </summary>
    [JsonIgnore] public bool OnlyReceive => AssetsReceived.Count != 0 && AssetsGiven.Count == 0;
	/// <summary>
	/// Это полноценный обмен. С обеих сторон имеются предметы.
	/// </summary>
	[JsonIgnore] public bool IsExchange => AssetsGiven.Count > 0 && AssetsReceived.Count > 0;
}