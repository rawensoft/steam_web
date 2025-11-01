namespace SteamWeb.Script.DTO;
public class ItemPriceHistory
{
    public DateTime Time { get; init; }
    public uint Count { get; init; }
    public decimal Price { get; init; }
}