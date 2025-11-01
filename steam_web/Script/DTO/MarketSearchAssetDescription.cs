using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO;
public class MarketSearchAssetDescription
{
    [JsonPropertyName("appid")] public uint AppId { get; init; } = 0;
    [JsonPropertyName("classid")] public string ClassId { get; init; } = string.Empty;
    [JsonPropertyName("instanceid")] public string InstanceId { get; init; } = string.Empty;
    [JsonPropertyName("name")] public string Name { get; init; } = string.Empty;
    [JsonPropertyName("market_name")] public string MarketName { get; init; } = string.Empty;
    [JsonPropertyName("market_hash_name")] public string MarketHashName { get; init; } = string.Empty;
    [JsonPropertyName("market_tradable_restriction")] public sbyte MarketTradableRestriction { get; init; } = 0;
    [JsonPropertyName("marketable")] public ushort Marketable { get; init; } = 0;
    [JsonPropertyName("tradable")] public ushort Tradable { get; init; } = 0;
    [JsonPropertyName("type")] public string Type { get; init; } = string.Empty;
	[JsonPropertyName("icon_url")] public string IconUrl { get; init; } = string.Empty;
	[JsonPropertyName("icon_url_large")] public string IconUrlLarge { get; init; } = string.Empty;
	[JsonPropertyName("commodity")] public uint Commodity { get; init; } = 0;
    [JsonPropertyName("currency")] public ushort Currency { get; init; } = 0;
    [JsonPropertyName("name_color")] public string NameColor { get; init; } = string.Empty;
    [JsonPropertyName("background_color")] public string BackgroundColor { get; init; } = string.Empty;
}