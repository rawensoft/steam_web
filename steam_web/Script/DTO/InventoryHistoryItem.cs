using SteamWeb.Inventory.V2.Models;
using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO;

public class InventoryHistoryItem
{
    [JsonPropertyName("commodity")]
    public sbyte Commodity { get; init; }

    [JsonPropertyName("background_color")]
    public string BackgroundColor { get; init; } = string.Empty;

    [JsonPropertyName("classid")]
    public ulong ClassId { get; init; }

    [JsonPropertyName("icon_drag_url")]
    public string IconDragUrl { get; init; }

    [JsonPropertyName("icon_url")]
    public string IconUrl { get; init; }

    [JsonPropertyName("icon_url_large")]
    public string IconUrlLarge { get; init; }

    [JsonPropertyName("instanceid")]
    public ulong InstanceId { get; init; }

    [JsonPropertyName("market_fee_app")]
    public string MarketFeeApp { get; init; }

    [JsonPropertyName("market_hash_name")]
    public string MarketHashName { get; init; }

    [JsonPropertyName("market_marketable_restriction")]
    public string MarketMarketableRestriction { get; init; }

    [JsonPropertyName("market_name")]
    public string MarketName { get; init; }

    [JsonPropertyName("market_tradable_restriction")]
    public string MarketTradableRestriction { get; init; }

    [JsonPropertyName("marketable")]
    public sbyte Marketable { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("name_color")]
    public string NameColor { get; init; }

    [JsonPropertyName("tradable")]
    public sbyte Tradable { get; init; }

    [JsonPropertyName("type")]
    public string Type { get; init; }

    [JsonPropertyName("tags")]
    public ItemTag[] Tags { get; init; } = Array.Empty<ItemTag>();
}
