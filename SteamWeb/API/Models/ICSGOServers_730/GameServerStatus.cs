using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.ICSGOServers_730
{
    public class GameServerStatus
    {
        public App app { get; set; } = new();
        public Services services { get; set; } = new();
        public Dictionary<string, DataCenter> datacenters { get; set; } = new();
        public Matchmaking matchmaking { get; set; } = new();
        public Dictionary<string, DCPerfectWorld> perfectworld { get; set; } = new();
    }
}
