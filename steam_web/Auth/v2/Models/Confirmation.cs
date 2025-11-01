using SteamWeb.Auth.v1.Enums;
using System.Text.Json.Serialization;

namespace SteamWeb.Auth.v2.Models;

public class Confirmation
{
    [JsonPropertyName("creation_time")]
    public int CreationTime { get; init; }

    [JsonPropertyName("type")]
    public EMobileConfirmationType Type { get; init; }

    [JsonPropertyName("type_name")]
    public string TypeName { get; init; }

    [JsonPropertyName("id")]
    public string Id { get; init; }

    [JsonPropertyName("creator_id")]
    public ulong CreatorId { get; init; }

    [JsonPropertyName("nonce")]
    public string Nonce { get; init; }

    [JsonPropertyName("accept")]
    public string Accept { get; init; }

    [JsonPropertyName("cancel")]
    public string Cancel { get; init; }

    [JsonPropertyName("icon")]
    public string Icon { get; init; } = string.Empty;

    [JsonPropertyName("multi")]
    public bool Multi { get; init; } = false;

    [JsonPropertyName("headline")]
    public string Headline { get; init; } = string.Empty;

    [JsonPropertyName("summary")]
    public string[] Summary { get; init; } = Array.Empty<string>();

    [JsonPropertyName("warn")]
    public string[] Warn { get; init; } = Array.Empty<string>();
}