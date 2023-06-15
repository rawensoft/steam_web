using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.Script.Models
{
    public class SteamRSA
    {
        public bool success { get; set; } = false;
        public string publickey_mod { get; set; } = null;
        public string publickey_exp { get; set; } = null;
        public string timestamp { get; set; } = null;
        public string token_gid { get; set; } = null;
    }
}
