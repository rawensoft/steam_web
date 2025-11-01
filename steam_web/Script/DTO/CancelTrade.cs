using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO;
public class CancelTrade
{
    [JsonIgnore]
    public bool Success => TradeOfferId != 0;

    /// <summary>
    /// a unique identifier for the trade offer
    /// </summary>
    [JsonPropertyName("tradeofferid")]
    public ulong TradeOfferId { get; init; }
}