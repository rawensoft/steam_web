using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;
using SteamWeb.Extensions;

namespace SteamWeb.Script.DTO;
public class PriceHistory
{
    public string? price_prefix { get; init; }
    public string? price_suffix { get; init; }
    public bool success { get; init; } = false;
    public bool e500 { get; init; } = false;
    [JsonPropertyName("prices")] public JsonElement[][] prices { get; set; } = Array.Empty<JsonElement[]>();
    [JsonIgnore] public ItemPriceHistory[] history { get; internal set; } = Array.Empty<ItemPriceHistory>();

    public static PriceHistory Deserialize(string json)
    {
        try
        {
            var obj = JsonSerializer.Deserialize<PriceHistory>(json)!;
            obj.history = SortPriceHistory(obj.prices);
            obj.prices = Array.Empty<JsonElement[]>();
            return obj;
        }
        catch (Exception)
        {
			return new();
		}
    }
    internal static ItemPriceHistory[] SortPriceHistory(JsonElement[][] prices)
    {
        const string format = "MMM d yyyy HH: z";
        var list = new List<ItemPriceHistory>(prices.Length);
        DateTime time;
        decimal price;
        uint count;
        string raw;
        var cultureCode = CultureInfo.GetCultureInfo("en-US");
        foreach (var item0 in prices)
        {
            raw = item0[0].GetRawText().Replace("\"", string.Empty);
            time = DateTime.ParseExact(raw, format, cultureCode);
            price = item0[1].GetDecimal();
            count = item0[2].GetString().ParseUInt32();
            list.Add(new()
            {
                Count = count,
                Time = time,
                Price = price,
            });
        }
        list.Reverse();
        return list.ToArray();
    }
}