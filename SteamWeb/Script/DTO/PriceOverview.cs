using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO
{
    public class PriceOverview
    {
        [JsonPropertyName("success")] public bool Success { get; init; } = false;
        [JsonPropertyName("lowest_price")] public string Lowest_Price { get; init; } = null;
        [JsonPropertyName("median_price")] public string Median_Price { get; init; } = null;
        [JsonPropertyName("volume")] public string Volume { get; init; } = null;

        private float GetPrice(string price)
        {
            var splitted = price.Split(' ');
            if (splitted.Length != 2) return 0;
            if (!float.TryParse(splitted[0], out var result)) return 0;
            return result;
        }
        public float GetLowestPrice() => GetPrice(Lowest_Price);
        public float GetMedianPrice() => GetPrice(Median_Price);
        public float GetVolume() => uint.TryParse(Volume, out var result) ? result : 0;
    }
}
