using System.Text.Json.Serialization;
namespace SteamWeb.API.Models.IEconService;
public class TradeHistoryDescription
{
    [JsonPropertyName("type")] public string? Type { get; init; }
    [JsonPropertyName("value")] public string? Value { get; init; }
    [JsonPropertyName("color")] public string? Color { get; init; }
    [JsonPropertyName("appid")] public uint AppId { get; init; }
    [JsonPropertyName("classid")] public ulong ClassId { get; init; }
    [JsonPropertyName("instanceid")] public ulong InstanceId { get; init; }
    [JsonPropertyName("currency")] public bool Currency { get; init; } = false;
    [JsonPropertyName("background_color")] public string BackgroundColor { get; init; } = string.Empty;
    [JsonPropertyName("icon_url")] public string? IconUrl { get; init; }
    [JsonPropertyName("icon_url_large")] public string? IconUrlLarge { get; init; }
	[JsonPropertyName("tradable")] public bool Tradable { get; init; } = false;
	[JsonPropertyName("name")] public string? Name { get; init; }
    [JsonPropertyName("name_color")] public string? NameColor { get; init; }
    [JsonPropertyName("market_name")] public string? MarketName { get; init; }
    [JsonPropertyName("market_hash_name")] public string? MarketHashName { get; init; }
	[JsonPropertyName("commodity")] public bool Commodity { get; init; } = false;
    [JsonPropertyName("market_tradable_restriction")] public sbyte MarketTradableRestriction { get; init; }
    [JsonPropertyName("market_marketable_restriction")] public sbyte MarketMarketableRestriction { get; init; }
    [JsonPropertyName("marketable")] public bool Marketable { get; init; } = false;
	[JsonPropertyName("sealed")] public bool Sealed { get; init; } = false;
}