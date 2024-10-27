using System.Text.Json.Serialization;
namespace SteamWeb.API.Models.IEconService;
public class TradeOffersSummary
{
    [JsonPropertyName("pending_received_count")] public uint PendingReceivedCount { get; init; }
    [JsonPropertyName("new_received_count")] public uint NewReceivedCount { get; init; }
    [JsonPropertyName("updated_received_count")] public uint UpdatedReceivedCount { get; init; }
    [JsonPropertyName("historical_received_count")] public uint HistoricalReceivedCount { get; init; }
    [JsonPropertyName("pending_sent_count")] public uint PendingSentCount { get; init; }
    [JsonPropertyName("newly_accepted_sent_count")] public uint NewlyAcceptedSentCount { get; init; }
    [JsonPropertyName("updated_sent_count")] public uint UpdatedSentCount { get; init; }
    [JsonPropertyName("historical_sent_count")] public uint HistoricalSentCount { get; init; }
    [JsonPropertyName("escrow_received_count")] public uint EscrowReceivedCount { get; init; }
	[JsonPropertyName("escrow_sent_count")] public uint EscrowSentCount { get; init; }
}