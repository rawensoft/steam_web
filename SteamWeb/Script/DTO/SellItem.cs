using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO;

public record SellItem
{
    [JsonPropertyName("success")] public bool Success { get; init; } = false;
    /// <summary>
    /// Нужно подтверждение
    /// </summary>
    [JsonPropertyName("requires_confirmation")] public int Requires_Confirmation { get; init; } = 0;
    /// <summary>
    /// Подтверждение по почте
    /// </summary>
    [JsonPropertyName("needs_email_confirmation")] public bool Needs_Email_Confirmation { get; init; } = false;
    /// <summary>
    /// Подтверждение в SDA
    /// </summary>
    [JsonPropertyName("needs_mobile_confirmation")] public bool Needs_Mobile_Confirmation { get; init; } = false;
    [JsonPropertyName("email_domain")] public string Email_Domain { get; init; }
    public string message { get; init; } = null;
    /// <summary>
    /// Нужно подтверждение
    /// </summary>
    [JsonIgnore] public bool Is_Requires_Confirmation => Requires_Confirmation > 0;
}
