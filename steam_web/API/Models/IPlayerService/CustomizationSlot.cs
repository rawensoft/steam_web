using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.IPlayerService
{
    public record CustomizationSlot
    {
        public int slot { get; set; }
        public int appid { get; set; }
    }
}
