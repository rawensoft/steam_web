namespace SteamWeb.Script.Models;
public class AjaxOp
{
    /// <summary>
    /// True для продолжения
    /// </summary>
    public bool success { get; init; } = false;
    public bool showResend { get; init; } = false;
    /// <summary>
    /// Следующее состояние
    /// </summary>
    public string? state { get; init; } = null;
    public string? errorText { get; init; } = null;
    public string? token { get; init; } = null;
    /// <summary>
    /// Не null при (state == get_phone_number)
    /// </summary>
    public string? phoneNumber { get; init; } = null;
    /// <summary>
    /// Не 0 при (state == get_sms_code)
    /// </summary>
    public int vac_policy { get; init; } = 0;
    /// <summary>
    /// Не 0 при (state == get_sms_code)
    /// </summary>
    public int tos_policy { get; init; } = 0;
    /// <summary>
    /// Не null при (state == get_sms_code)
    /// </summary>
    public bool? active_locks { get; init; } = null;
    /// <summary>
    /// Не null при (state == get_sms_code)
    /// </summary>
    public bool? phone_tos_violation { get; init; } = null;
    /// <summary>
    /// Не null при (state == email_verification)
    /// </summary>
    public string? inputSize { get; init; } = null;
    /// <summary>
    /// Не null при (state == email_verification)
    /// </summary>
    public string? maxLength { get; init; } = null;
}
