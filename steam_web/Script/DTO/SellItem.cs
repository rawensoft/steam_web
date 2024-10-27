using System.Text.Json.Serialization;
namespace SteamWeb.Script.DTO;
public class SellItem
{
    [JsonPropertyName("success")] public bool Success { get; init; } = false;
    /// <summary>
    /// Нужно подтверждение
    /// </summary>
    [JsonPropertyName("requires_confirmation")] public int RequiresConfirmation { get; init; } = 0;
    /// <summary>
    /// Подтверждение по почте
    /// </summary>
    [JsonPropertyName("needs_email_confirmation")] public bool NeedsEmailConfirmation { get; init; } = false;
    /// <summary>
    /// Подтверждение в SDA
    /// </summary>
    [JsonPropertyName("needs_mobile_confirmation")] public bool NeedsMobileConfirmation { get; init; } = false;
    [JsonPropertyName("email_domain")] public string? EmailDomain { get; init; }
    public string? message { get; init; } = null;
    /// <summary>
    /// Нужно подтверждение
    /// </summary>
    [JsonIgnore] public bool IsRequiresConfirmation => RequiresConfirmation > 0;
}