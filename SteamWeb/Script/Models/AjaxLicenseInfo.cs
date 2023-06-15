namespace SteamWeb.Script.Models
{
    public class AjaxLicenseInfo
    {
        public string transactionid { get; set; } = null;
        public long packageid { get; set; } = 0;
        public int result_detail { get; set; } = 0;
        public ulong transaction_time { get; set; } = 0;
        public int payment_method { get; set; } = 0;
        public string base_price { get; set; } = null;
        public string total_discount { get; set; } = null;
        public string tax { get; set; } = null;
        public string shipping { get; set; } = null;
        public int currency_code { get; set; } = 0;
        public string country_code { get; set; } = null;
        public string error_headline { get; set; } = null;
        public string error_string { get; set; } = null;
        public string error_link_text { get; set; } = null;
        public string error_link_url { get; set; } = null;
        public int error_appid { get; set; } = 0;
        public AjaxLicenseItem[] line_items { get; set; } = new AjaxLicenseItem[0];
    }
}
