using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.IFriendMessagesService
{
    public class Message
    {
        public uint accountid {  get; set; }
        public int timestamp { get; set; }
        public string message {  get; set; }
    }
}
