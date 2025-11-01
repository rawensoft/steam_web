using System.Text.Json.Serialization;

namespace SteamWeb.Script.Models;
public class AjaxOp
{
    /// <summary>
    /// True для продолжения
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; init; } = false;

    [JsonPropertyName("showResend")]
    public bool ShowResend { get; init; } = false;

    /// <summary>
    /// Следующее состояние
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; init; } = null;

    [JsonPropertyName("errorText")]
    public string? ErrorText { get; init; } = null;

    [JsonPropertyName("token")]
    public string? Token { get; init; } = null;

    /// <summary>
    /// Не null при (state == get_phone_number)
    /// </summary>
    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; init; } = null;

    /// <summary>
    /// Не 0 при (state == get_sms_code)
    /// </summary>
    [JsonPropertyName("vac_policy")]
    public int VACPolicy { get; init; } = 0;

    /// <summary>
    /// Не 0 при (state == get_sms_code)
    /// </summary>
    [JsonPropertyName("tos_policy")]
    public int TOSPolicy { get; init; } = 0;

    /// <summary>
    /// Не null при (state == get_sms_code)
    /// </summary>
    [JsonPropertyName("active_locks")]
    public bool? ActiveLocks { get; init; } = null;

    /// <summary>
    /// Не null при (state == get_sms_code)
    /// </summary>
    [JsonPropertyName("phone_tos_violation")]
    public bool? PhoneTOSViolation { get; init; } = null;

    /// <summary>
    /// Не null при (state == email_verification)
    /// </summary>
    [JsonPropertyName("inputSize")]
    public string? InputSize { get; init; } = null;

    /// <summary>
    /// Не null при (state == email_verification)
    /// </summary>
    [JsonPropertyName("maxLength")]
    public string? MaxLength { get; init; } = null;
}
