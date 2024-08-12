using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO;
public class MarketSearchItem
{
    [JsonPropertyName("app_icon")] public string AppIcon { get; init; } = string.Empty;
    [JsonPropertyName("app_name")] public string AppName { get; init; } = string.Empty;
    [JsonPropertyName("name")] public string Name { get; init; } = string.Empty;
    [JsonPropertyName("hash_name")] public string HashName { get; init; } = string.Empty;
    [JsonPropertyName("sell_listings")] public uint SellListings { get; init; } = 0;
    [JsonPropertyName("sell_price")] public uint SellPrice { get; init; } = 0;
    [JsonPropertyName("sell_price_text")] public string SellPriceText { get; init; } = string.Empty;
    [JsonPropertyName("sale_price_text")] public string SalePriceText { get; init; } = string.Empty;
    [JsonPropertyName("asset_description")] public MarketSearchAssetDescription AssetDescription { get; init; } = new();
}