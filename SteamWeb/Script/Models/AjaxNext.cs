using SteamWeb.Extensions;
using System.Text.Json.Serialization;

namespace SteamWeb.Script.Models
{
    public class AjaxNext
    {
        public string hash { get; set; } = null;
        public string errorMsg { get; set; } = null;

        [JsonIgnore] public bool IsHash => !hash.IsEmpty();
        [JsonIgnore] public bool IsError => !errorMsg.IsEmpty();
    }
}
