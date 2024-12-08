using System.Text.Json.Serialization;
using SteamWeb.Inventory.V2.Models;

namespace SteamWeb.Script.DTO.ItemRender;

public class ItemRenderListingAsset
{
    [JsonPropertyName("currency")] public byte Currency { get; init; }
    [JsonPropertyName("appid")] public uint AppId { get; init; }
    [JsonPropertyName("contextid")] public ulong ContextId { get; init; }
    [JsonPropertyName("id")] public ulong Id { get; init; }
    [JsonPropertyName("amount")] public ushort Amount { get; init; }
    [JsonPropertyName("market_actions")] public ItemAction[] MarketActions { get; init; } = Array.Empty<ItemAction>();
}
