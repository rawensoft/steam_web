using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.ISteamUser
{
    public record PlayerFriend
    {
        public string steamid { get; set; }
        /// <summary>
        /// friend
        /// requestrecipient
        /// ignored
        /// </summary>
        public string relationship { get; set; }
        public int friend_since { get; set; }
    }
}
