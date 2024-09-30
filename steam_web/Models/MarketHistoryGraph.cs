namespace SteamWeb.Models;
public class MarketHistoryGraph
{
    public DateTime Time { get; init; }
    public decimal Price { get; init; }
    public int Count { get; init; }
}