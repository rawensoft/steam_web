using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.Script.Models
{
    public class AjaxOp
    {
        /// <summary>
        /// True для продолжения
        /// </summary>
        public bool success { get; set; } = false;
        public bool showResend { get; set; } = false;
        /// <summary>
        /// Следующее состояние
        /// </summary>
        public string state { get; set; } = null;
        public string errorText { get; set; } = null;
        public string token { get; set; } = null;
        /// <summary>
        /// Не null при (state == get_phone_number)
        /// </summary>
        public string phoneNumber { get; set; } = null;
        /// <summary>
        /// Не 0 при (state == get_sms_code)
        /// </summary>
        public int vac_policy { get; set; } = 0;
        /// <summary>
        /// Не 0 при (state == get_sms_code)
        /// </summary>
        public int tos_policy { get; set; } = 0;
        /// <summary>
        /// Не null при (state == get_sms_code)
        /// </summary>
        public bool? active_locks { get; set; } = null;
        /// <summary>
        /// Не null при (state == get_sms_code)
        /// </summary>
        public bool? phone_tos_violation { get; set; } = null;
        /// <summary>
        /// Не null при (state == email_verification)
        /// </summary>
        public string inputSize { get; set; } = null;
        /// <summary>
        /// Не null при (state == email_verification)
        /// </summary>
        public string maxLength { get; set; } = null;
    }
}
