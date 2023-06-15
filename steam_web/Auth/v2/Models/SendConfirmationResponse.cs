using System.Text.Json.Serialization;

namespace SteamWeb.Auth.v2.Models;

internal class SendConfirmationResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; init; } = false;
}

