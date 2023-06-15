using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.ISteamUser
{
    public class Players<T>
    {
        public List<T> players { get; set; } = new();
    }
}
