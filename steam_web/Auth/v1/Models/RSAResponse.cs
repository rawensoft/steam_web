using System.Text.Json.Serialization;

namespace SteamWeb.Auth.v1.Models;

internal class RSAResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; init; } = false;

    [JsonPropertyName("publickey_exp")]
    public string Exponent { get; init; }

    [JsonPropertyName("publickey_mod")]
    public string Modulus { get; init; }

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; init; }

    [JsonPropertyName("token_gid")]
    public string token_gid { get; init; }
}
