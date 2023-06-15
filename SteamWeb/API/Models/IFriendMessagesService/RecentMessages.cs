using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.IFriendMessagesService
{
    public class RecentMessages
    {
        public Message[] messages { get; set; } = new Message[0];
        public bool more_available { get; set; } = false;
    }
}
