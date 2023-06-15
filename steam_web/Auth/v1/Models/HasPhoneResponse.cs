using System.Text.Json.Serialization;

namespace SteamWeb.Auth.v1;
public partial class AuthenticatorLinker
{
    private class HasPhoneResponse
    {
        [JsonPropertyName("has_phone")] public bool HasPhone { get; init; }
    }
}
