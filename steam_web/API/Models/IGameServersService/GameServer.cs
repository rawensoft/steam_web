using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.IGameServersService
{
    public record GameServer
    {
        public string steamid { get; set; }
        public int appid { get; set; }
        public string login_token { get; set; }
        public string memo { get; set; }
        public bool is_deleted { get; set; }
        public bool is_expired { get; set; }
        public int rt_last_logon { get; set; }
    }
}
