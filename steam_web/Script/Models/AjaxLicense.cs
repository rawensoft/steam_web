using System.Text.Json.Serialization;

namespace SteamWeb.Script.Models;
public class AjaxLicense
{
    /// <summary>
    /// 1 - активировался
    /// 2 - уже был активирован
    /// 16 - возможно активировался
    /// </summary>
    [JsonPropertyName("success")]
    public EResult Success { get; init; } = EResult.Invalid;

    [JsonPropertyName("rwgrsn")]
    public EResult Rwgrsn { get; init; } = 0;

    [JsonPropertyName("purchase_result_details")]
    public int PurchaseResultDetails { get; init; } = 0;

    [JsonPropertyName("purchase_receipt_info")]
    public AjaxLicenseInfo PurchaseReceiptInfo { get; init; } = new();
}