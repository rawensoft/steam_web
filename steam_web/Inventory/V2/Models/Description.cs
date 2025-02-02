using System.Text.Json.Serialization;
namespace SteamWeb.Inventory.V2.Models;
public class Description
{
    [JsonPropertyName("appid")] public uint AppId { get; init; }
    [JsonPropertyName("classid")] public ulong ClassId { get; init; }
    [JsonPropertyName("instanceid")] public ulong InstanceId { get; init; }
    [JsonPropertyName("icon_url")] public string? IconUrl { get; init; }
    [JsonPropertyName("icon_url_large")] public string? IconUrlLarge { get; init; }
    [JsonPropertyName("icon_drag_url")] public string? IconFragUrl { get; init; }
    [JsonPropertyName("name")] public string Name { get; init; }
    [JsonPropertyName("market_hash_name")] public string MarketHashName { get; init; }
    [JsonPropertyName("market_name")] public string MarketName { get; init; }
    [JsonPropertyName("name_color")] public string NameColor { get; init; }
    [JsonPropertyName("background_color")] public string BackgroundColor { get; init; } = string.Empty;
    [JsonPropertyName("type")] public string Type { get; init; }
    [JsonPropertyName("tradable")] public sbyte Tradable { get; init; }
    [JsonPropertyName("marketable")] public sbyte Marketable { get; init; }
    [JsonPropertyName("commodity")] public sbyte Commodity { get; init; }
    [JsonPropertyName("market_tradable_restriction")] public sbyte MarketTradableRestriction { get; init; }
    [JsonPropertyName("market_marketable_restriction")] public sbyte MarketMarketableRestriction { get; init; }
    /// <summary>
    /// Время истечения ограничений на трейдинг
    /// </summary>
    [JsonPropertyName("cache_expiration")] public DateTime? CacheExpiration { get; init; }
    [JsonPropertyName("descriptions")] public ItemDescription[] Descriptions { get; init; } = Array.Empty<ItemDescription>();
    [JsonPropertyName("owner_descriptions")] public ItemDescription[] OwnerDescriptions { get; init; } = Array.Empty<ItemDescription>();
    [JsonPropertyName("actions")] public ItemAction[] Actions { get; init; } = Array.Empty<ItemAction>();
    [JsonPropertyName("market_actions")] public ItemAction[] MarketActions { get; init; } = Array.Empty<ItemAction>();
    [JsonPropertyName("tags")] public ItemTag[] Tags { get; init; } = Array.Empty<ItemTag>();
    /// <summary>
    /// Только TF2/440
    /// </summary>
    [JsonPropertyName("app_data")] public ItemAppData? AppData { get; init; }

    [JsonIgnore]
    public bool IsTradable => Tradable != 0;
    [JsonIgnore]
    public bool IsMarketable => Marketable != 0;

    public ItemTag? GetTagByCategory(string category)
    {
        foreach (var tag in Tags)
            if (tag.Category == category)
                return tag;
        return null;
    }
}