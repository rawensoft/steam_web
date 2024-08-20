using System.Text.Json.Serialization;

namespace SteamWeb.Inventory.V2.Models;
public sealed class ItemDescription
{
    [JsonPropertyName("type")]public string? Type { get; init; }
    [JsonPropertyName("value")]public string Value { get; init; }
    [JsonPropertyName("color")]public string? Color { get; init; }
    [JsonPropertyName("app_data")] public ItemAppData? AppData { get; init; }
}