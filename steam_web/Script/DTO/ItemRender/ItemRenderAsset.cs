using System.Text.Json.Serialization;
using SteamWeb.Inventory.V2.Models;

namespace SteamWeb.Script.DTO.ItemRender;

public class ItemRenderAsset
{
    [JsonPropertyName("currency")] public byte Currency { get; init; }
    [JsonPropertyName("appid")] public uint AppId { get; init; }
    [JsonPropertyName("contextid")] public byte ContextId { get; init; }
    [JsonPropertyName("id")] public ulong Id { get; init; }
    [JsonPropertyName("classid")] public ulong ClassId { get; init; }
    [JsonPropertyName("instanceid")] public ulong InstanceId { get; init; }
    [JsonPropertyName("amount")] public ushort Amount { get; init; }
    [JsonPropertyName("status")] public ushort Status { get; init; }
    [JsonPropertyName("original_amount")] public ushort OriginalAmount { get; init; }
    [JsonPropertyName("unowned_id")] public ulong UnownedId { get; init; }
    [JsonPropertyName("unowned_contextid")] public byte UnownedContextId { get; init; }
    [JsonPropertyName("background_color")] public string BackgroundColor { get; init; } = string.Empty;
    [JsonPropertyName("icon_url")] public string? IconUrl { get; init; }
    [JsonPropertyName("tradable")] public byte Tradable { get; init; }
    [JsonPropertyName("actions")] public ItemAction[] Actions { get; init; } = Array.Empty<ItemAction>();
    [JsonPropertyName("name")] public string? Name { get; init; }
    [JsonPropertyName("type")] public string? Type { get; init; }
    [JsonPropertyName("market_name")] public string? MarketName { get; init; }
    [JsonPropertyName("market_hash_name")] public string? MarketHashName { get; init; }
    [JsonPropertyName("market_actions")] public ItemAction[] MarketActions { get; init; } = Array.Empty<ItemAction>();
    [JsonPropertyName("app_icon")] public string? AppIcon { get; init; }
    [JsonPropertyName("commodity")] public byte Commodity { get; init; }
    [JsonPropertyName("market_tradable_restriction")] public byte MarketTradableRestriction { get; init; }
    [JsonPropertyName("market_marketable_restriction")] public byte MarketMarketableRestriction { get; init; }
    [JsonPropertyName("marketable")] public byte Marketable { get; init; }
}
