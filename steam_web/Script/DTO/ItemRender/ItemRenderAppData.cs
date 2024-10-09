using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO.ItemRender;

public class ItemRenderAppData
{
    [JsonPropertyName("appid")] public uint AppId { get; init; }
    [JsonPropertyName("name")] public string? Name { get; init; }
    [JsonPropertyName("icon")] public string? Icon { get; init; }
    [JsonPropertyName("link")] public string? Link { get; init; }
}
