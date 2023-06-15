using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.IPlayerService
{
    public record OwnedGame
    {
        /// <summary>
        /// Есть значение, если include_appinfo == true
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Есть значение, если include_appinfo == true
        /// </summary>
        public string img_icon_url { get; set; }
        /// <summary>
        /// Есть значение, если include_appinfo == true
        /// </summary>
        public string img_logo_url { get; set; }
        /// <summary>
        /// Есть значение, если include_appinfo == true
        /// </summary>
        public bool? has_community_visible_stats { get; set; }
        public int appid { get; set; }
        public int playtime_forever { get; set; }
        public int playtime_windows_forever { get; set; }
        public int playtime_mac_forever { get; set; }
        public int playtime_linux_forever { get; set; }
    }
}
