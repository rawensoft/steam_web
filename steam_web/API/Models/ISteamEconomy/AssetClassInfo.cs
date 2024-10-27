using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.ISteamEconomy;
public class AssetClassInfo
{
    [JsonPropertyName("icon_url")] public string IconUrl { get; init; } = string.Empty;
    [JsonPropertyName("icon_url_large")] public string IconUrlLarge { get; init; } = string.Empty;
    [JsonPropertyName("icon_drag_url")] public string IconDragUrl { get; init; } = string.Empty;
    [JsonPropertyName("name")] public string Name { get; init; }
    [JsonPropertyName("market_name")] public string MarketName { get; init; } = string.Empty;
    [JsonPropertyName("name_color")] public string NameColor { get; init; } = string.Empty;
    [JsonPropertyName("background_color")] public string BackgroundColor { get; init; } = string.Empty;
    [JsonPropertyName("type")] public string Type { get; init; }
    [JsonPropertyName("tradable")] public byte Tradable { get; init; }
    [JsonPropertyName("marketable")] public byte Narketable { get; init; }
    [JsonPropertyName("commodity")] public byte Commodity { get; init; }
    [JsonPropertyName("classid")] public ulong ClassId { get; init; }
    [JsonPropertyName("instanceid")] public ulong? InstanceId { get; init; }

    [JsonPropertyName("fraudwarnings")]
#if NET8_0_OR_GREATER
    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public Dictionary<string, string> Fraudwarnings { get; } = new(3);
#elif NET5_0_OR_GREATER
    public Dictionary<string, string> Fraudwarnings { get; init; } = new(1);
#endif

    [JsonPropertyName("descriptions")]
#if NET8_0_OR_GREATER
    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public Dictionary<string, string> Descriptions { get; } = new(5);
#elif NET5_0_OR_GREATER
    public Dictionary<string, string> Descriptions { get; init; } = new(1);
#endif

    [JsonPropertyName("actions")]
#if NET8_0_OR_GREATER
    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public Dictionary<string, Inventory.V2.Models.ItemAction> Actions { get; } = new(3);
#elif NET5_0_OR_GREATER
    public Dictionary<string, Inventory.V2.Models.ItemAction> Actions { get; init; } = new(1);
#endif

    [JsonPropertyName("owner_actions")]
#if NET8_0_OR_GREATER
    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public Dictionary<string, Inventory.V2.Models.ItemAction> OwnerActions { get; } = new(5);
#elif NET5_0_OR_GREATER
    public Dictionary<string, Inventory.V2.Models.ItemAction> OwnerActions { get; init; } = new(1);
#endif
}