using System.Text.Json.Serialization;
using SteamWeb.Extensions;

namespace SteamWeb.Script.DTO;
public class PriceOverview
{
    [JsonPropertyName("success")] public bool Success { get; init; } = false;
    [JsonPropertyName("lowest_price")] public string? LowestPrice { get; init; } = null;
    [JsonPropertyName("median_price")] public string? MedianPrice { get; init; } = null;
    [JsonPropertyName("volume")] public ulong? Volume { get; init; } = null;

    private static float GetPrice(string? price)
    {
        if (price.IsEmpty())
			return 0f;
		var splitted = price!.Split(' ');
        if (splitted.Length != 2)
            return 0;
        if (!float.TryParse(splitted[0], out var result))
            return 0;
        return result;
    }
    public float GetLowestPrice() => GetPrice(LowestPrice);
    public float GetMedianPrice() => GetPrice(MedianPrice);
}