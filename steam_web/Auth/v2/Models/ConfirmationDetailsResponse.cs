using System.Text.Json.Serialization;

namespace SteamWeb.Auth.v2.Models;

internal class ConfirmationDetailsResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; init; } = false;

    [JsonPropertyName("html")]
    public string HTML { get; init; }
}

