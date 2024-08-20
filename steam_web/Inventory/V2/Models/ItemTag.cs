using System.Text.Json.Serialization;

namespace SteamWeb.Inventory.V2.Models;
public class ItemTag
{
    [JsonPropertyName("internal_name")] public string InternalName { get; init; }
    [JsonPropertyName("name")] public string Name { get; init; }
    [JsonPropertyName("color")] public string? Color { get; init; }
    [JsonPropertyName("category")] public string Category { get; init; }
    [JsonPropertyName("category_name")] public string CategoryName { get; init; }
}