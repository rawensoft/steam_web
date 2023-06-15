using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.ICSGOServers_730
{
    public record Matchmaking
    {
        public string scheduler { get; set; } = "normal";
        public int online_servers { get; set; }
        public int online_players { get; set; }
        public int searching_players { get; set; }
        public int search_seconds_avg { get; set; }
    }
}
