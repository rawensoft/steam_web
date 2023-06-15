using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.IMobileNotificationService
{
    public class UserNotificationCounts
    {
        public UserNotification[] notifications { get; set; } = new UserNotification[0];
        public uint account_alert_count { get; set; } = 0;
    }
}
