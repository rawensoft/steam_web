using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.IPlayerService
{
    public class ProfileCustomization
    {
        /// <summary>
        /// Доступно если (include_inactive_customizations || include_purchased_customizations)
        /// </summary>
        public Customization customizations { get; set; } = new();
        public ushort slots_available { get; set; }
        public ProfileTheme profile_theme { get; set; } = new();
        public ProfilePreferences profile_preferences { get; set; } = new();
    }
}
