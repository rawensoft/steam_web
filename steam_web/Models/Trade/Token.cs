using System.Text.Json.Serialization;

namespace SteamWeb.Models.Trade;
public class Token
{
    [JsonPropertyName("trade_offer_access_token")] public string? TradeOfferAccessToken { get; init; }

    [JsonConstructor]
    public Token() { }
    public Token(string trade_offer_access_token) => TradeOfferAccessToken = trade_offer_access_token;
}