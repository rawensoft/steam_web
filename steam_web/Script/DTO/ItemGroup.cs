using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO;

public class ItemGroup
{
    [JsonPropertyName("avatarHash")]
    public string? AvatarHash { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("steamid")]
    public ulong SteamId { get; init; }
}
