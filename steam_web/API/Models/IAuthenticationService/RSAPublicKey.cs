using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.IAuthenticationService
{
    public class RSAPublicKey
    {
        public string publickey_mod { get; set; }
        public string publickey_exp { get; set; }
        public string timestamp { get; set; }
    }
}
