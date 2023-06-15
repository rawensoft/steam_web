using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.IPlayerService
{
    public record PlayerSteamLevel
    {
        public uint player_level { get; set; }
    }
}
