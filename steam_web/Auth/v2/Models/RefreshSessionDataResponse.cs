using System.Text.Json.Serialization;

namespace SteamWeb.Auth.v2.Models;

internal class RefreshSessionDataResponse
{
    [JsonPropertyName("response")] public RefreshSessionDataInternalResponse Response { get; init; } = new();

    internal class RefreshSessionDataInternalResponse
    {
        [JsonPropertyName("token")] public string Token { get; init; }
        [JsonPropertyName("token_secure")] public string TokenSecure { get; init; }
    }
}

