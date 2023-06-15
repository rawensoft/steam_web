namespace SteamWeb.Models.TradeHistory;
public sealed class TradeHistory
{
    public bool IsError { get; internal set; } = false;
    public bool IsEmpty { get; internal set; } = false;
    public History response { get; internal set; } = new();

    public ItemTrade GetTradeByID(string tradeid)
    {
        for (int i = 0; i < response.trade_offers_received.Length; i++)
        {
            var item = response.trade_offers_received[i];
            if (item.tradeofferid == tradeid)
                return item;
        }
        return null;
    }
}
