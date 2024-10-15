using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO;
public class MarketSearchResponse
{
    [JsonPropertyName("success")] public bool Success { get; init; } = false;
	[JsonPropertyName("is_429")] public bool IsTooManyRequests { get; init; } = false;
	[JsonPropertyName("start")] public uint Start { get; init; } = 0;
    [JsonPropertyName("pagesize")] public uint PageSize { get; init; } = 0;
    [JsonPropertyName("total_count")] public uint TotalCount { get; init; } = 0;
    [JsonPropertyName("results")] public MarketSearchItem[] Results { get; init; } = Array.Empty<MarketSearchItem>();
    [JsonPropertyName("searchdata")] public MarketSearchRequest SearchData { get; init; } = new();
}