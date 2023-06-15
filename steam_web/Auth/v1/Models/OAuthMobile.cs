using System.Text.Json.Serialization;
using SteamWeb.Extensions;

namespace SteamWeb.Auth.v1.Models;

internal class OAuthMobile : IOAuth
{
    [JsonPropertyName("account_name")] public string AccountName { get; init; }
    [JsonPropertyName("steamid")] public string SteamID { get; init; } = "0";
    [JsonIgnore] public ulong SteamID64 => SteamID.ParseUInt64();
    [JsonPropertyName("oauth_token")] public string OAuthToken { get; init; }
    [JsonPropertyName("wgtoken")] public string SteamLogin { get; init; }
    [JsonPropertyName("wgtoken_secure")] public string SteamLoginSecure { get; init; }
    [JsonPropertyName("webcookie")] public string WebCookie { get; init; }
}
