using System.Collections.Generic;
using System.Text.Json;
using SteamWeb.Extensions;
using System.Text.Json.Serialization;

namespace SteamWeb.Models;
public class AppContextData
{
    public class ContextItem
    {
        [JsonPropertyName("asset_count")] public ushort AssetsCount { get; init; }
        [JsonPropertyName("id")] public string ContextID { get; init; }
        [JsonPropertyName("name")] public string Name { get; init; }
    }
    [JsonPropertyName("appid")] public uint AppID { get; init; }
    [JsonPropertyName("name")] public string Name { get; init; }
    [JsonPropertyName("asset_count")] public ushort AssetsCount { get; init; }
    /// <summary>
    /// FULL
    /// </summary>
    [JsonPropertyName("trade_permissions")] public string TradePermissions { get; init; }
    [JsonPropertyName("load_failed")] public byte LoadFailed { get; init; } = 0;
    [JsonPropertyName("owner_only")] public bool OwnerOnly { get; init; } = false;
    [JsonPropertyName("rgContexts")] public Dictionary<string, ContextItem> rgContexts { get; init; } = new(3);

    private const string emptyString = "[]";

    public static Dictionary<string, AppContextData> Deserialize(string data)
    {
        string json = data.GetBetween("var g_rgAppContextData = ", ";").Replace("rgContexts\":[]", "rgContexts\":{}");
        if (json == null || json == emptyString)
            return new(1);
        var obj = JsonSerializer.Deserialize<Dictionary<string, AppContextData>>(json);
        return obj;
    }
}
