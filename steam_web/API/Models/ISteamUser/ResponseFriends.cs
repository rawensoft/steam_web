using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.ISteamUser
{
    public class ResponseFriends<T>
    {
        public bool success { get; set; } = false;
        public FriendsList<T> friendslist { get; set; } = new();
    }
}
