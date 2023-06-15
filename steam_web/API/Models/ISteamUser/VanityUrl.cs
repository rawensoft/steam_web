using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.ISteamUser
{
    public class VanityUrl
    {
        [JsonIgnore] public bool b_response
        {
            get
            {
                if (response > 1) return false;
                if (response == 0) return false;
                return true;
            }
        }
        public ushort response { get; set; } = 0;
        /// <summary>
        /// Есть значение если response > 1 && response == 0
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// Есть значение если b_response == true
        /// </summary>
        public string steamid { get; set; }
    }
}
