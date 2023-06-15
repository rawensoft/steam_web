using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.IPlayerService
{
    public record Badge
    {
        public int badgeid { get; set; }
        public int level { get; set; }
        public int completion_time { get; set; }
        public int xp { get; set; }
        public int scarcity { get; set; }
        public int? appid { get; set; }
        public string communityitemid { get; set; }
        public int? border_color { get; set; }
    }
}
