namespace SteamWeb.Models.Trade;
public sealed class Token
{
    public string trade_offer_access_token { get; init; }

    public Token() { }
    public Token(string trade_offer_access_token) => this.trade_offer_access_token = trade_offer_access_token;
}
