namespace SteamWeb.Script.Models;
public class AjaxLicenseInfo
{
    public string? transactionid { get; init; } = null;
    public long packageid { get; init; } = 0;
    public int result_detail { get; init; } = 0;
    public ulong transaction_time { get; init; } = 0;
    public int payment_method { get; init; } = 0;
    public string? base_price { get; init; } = null;
    public string? total_discount { get; init; } = null;
    public string? tax { get; init; } = null;
    public string? shipping { get; init; } = null;
    public int currency_code { get; init; } = 0;
    public string? country_code { get; init; } = null;
    public string? error_headline { get; init; } = null;
    public string? error_string { get; init; } = null;
    public string? error_link_text { get; init; } = null;
    public string? error_link_url { get; init; } = null;
    public int error_appid { get; init; } = 0;
    public AjaxLicenseItem[] line_items { get; init; } = Array.Empty<AjaxLicenseItem>();
}