using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.IGameServersService
{
    public record AccountList
    {
        public List<GameServer> servers { get; set; } = new();
        public bool is_banned { get; set; }
        public int expires { get; set; }
        /// <summary>
        /// steamid64 аккаунта, которому принадлежат эти сервера
        /// </summary>
        public string actor { get; set; } = "0";
        public int last_action_time { get; set; }
    }
}
