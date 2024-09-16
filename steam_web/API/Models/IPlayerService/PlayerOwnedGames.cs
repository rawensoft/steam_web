using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IPlayerService;
public class PlayerOwnedGames
{
    [JsonPropertyName("game_count")] public ushort GameCount { get; init; }
    [JsonPropertyName("games")] public OwnedGame[] Games { get; init; } = Array.Empty<OwnedGame>();
}