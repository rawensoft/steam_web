using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.ILoyaltyRewardsService;
public class AccountSummary
{
    /// <summary>
    /// Текущее кол поинтов
    /// </summary>
    [JsonPropertyName("points")] public uint Points { get; init; }
    /// <summary>
    /// Поинтов получено
    /// </summary>
    [JsonPropertyName("points_earned")] public uint PointsEarned { get; init; }
    /// <summary>
    /// Поинтов потрачено
    /// </summary>
    [JsonPropertyName("points_spent")] public uint PointsSpent { get; init; }
}