using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.ISteamUserOAuth
{
    public record TokenDetail
    {
        public int success { get; set; } = 0;
        public string reason { get; set; } = "error";
        public string steamid { get; set; } = "0";
        public string client_id { get; set; }
        public int expiration { get; set; }
    }
}
