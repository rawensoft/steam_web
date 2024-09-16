using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.ISteamUser;
public class PlayerSummary
{
    [JsonPropertyName("steamid")] public ulong SteamId { get; init; }
    [JsonPropertyName("communityvisibilitystate")] public int CommunityVisibilityState { get; init; }
    [JsonPropertyName("profilestate")] public int ProfileState { get; init; }
    [JsonPropertyName("personaname")] public string PersonaName { get; init; } = string.Empty;
    [JsonPropertyName("commentpermission")] public int CommentPermission { get; init; }
    [JsonPropertyName("profileurl")] public string? ProfileUrl { get; init; }
    [JsonPropertyName("avatar")] public string? Avatar { get; init; }
    [JsonPropertyName("avatarmedium")] public string? AvatarMedium { get; init; }
    [JsonPropertyName("avatarfull")] public string? AvatarFull { get; init; }
    [JsonPropertyName("avatarhash")] public string? AvatarHash { get; init; }
    [JsonPropertyName("lastlogoff")] public int LastLogoff { get; init; }
    [JsonPropertyName("personastate")] public int PersonaState { get; init; }
    [JsonPropertyName("realname")] public string? RealName { get; init; }
    [JsonPropertyName("primaryclanid")] public string? PrimaryClanId { get; init; }
    [JsonPropertyName("timecreated")] public int TimeCreated { get; init; }
    [JsonPropertyName("personastateflags")] public int PersonaStateFlags { get; init; }
    [JsonPropertyName("loccountrycode")] public string? LocationCountryCode { get; init; }
    [JsonPropertyName("locstatecode")] public string? LocationStateCode { get; init; }
    [JsonPropertyName("loccityid")] public int LocationCityId { get; init; }
}