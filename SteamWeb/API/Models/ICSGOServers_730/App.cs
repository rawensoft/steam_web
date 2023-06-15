using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.ICSGOServers_730
{
    public record App
    {
        public int version { get; set; }
        public int timestamp { get; set; }
        public string time { get; set; }
    }
}
