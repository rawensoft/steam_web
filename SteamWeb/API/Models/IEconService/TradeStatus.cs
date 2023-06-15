using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.IEconService
{
    public class TradeStatus
    {
        public List<TradeHistory> trades { get; set; } = new();
        /// <summary>
        /// Доступно если get_descriptions == true
        /// </summary>
        public List<TradeHistoryDescription> descriptions { get; set; } = new();

        /// <summary>
        /// Доступно если get_descriptions == true
        /// </summary>
        /// <param name="classid"></param>
        /// <param name="instanceid"></param>
        /// <returns>Null если описание не найдено</returns>
        public TradeHistoryDescription GetDescriptionForItem(string classid, string instanceid)
        {
            foreach (var item in descriptions)
            {
                if (item.classid == classid && item.instanceid == instanceid) return item;
            }
            return null;
        }
        /// <summary>
        /// Доступно если get_descriptions == true
        /// </summary>
        /// <param name="classid"></param>
        /// <param name="instanceid"></param>
        /// <returns>Null если описание не найдено</returns>
        public TradeHistoryDescription GetDescriptionForItem(ulong classid, uint instanceid)
        {
            var @class = classid.ToString();
            var instance = instanceid.ToString();
            return GetDescriptionForItem(@class, instance);
        }
    }
}
