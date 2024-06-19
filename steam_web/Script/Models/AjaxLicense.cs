namespace SteamWeb.Script.Models;
public class AjaxLicense
{
    /// <summary>
    /// 1 - активировался
    /// 2 - уже был активирован
    /// 16 - возможно активировался
    /// </summary>
    public int success { get; init; } = 0;
    public int rwgrsn { get; init; } = 0;
    public int purchase_result_details { get; init; } = 0;
    public AjaxLicenseInfo purchase_receipt_info { get; init; } = new();
}