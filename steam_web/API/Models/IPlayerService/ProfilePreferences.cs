using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IPlayerService;
public class ProfilePreferences
{
    [JsonPropertyName("hide_profile_awards")] public bool HideProfileAwards { get; init; } = false;
}