using System.Text.Json.Serialization;

namespace SteamWeb.Inventory.V2.Models;
public class ItemAction
{
    [JsonPropertyName("name")] public string Name { get; init; }
    [JsonPropertyName("link")] public string Link { get; init; }
}