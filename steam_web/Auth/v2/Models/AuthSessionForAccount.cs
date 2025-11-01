using System.Text.Json.Serialization;

namespace SteamWeb.Auth.v2.Models;

public class AuthSessionForAccount
{
    [JsonPropertyName("client_ids")]
    public string[] ClientIds { get; init; } = Array.Empty<string>();
}