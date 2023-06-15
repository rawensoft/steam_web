using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.IEconService
{
    public record TradeHistoryDescription
    {
        public string type { get; set; }
        public string value { get; set; }
        public string color { get; set; }
        public int appid { get; set; }
        public string classid { get; set; }
        public string instanceid { get; set; }
        public bool currency { get; set; } = false;
        public string background_color { get; set; }
        public string icon_url { get; set; }
        public string icon_url_large { get; set; }
        //public List<Description> descriptions { get; set; }
        public bool tradable { get; set; } = false;
        //public List<Action> actions { get; set; }
        public string name { get; set; }
        public string name_color { get; set; }
        public string market_name { get; set; }
        public string market_hash_name { get; set; }
        //public List<MarketAction> market_actions { get; set; }
        public bool commodity { get; set; } = false;
        public int market_tradable_restriction { get; set; }
        public bool marketable { get; set; } = false;

        [JsonIgnore] public ulong u_classid
        {
            get
            {
                if (ulong.TryParse(classid, out var result)) return result;
                return 0;
            }
        }
        [JsonIgnore] public uint u_instanceid
        {
            get
            {
                if (uint.TryParse(instanceid, out var result)) return result;
                return 0;
            }
        }
    }
}
