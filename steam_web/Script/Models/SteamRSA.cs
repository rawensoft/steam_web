using System.Text.Json.Serialization;

namespace SteamWeb.Script.Models;
public class SteamRSA
{
    [JsonPropertyName("success")]
    public bool Success { get; init; } = false;

    [JsonPropertyName("publickey_mod")]
    public string? PublicKeyMod { get; init; } = null;

    [JsonPropertyName("publickey_exp")]
    public string? PublicKeyExp { get; init; } = null;

    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; init; } = null;

    [JsonPropertyName("token_gid")]
    public string? TokenGid { get; init; } = null;
}
