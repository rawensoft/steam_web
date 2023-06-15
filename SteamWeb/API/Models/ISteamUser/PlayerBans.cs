using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.ISteamUser
{
    public class PlayerBans
    {
        public List<PlayerBan> players { get; set; } = new();
        public bool success { get; set; } = false;
    }
}
