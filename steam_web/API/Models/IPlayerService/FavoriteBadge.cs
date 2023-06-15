using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.IPlayerService
{
    public class FavoriteBadge
    {
        public bool has_favorite_badge { get; set; } = false;
        /// <summary>
        /// Есть значение, если has_favorite_badge == true
        /// </summary>
        public int? badgeid { get; set; } = null;
        /// <summary>
        /// Есть значение, если has_favorite_badge == true
        /// </summary>
        public int? level { get; set; } = null;
    }
}
