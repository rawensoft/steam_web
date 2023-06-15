using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO
{
    public class MarketSearchAssetDescription
    {
        [JsonPropertyName("appid")] public uint appid { get; init; } = 0;
        [JsonPropertyName("classid")] public string classid { get; init; } = null;
        [JsonPropertyName("instanceid")] public string instanceid { get; init; } = null;
        [JsonPropertyName("name")] public string name { get; init; } = null;
        [JsonPropertyName("market_name")] public string market_name { get; init; } = null;
        [JsonPropertyName("market_hash_name")] public string market_hash_name { get; init; } = null;
        [JsonPropertyName("market_tradable_restriction")] public uint market_tradable_restriction { get; init; } = 0;
        [JsonPropertyName("marketable")] public ushort marketable { get; init; } = 0;
        [JsonPropertyName("tradable")] public ushort tradable { get; init; } = 0;
        [JsonPropertyName("type")] public string type { get; init; } = null;
        [JsonPropertyName("commodity")] public uint commodity { get; init; } = 0;
        [JsonPropertyName("currency")] public ushort currency { get; init; } = 0;
    }
}
