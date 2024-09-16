using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IPlayerService;
public class FavoriteBadge
{
    [JsonPropertyName("has_favorite_badge")] public bool HasFavoriteBadge { get; init; } = false;
    /// <summary>
    /// Есть значение, если has_favorite_badge == true
    /// </summary>
    [JsonPropertyName("badgeid")] public ushort? BadgeId { get; init; }
    /// <summary>
    /// Есть значение, если has_favorite_badge == true
    /// </summary>
    [JsonPropertyName("level")] public uint? Level { get; init; }
}