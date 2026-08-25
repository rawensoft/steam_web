using System.Text;
using System.Text.Json.Serialization;
using SteamWeb.Extensions;

namespace SteamWeb.Script.DTO;
public class PriceOverview
{
    [JsonPropertyName("success")] public bool Success { get; init; } = false;
    [JsonPropertyName("lowest_price")] public string? LowestPrice { get; init; } = null;
    [JsonPropertyName("median_price")] public string? MedianPrice { get; init; } = null;
    [JsonPropertyName("volume")] public ulong? Volume { get; init; } = null;

    internal static decimal GetPrice(string? price)
    {
        if (price.IsEmpty())
			return 0m;

        byte flag = 0;
        var sb = new StringBuilder();
        var index = 0;
        var strLength = price.Length;
        byte first = 0;
        byte flags = 0;
        int firstDot = 0;
        foreach (var ch in price)
        {
            if (char.IsDigit(ch))
                sb.Append(ch);
            else if (ch == 44 || ch == 46 && (sb.Length != 0 && index + 1 != strLength))
            {
                if (ch == 46)
                {
                    flags++;
                    flag += 1;
                    sb.Append('.');
                    if (first == 0)
                        first = 46;
                    if (firstDot == 0)
                        firstDot = index;
                }
                else
                {
                    flags++;
                    flag += 2;
                    sb.Append(',');
                    if (first == 0)
                        first = 44;
                }
            }
            index++;
        }
        if (flag >= 3)
        {
            if (first == 46)
            {
                sb.Replace(".", "");
                sb.Replace(",", ".");
            }
            else
                sb.Replace(",", "");
        }
        else if (flag == 2)
        {
            sb.Replace(",", ".");
            if (flags > 1 && first == 46)
                sb.Remove(firstDot, 1);
        }
        var str = sb.ToString();
        if (!decimal.TryParse(str, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out var result))
            return 0;
        return result;
    }
    public decimal GetLowestPrice() => GetPrice(LowestPrice);
    public decimal GetMedianPrice() => GetPrice(MedianPrice);
}