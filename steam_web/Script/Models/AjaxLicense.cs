using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.Script.Models
{
    public class AjaxLicense
    {
        /// <summary>
        /// 1 - активировался
        /// 2 - уже был активирован
        /// 16 - возможно активировался
        /// </summary>
        public int success { get; set; } = 0;
        public int rwgrsn { get; set; } = 0;
        public int purchase_result_details { get; set; } = 0;
        public AjaxLicenseInfo purchase_receipt_info { get; set; } = new();
    }
}
