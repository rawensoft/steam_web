using System;
using System.Text.Json.Serialization;

namespace SteamWeb.Inventory.V2.Models
{
    public sealed class Description
    {
        public string appid { get; init; }
        public string classid { get; init; }
        public string instanceid { get; init; }
        public string icon_url { get; init; }
        public string icon_url_large { get; init; }
        public string icon_drag_url { get; init; }
        public string name { get; init; }
        public string market_hash_name { get; init; }
        public string market_name { get; init; }
        public string name_color { get; init; }
        public string background_color { get; init; }
        public string type { get; init; }
        public int tradable { get; init; }
        public int marketable { get; init; }
        public int commodity { get; init; }
        public string market_tradable_restriction { get; init; }
        public string market_marketable_restriction { get; init; }
        /// <summary>
        /// Время истечения ограничений
        /// </summary>
        public string cache_expiration { get; init; }
        [JsonIgnore]
        public DateTime get_tradable_expiration
        {
            get
            {
                if (string.IsNullOrEmpty(cache_expiration)) return DateTime.Now;
                return DateTime.Parse(cache_expiration);
            }
        }
        [JsonIgnore]
        public bool is_tradable
        {
            get
            {
                //if (string.IsNullOrEmpty(cache_expiration) && tradable > 0) return true;
                if (tradable > 0) return true;
                return false;
            }
        }
        [JsonIgnore]
        public bool is_marketable
        {
            get
            {
                if (marketable > 0) return true;
                return false;
            }
        }
        public ItemDescription[] descriptions { get; init; } = new ItemDescription[] { };
        public ItemDescription[] owner_descriptions { get; init; } = new ItemDescription[] { };
        public ItemAction[] actions { get; init; } = new ItemAction[] { };
        public ItemAction[] market_actions { get; init; } = new ItemAction[] { };
        public ItemTag[] tags { get; init; } = new ItemTag[] { };
        /// <summary>
        /// Только TF2/440
        /// </summary>
        public ItemAppData app_data { get; init; }

        public ItemTag GetTagByCategory(string category)
        {
            category = category.ToLower();
            for (int i = 0; i < tags.Length; i++)
            {
                var tag = tags[i];
                if (tag.category.ToLower() == category) return tag;
            }
            return null;
        }
    }
}
