using System.Text.Json;
using System.Text.Json.Serialization;

namespace SteamWeb.Auth.v1.Models;

internal class LoginResponse<T> where T : IOAuth
{
    [JsonPropertyName("success")]
    public bool Success { get; init; } = false;

    [JsonPropertyName("login_complete")]
    public bool LoginComplete { get; init; } = false;

    [JsonPropertyName("transfer_parameters")]
    public T OAuthData { get; init; } = default;

    [JsonPropertyName("oauth")]
    public string SetOauth { init => OAuthData = JsonSerializer.Deserialize<T>(value); }

    [JsonPropertyName("captcha_needed")]
    public bool CaptchaNeeded { get; init; } = false;//проверить

    [JsonPropertyName("captcha_gid")]
    public long CaptchaGID { get; init; } //проверить

    [JsonPropertyName("emaildomain")]
    public string EmailDomain { get; init; }

    [JsonPropertyName("emailsteamid")]
    public string EmailSteamID { get; init; }

    [JsonPropertyName("emailauth_needed")]
    public bool EmailAuthNeeded { get; init; } = false;

    [JsonPropertyName("requires_twofactor")]
    public bool TwoFactorNeeded { get; init; } = false;

    [JsonPropertyName("message")]
    public string Message { get; init; }

    [JsonPropertyName("clear_password_field")]
    public bool ClearPasswordField { get; init; } = false;
}
