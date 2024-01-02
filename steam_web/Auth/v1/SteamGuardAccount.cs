using System.Text.Json.Serialization;
using SteamWeb.Auth.v1.Models;

namespace SteamWeb.Auth.v1;
public class SteamGuardAccount
{
    [JsonPropertyName("shared_secret")] public string SharedSecret { get; init; }
    [JsonPropertyName("serial_number")] public string SerialNumber { get; init; }
    [JsonPropertyName("revocation_code")] public string RevocationCode { get; init; }
    [JsonPropertyName("uri")] public string URI { get; init; }
    [JsonPropertyName("server_time")] public int ServerTime { get; init; }
    [JsonPropertyName("account_name")] public string AccountName { get; init; }
    [JsonPropertyName("token_gid")] public string TokenGID { get; init; }
    [JsonPropertyName("identity_secret")] public string IdentitySecret { get; init; }
    [JsonPropertyName("secret_1")] public string Secret1 { get; init; }
    [JsonPropertyName("status")] public int Status { get; init; }
    [JsonPropertyName("device_id")] public string DeviceID { get; set; }
    /// <summary>
    /// Set to true if the authenticator has actually been applied to the account.
    /// </summary>
    [JsonPropertyName("fully_enrolled")] public bool FullyEnrolled { get; set; } = false;
    public SessionData? Session { get; set; }
}
