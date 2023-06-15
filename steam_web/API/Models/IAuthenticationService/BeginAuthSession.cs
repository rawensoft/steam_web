using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.IAuthenticationService
{
    public class BeginAuthSession
    {
        public string client_id { get; set; }
        public string request_id { get; set; }
        public ushort interval { get; set; }
    }
}
