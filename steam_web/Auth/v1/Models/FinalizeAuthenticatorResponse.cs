using System.Text.Json.Serialization;

namespace SteamWeb.Auth.v1;
public partial class AuthenticatorLinker
{
    private class FinalizeAuthenticatorResponse
    {
        [JsonPropertyName("response")] public FinalizeAuthenticatorInternalResponse Response { get; init; }

        internal class FinalizeAuthenticatorInternalResponse
        {
            [JsonPropertyName("status")] public int Status { get; init; }
            [JsonPropertyName("server_time")] public long ServerTime { get; init; }
            [JsonPropertyName("want_more")] public bool WantMore { get; init; }
            [JsonPropertyName("success")] public bool Success { get; init; }
        }
    }
}
