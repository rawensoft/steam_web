using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IPlayerService;
public class Badge
{
    [JsonPropertyName("badgeid")] public ushort BadgeId { get; init; }
    [JsonPropertyName("level")] public uint Level { get; init; }
    [JsonPropertyName("completion_time")] public int CompletionTime { get; init; }
    [JsonPropertyName("xp")] public uint Xp { get; init; }
    [JsonPropertyName("scarcity")] public uint Scarcity { get; init; }
    [JsonPropertyName("appid")] public uint? AppId { get; init; }
    [JsonPropertyName("communityitemid")] public ulong? CommunityItemId { get; init; }
    [JsonPropertyName("border_color")] public ushort? BorderColor { get; init; }
}