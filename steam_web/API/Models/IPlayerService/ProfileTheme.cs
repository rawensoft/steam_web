using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IPlayerService;
public class ProfileTheme
{
    /// <summary>
    /// Example: DarkMode
    /// </summary>
    [JsonPropertyName("theme_id")] public string? ThemeId { get; init; }
    /// <summary>
    /// Example: #ProfileTheme_DarkMode
    /// </summary>
    [JsonPropertyName("title")] public string? Title { get; init; }
}