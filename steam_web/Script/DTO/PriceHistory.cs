using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using System.Globalization;

namespace SteamWeb.Script.DTO;

public sealed record PriceHistory
{
    public string price_prefix { get; init; }
    public string price_suffix { get; init; }
    public bool success { get; init; } = false;
    public bool e500 { get; init; } = false;
    [JsonPropertyName("prices")] public JsonElement[][] prices { get; set; } = new JsonElement[0][];
    [JsonIgnore] public ItemPriceHistory[] history { get; internal set; } = new ItemPriceHistory[0];

    public static PriceHistory Deserialize(string json)
    {
        try
        {
            var obj = JsonSerializer.Deserialize<PriceHistory>(json);
            obj.history = SortPriceHistory(obj.prices);
            obj.prices = new JsonElement[0][];
            return obj;
        }
        catch (Exception ex)
        { }
        return new();
    }
    internal static ItemPriceHistory[] SortPriceHistory(JsonElement[][] prices)
    {
        var list = new List<ItemPriceHistory>(prices.Length);
        DateTime time;
        float price;
        int count;
        string raw;
        var cultureCode = CultureInfo.GetCultureInfo("en-US");
        string format = "MMM d yyyy HH: z";
        foreach (var item0 in prices)
        {
            raw = item0[0].GetRawText().Replace("\"", "");
            time = DateTime.ParseExact(raw, format, cultureCode);
            price = (float)item0[1].GetDouble();
            count = int.Parse(item0[2].GetString());
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
