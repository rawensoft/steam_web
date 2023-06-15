using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.ICSGOServers_730
{
    public record Services
    {
        /// <summary>
        /// Sessions Logon
        /// </summary>
        public string SessionsLogon { get; set; } = "offline";
        public string SteamCommunity { get; set; } = "offline";
        /// <summary>
        /// Game Coordinator
        /// </summary>
        public string IEconItems { get; set; } = "offline";
        public string Leaderboards { get; set; } = "offline";
    }
}
