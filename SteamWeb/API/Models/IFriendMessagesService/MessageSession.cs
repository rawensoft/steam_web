using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.IFriendMessagesService
{
    public class MessageSession
    {
        /// <summary>
        /// SteamID32
        /// </summary>
        public uint accountid_friend {  get; set; }
        public int last_message { get; set; }
        public int last_view { get; set; }
        public uint unread_message_count { get; set; }
    }
}
