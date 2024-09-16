using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.ILoyaltyRewardsService;
public class RewardSummary
{
    [JsonPropertyName("summary")] public AccountSummary Summary { get; init; } = new();
    [JsonPropertyName("timestamp_updated")] public int TimestampUpdated { get; init; }
    [JsonPropertyName("auditid_highwater")] public ulong AuditIdHighWater { get; init; }
}