using System.Text.Json.Serialization;
namespace SteamWeb.API.Models.IEconService;
public class TradeHold
{
	[JsonPropertyName("escrow_end_duration_seconds")] public uint EscrowEndDurationSeconds { get; init; }
	/// <summary>
	/// Не null при escrow_end_duration_seconds > 0
	/// </summary>
	[JsonPropertyName("escrow_end_date")] public int? EscrowEndDate { get; init; }
	/// <summary>
	/// Не null при escrow_end_duration_seconds > 0. Пример: 2022-12-27T12:20:00Z
	/// </summary>
	[JsonPropertyName("escrow_end_date_rfc3339")] public DateTime? EscrowEndDateRfc3339 { get; init; }
}