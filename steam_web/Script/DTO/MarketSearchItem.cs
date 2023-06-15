using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO
{
    public class MarketSearchItem
    {
        [JsonPropertyName("app_icon")] public string App_Icon { get; init; } = null;
        [JsonPropertyName("app_name")] public string App_Name { get; init; } = null;
        [JsonPropertyName("name")] public string Name { get; init; } = null;
        [JsonPropertyName("hash_name")] public string Hash_Name { get; init; } = null;
        [JsonPropertyName("sell_listings")] public uint Sell_Listings { get; init; } = 0;
        [JsonPropertyName("sell_price")] public uint Sell_Price { get; init; } = 0;
        [JsonPropertyName("sell_price_text")] public string Sell_Price_Text { get; init; } = null;
        [JsonPropertyName("sale_price_text")] public string Sale_Price_Text { get; init; } = null;
        [JsonPropertyName("asset_description")] public MarketSearchAssetDescription Asset_Description { get; init; } = new();
    }
}
