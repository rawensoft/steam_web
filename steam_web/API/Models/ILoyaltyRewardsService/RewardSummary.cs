using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.ILoyaltyRewardsService
{
    public class RewardSummary
    {
        [JsonPropertyName("summary")] public AccountSummary Summary { get; init; } = new();
        [JsonPropertyName("timestamp_updated")] public int TimeStamp_Updated { get; init; }
        [JsonPropertyName("auditid_highwater")] public string AuditID_HighWater { get; init; }
    }
}
