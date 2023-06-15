using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.ISteamUser
{
    public class FriendsList<T>
    {
        public T[] friends { get; set; } = new T[0];
    }
}
