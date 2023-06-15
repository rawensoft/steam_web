using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.ISteamUserOAuth
{
    public class ResponseGroups
    {
        public bool success { get; set; } = false;
        public PlayerGroup[] groups { get; set; } = new PlayerGroup[0];
    }
}
