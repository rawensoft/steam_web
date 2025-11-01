using System.Text.Json.Serialization;

namespace SteamWeb.Script.Models;
public class AjaxLicenseInfo
{
    [JsonPropertyName("transactionid")]
    public string? TransactionId { get; init; } = null;

    [JsonPropertyName("packageid")]
    public long PackageId { get; init; } = 0;

    [JsonPropertyName("result_detail")]
    public int ResultDetail { get; init; } = 0;

    [JsonPropertyName("transaction_time")]
    public ulong TransactionTime { get; init; } = 0;

    [JsonPropertyName("payment_method")]
    public int PaymentMethod { get; init; } = 0;

    [JsonPropertyName("base_price")]
    public string? BasePrice { get; init; } = null;

    [JsonPropertyName("total_discount")]
    public string? TotalDiscount { get; init; } = null;

    [JsonPropertyName("tax")]
    public string? Tax { get; init; } = null;

    [JsonPropertyName("shipping")]
    public string? Shipping { get; init; } = null;

    [JsonPropertyName("currency_code")]
    public int CurrencyCode { get; init; } = 0;

    [JsonPropertyName("country_code")]
    public string? CountryCode { get; init; } = null;

    [JsonPropertyName("error_headline")]
    public string? ErrorHeadline { get; init; } = null;

    [JsonPropertyName("error_string")]
    public string? ErrorString { get; init; } = null;

    [JsonPropertyName("error_link_text")]
    public string? ErrorLinkText { get; init; } = null;

    [JsonPropertyName("error_link_url")]
    public string? ErrorLinkUrl { get; init; } = null;

    [JsonPropertyName("error_appid")]
    public uint ErrorAppId { get; init; } = 0;

    [JsonPropertyName("line_items")]
    public AjaxLicenseItem[] LineItems { get; init; } = Array.Empty<AjaxLicenseItem>();
}