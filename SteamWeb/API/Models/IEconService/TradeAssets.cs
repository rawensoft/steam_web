using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IEconService
{
    public class TradeAssets
    {
        public int appid { get; init; }
        public string contextid { get; init; }
        /// <summary>
        /// either assetid or currencyid will be set
        /// </summary>
        public string assetid { get; init; }
        /// <summary>
        /// either assetid or currencyid will be set
        /// </summary>
        public string currencyid { get; init; }
        /// <summary>
        /// together with instanceid, uniquely identifies the display of the item
        /// </summary>
        public string classid { get; init; }
        /// <summary>
        /// together with classid, uniquely identifies the display of the item
        /// </summary>
        public string instanceid { get; init; }
        /// <summary>
        /// the amount offered in the trade, for stackable items and currency
        /// </summary>
        public string amount { get; init; }
        /// <summary>
        /// a boolean that indicates the item is no longer present in the user's inventory
        /// </summary>
        public bool missing { get; init; } = false;
        /// <summary>
        /// a string that represent Steam's determination of the item's value, in whole USD pennies. How this is determined is unknown.
        /// </summary>
        public string est_usd { get; init; }

        [JsonIgnore]
        public ushort u_context
        {
            get
            {
                if (ushort.TryParse(contextid, out var result)) return result;
                return 2;
            }
        }
        /// <summary>
        /// either assetid or currencyid will be set
        /// </summary>
        [JsonIgnore]
        public ulong u_assetid
        {
            get
            {
                if (ulong.TryParse(assetid, out var result)) return result;
                return 0;
            }
        }
        /// <summary>
        /// either assetid or currencyid will be set
        /// </summary>
        [JsonIgnore]
        public ulong u_currencyid
        {
            get
            {
                if (ulong.TryParse(currencyid, out var result)) return result;
                return 0;
            }
        }
        /// <summary>
        /// the amount offered in the trade, for stackable items and currency
        /// </summary>
        [JsonIgnore]
        public uint u_amount
        {
            get
            {
                if (uint.TryParse(amount, out var result)) return result;
                return 0;
            }
        }
        /// <summary>
        /// together with instanceid, uniquely identifies the display of the item
        /// </summary>
        [JsonIgnore]
        public ulong u_classid
        {
            get
            {
                if (ulong.TryParse(classid, out var result)) return result;
                return 0;
            }
        }
        /// <summary>
        /// together with classid, uniquely identifies the display of the item
        /// </summary>
        [JsonIgnore]
        public uint u_instanceid
        {
            get
            {
                if (uint.TryParse(instanceid, out var result)) return result;
                return 0;
            }
        }
        /// <summary>
        /// a string that represent Steam's determination of the item's value, in whole USD pennies. How this is determined is unknown.
        /// </summary>
        [JsonIgnore]
        public uint u_est_usd
        {
            get
            {
                if (uint.TryParse(est_usd, out var result)) return result;
                return 0;
            }
        }
    }
}
