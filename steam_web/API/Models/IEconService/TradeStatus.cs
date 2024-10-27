using System.Text.Json.Serialization;
using SteamWeb.Extensions;

namespace SteamWeb.API.Models.IEconService;
public class TradeStatus
{
	[JsonPropertyName("trades")] public TradeHistory[] Trades { get; init; } = Array.Empty<TradeHistory>();
	/// <summary>
	/// Доступно если get_descriptions == true
	/// </summary>
	[JsonPropertyName("descriptions")] public List<TradeHistoryDescription> Descriptions { get; init; } = new(1);

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
