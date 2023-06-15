using System.Text.Json.Serialization;

namespace SteamWeb.Models.TradeHistory;
public partial class ItemTrade
{
    [JsonIgnore] public Enums.ETradeOfferState e_trade_offer_state => (Enums.ETradeOfferState)trade_offer_state;
    public string tradeofferid { get; init; }
    public long accountid_other { get; init; }
    public string message { get; init; }
    public ulong expiration_time { get; init; }
    public int trade_offer_state { get; init; }
    public bool is_our_offer { get; init; }
    public ulong time_created { get; init; }
    public ulong time_updated { get; init; }
    public string tradeid { get; init; }
    public bool from_real_time_trade { get; init; }
    public int escrow_end_date { get; init; }
    public int confirmation_method { get; init; }
    public Asset[] items_to_receive { get; init; } = new Asset[0];
    public Asset[] items_to_give { get; init; } = new Asset[0];
}
