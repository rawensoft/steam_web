using System.Text.Json.Serialization;

namespace SteamWeb.Auth.v1;
public partial class AuthenticatorLinker
{
    private class AddAuthenticatorResponse
    {
        [JsonPropertyName("response")] public SteamGuardAccount Response { get; init; } = new();
    }
}
