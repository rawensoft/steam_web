using System.Text.Json;
using SteamWeb.Extensions;
using System.Text.Json.Serialization;

namespace SteamWeb.Models;
public class AppContextData
{
    private const string emptyString = "[]";

    [JsonPropertyName("appid")] public uint AppId { get; init; }
    [JsonPropertyName("name")] public string Name { get; init; }
    [JsonPropertyName("asset_count")] public ushort AssetsCount { get; init; }
    /// <summary>
    /// FULL
    /// </summary>
    [JsonPropertyName("trade_permissions")] public string TradePermissions { get; init; }
    [JsonPropertyName("load_failed")] public byte LoadFailed { get; init; } = 0;
    [JsonPropertyName("owner_only")] public bool OwnerOnly { get; init; } = false;
    [JsonPropertyName("rgContexts")] public Dictionary<byte, ContextItem> rgContexts { get; init; } = new(3);

    public static Dictionary<uint, AppContextData> Deserialize(string data)
    {
        string? json = data.GetBetween("var g_rgAppContextData = ", ";")?.Replace("rgContexts\":[]", "rgContexts\":{}");
        if (json == null || json == emptyString)
            return new(1);
        var obj = JsonSerializer.Deserialize<Dictionary<uint, AppContextData>>(json, Steam.JsonOptions)!;
        return obj;
    }
}
public class ContextItem
{
    [JsonPropertyName("asset_count")] public ushort AssetsCount { get; init; }
    [JsonPropertyName("id")] public byte ContextId { get; init; }
    [JsonPropertyName("name")] public string Name { get; init; }
}