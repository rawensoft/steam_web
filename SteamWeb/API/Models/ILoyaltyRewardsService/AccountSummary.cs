using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.ILoyaltyRewardsService
{
    public class AccountSummary
    {
        /// <summary>
        /// Текущее кол поинтов
        /// </summary>
        [JsonPropertyName("points")] public string Points { get; init; } = "0";
        /// <summary>
        /// Поинтов получено
        /// </summary>
        [JsonPropertyName("points_earned")] public string Points_Earned { get; init; } = "0";
        /// <summary>
        /// Поинтов потрачено
        /// </summary>
        [JsonPropertyName("points_spent")] public string Points_Spent { get; init; } = "0";

        /// <summary>
        /// Текущее кол поинтов (если не удалось спарсить значение, то 0)
        /// </summary>
        [JsonIgnore] public uint i_Points => uint.TryParse(Points, out var result) ? result : 0;
        /// <summary>
        /// Поинтов получено (если не удалось спарсить значение, то 0)
        /// </summary>
        [JsonIgnore] public uint i_Points_Earned => uint.TryParse(Points_Earned, out var result) ? result : 0;
        /// <summary>
        /// Поинтов потрачено (если не удалось спарсить значение, то 0)
        /// </summary>
        [JsonIgnore] public uint i_Points_Spent => uint.TryParse(Points_Spent, out var result) ? result : 0;
    }
}
