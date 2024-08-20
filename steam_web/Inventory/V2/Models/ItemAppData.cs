using System.Text.Json.Serialization;
namespace SteamWeb.Inventory.V2.Models;
public sealed class ItemAppData
{
    [JsonPropertyName("quantity")] public byte? Quantity { get; init; }
    [JsonPropertyName("quality")] public byte? Quality { get; init; }
    [JsonPropertyName("def_index")] public ushort? DefIndex { get; init; }
    [JsonPropertyName("is_itemset_name")] public int? IsItemsetName { get; init; }
    [JsonPropertyName("limited")] public byte? limited { get; init; }
}