using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.ICSGOServers_730
{
    public record DCPerfectWorld
    {
        public string availability { get; set; } = "offline";
        public string latency { get; set; } = "idle";
    }
}
