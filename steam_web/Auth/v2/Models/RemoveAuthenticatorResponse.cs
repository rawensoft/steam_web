using System.Text.Json.Serialization;

namespace SteamWeb.Auth.v2.Models;

internal class RemoveAuthenticatorResponse
{
    [JsonPropertyName("response")] public RemoveAuthenticatorInternalResponse Response { get; init; } = new();

    internal class RemoveAuthenticatorInternalResponse
    {
        [JsonPropertyName("success")] public bool Success { get; init; } = false;
    }
}

