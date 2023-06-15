using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.IMobileNotificationService
{
    public class UserNotification
    {
        public ushort user_notification_type { get; set; }
        public uint count {  get; set; }
    }
}
