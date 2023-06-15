using SteamWeb.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SteamWeb.Script.Models
{
    public class AjaxDefault
    {
        public bool success { get; set; } = false;
        public string hash { get; set; } = null;
        public string errorMsg { get; set; } = null;
        [JsonIgnore] public bool IsErrorMsg => !errorMsg.IsEmpty();
        [JsonIgnore] public bool IsHash => !hash.IsEmpty();
    }
}
