using System.Text.Json.Serialization;
using SteamWeb.Extensions;

namespace SteamWeb.Auth.v1.Models;

internal class OAuthDesktop : IOAuth
{
    [JsonPropertyName("steamid")] public string SteamID { get; init; } = "0";
    [JsonIgnore] public ulong SteamID64 => SteamID.ParseUInt64();
    [JsonPropertyName("auth")] public string OAuthToken { get; init; }
    [JsonPropertyName("token_secure")] public string SteamLoginSecure { get; init; }
    [JsonPropertyName("webcookie")] public string WebCookie { get; init; }
    [JsonPropertyName("remember_login")] public bool RememberLogin { get; init; } = false;
}
