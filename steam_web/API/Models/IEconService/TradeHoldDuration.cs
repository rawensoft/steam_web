namespace SteamWeb.API.Models.IEconService;

public class TradeHoldDuration
{
    public TradeHold my_escrow { get; init; } = new();
    public TradeHold their_escrow { get; init; } = new();
    public TradeHold both_escrow { get; init; } = new();
}
