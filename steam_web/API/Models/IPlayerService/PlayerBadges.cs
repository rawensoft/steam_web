using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.IPlayerService
{
    public class PlayerBadges
    {
        public List<Badge> badges { get; set; } = new();
        public int player_xp { get; set; }
        public int player_level { get; set; }
        public int player_xp_needed_to_level_up { get; set; }
        public int player_xp_needed_current_level { get; set; }
    }
}
