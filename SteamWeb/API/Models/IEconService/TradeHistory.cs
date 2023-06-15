using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.IEconService
{
    public record TradeHistory
    {
        /// <summary>
        /// a unique identifier for the trade/ Пример этого 3749786070834469105.
        /// </summary>
        public string tradeid { get; set; }
        /// <summary>
        /// the SteamID64 of your trade partner
        /// </summary>
        public string steamid_other { get; set; }
        /// <summary>
        /// unix timestamp of the time the trade started to commit
        /// </summary>
        public int time_init { get; set; }
        /// <summary>
        /// unix timestamp of the time the trade will leave escrow
        /// </summary>
        public int time_escrow_end { get; set; }
        public Enums.ETradeStatus status { get; set; } = Enums.ETradeStatus.k_ETradeStatus_Failed;
        public List<TradeHistoryAssets> assets_given { get; set; } = new();
        public List<TradeHistoryAssets> assets_received { get; set; } = new();

        /// <summary>
        /// Трейд содержит предметы только для отправки
        /// </summary>
        [JsonIgnore] public bool only_given
        {
            get
            {
                if (assets_given.Count > 0 && assets_received.Count == 0) return true;
                return false;
            }
        }
        /// <summary>
        /// Трейд содержит предметы только для меня
        /// </summary>
        [JsonIgnore] public bool only_receive
        {
            get
            {
                if (assets_received.Count > 0 && assets_given.Count == 0) return true;
                return false;
            }
        }
        /// <summary>
        /// Это полноценный обмен. С обеих сторон имеются предметы.
        /// </summary>
        [JsonIgnore] public bool is_exchange
        {
            get
            {
                if (assets_given.Count > 0 && assets_received.Count > 0) 
                    return true;
                return false;
            }
        }
        /// <summary>
        /// SteamID64
        /// </summary>
        [JsonIgnore] public ulong u_steamid_other
        {
            get
            {
                if (ulong.TryParse(steamid_other, out var result)) return result;
                return 0;
            }
        }
        /// <summary>
        /// a unique identifier for the trade/ Пример этого 3749786070834469105.
        /// </summary>
        [JsonIgnore] public ulong u_tradeid
        {
            get
            {
                if (ulong.TryParse(tradeid, out var result)) return result;
                return 0;
            }
        }
    }
}
