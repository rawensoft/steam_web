using System.Text.Json.Serialization;

namespace SteamWeb.Auth.v1;
public partial class AuthenticatorLinker
{
    private class AddPhoneResponse
    {
        [JsonPropertyName("success")] public bool Success { get; init; } = false;
    }
}
