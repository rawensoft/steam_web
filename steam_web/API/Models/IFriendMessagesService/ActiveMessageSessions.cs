using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.IFriendMessagesService
{
    public class ActiveMessageSessions
    {
        public MessageSession[] message_sessions { get; set; } = new MessageSession[0];
        public int timestamp { get; set; }
    }
}
