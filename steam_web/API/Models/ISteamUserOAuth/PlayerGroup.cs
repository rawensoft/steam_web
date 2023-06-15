using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.ISteamUserOAuth
{
    public class PlayerGroup
    {
        public string steamid { get; set; }
        /// <summary>
        /// Member
        /// Blocked
        /// Invited
        /// </summary>
        public string relationship { get; set; }
        /// <summary>
        /// 4 - User
        /// 1 - ?Admin
        /// 128 - ?
        /// 140 - ?
        /// </summary>
        public string permission { get; set; }
    }
}
