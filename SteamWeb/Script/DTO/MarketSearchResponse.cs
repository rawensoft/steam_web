using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO
{
    public class MarketSearchResponse
    {
        [JsonPropertyName("success")] public bool Success { get; init; } = false;
        [JsonPropertyName("start")] public uint Start { get; init; } = 0;
        [JsonPropertyName("pagesize")] public uint PageSize { get; init; } = 0;
        [JsonPropertyName("total_count")] public uint Total_Count { get; init; } = 0;
        [JsonPropertyName("results")] public MarketSearchItem[] Results { get; init; } = new MarketSearchItem[0];
        [JsonPropertyName("searchdata")] public MarketSearchRequest SearchData { get; init; } = new();
    }
}
