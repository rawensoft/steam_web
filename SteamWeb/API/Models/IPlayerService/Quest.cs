using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.IPlayerService
{
    public record Quest
    {
        public ushort questid { get; set; }
        public bool completed { get; set; } = false;
    }
}
