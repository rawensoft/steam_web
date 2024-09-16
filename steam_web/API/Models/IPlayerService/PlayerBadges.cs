using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IPlayerService;
public class PlayerBadges
{
    [JsonPropertyName("badges")] public Badge[] Badges { get; init; } = Array.Empty<Badge>();
    [JsonPropertyName("player_xp")] public uint PlayerXp { get; init; }
    [JsonPropertyName("player_level")] public ushort PlayerLevel { get; init; }
    [JsonPropertyName("player_xp_needed_to_level_up")] public uint PlayerXpNeededToLevelUp { get; init; }
    [JsonPropertyName("player_xp_needed_current_level")] public uint PlayerXpNeededCurrentLevel { get; init; }
}