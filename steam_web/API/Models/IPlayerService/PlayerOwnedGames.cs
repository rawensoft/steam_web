using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.IPlayerService
{
    public class PlayerOwnedGames
    {
        public uint game_count { get; set; }
        public List<OwnedGame> games { get; set; } = new();
}
}
