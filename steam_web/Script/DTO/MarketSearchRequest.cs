using System.Collections.Generic;
using System.Text.Json.Serialization;
using SteamWeb.Script.Enums;

namespace SteamWeb.Script
{
    public class MarketSearchRequest
    {
        /// <summary>
        /// Название предмета
        /// </summary>
        [JsonPropertyName("query")] public string Query { get; set; } = "";
        /// <summary>
        /// Смещение
        /// </summary>
        [JsonPropertyName("start")] public uint Start { get; set; } = 0;
        /// <summary>
        /// Макс предметов (1 - 100)
        /// </summary>
        [JsonPropertyName("count")] public uint Count { get; set; } = 100;
        [JsonPropertyName("search_descriptions")] public uint Search_Descriptions { get; private set; } = 0;
        [JsonIgnore] public SORT_COLUMN Sort_Column { get; set; } = SORT_COLUMN.Price;
        [JsonIgnore] public SORT_DIRECTION Sort_Dir { get; set; } = SORT_DIRECTION.Asc;
        [JsonIgnore] public List<CATEGORY_730_TYPE> Category730Types { get; set; } = new (20);
        [JsonIgnore] public List<CATEGORY_730_WEAPON> Category730Weapons { get; set; } = new(50);
        [JsonPropertyName("appid")] public uint AppId { get; set; } = 0;
        /// <summary>
        /// Всегда 1
        /// </summary>
        [JsonPropertyName("norender")] public uint NoRender { get; private set; } = 1;
    }
}
