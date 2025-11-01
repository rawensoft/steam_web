using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO;

public class InventoryHistoryApp
{
    [JsonPropertyName("appid")]
    public uint AppId { get; init; }

    [JsonPropertyName("icon")]
    public string Icon { get; init; } = string.Empty;

    [JsonPropertyName("link")]
    public string Link { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}
