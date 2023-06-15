namespace SteamWeb.Models;
public sealed class MarketHistoryGraph
{
    public System.DateTime Time { get; init; }
    public double Price { get; init; }
    public int Count { get; init; }

    [System.Text.Json.Serialization.JsonConstructor]
    public MarketHistoryGraph(System.DateTime time, double price, int count)
    {
        Time = time;
        Price = price;
        Count = count;
    }
}
