namespace SteamWeb.Models;
public sealed class WalletInfo
{
    public string wallet_country { get; init; }
    public int wallet_currency { get; init; }
    public string wallet_state { get; init; }
    public int wallet_fee { get; init; }
    public int wallet_fee_minimum { get; init; }
    public double wallet_fee_percent { get; init; }
    public double wallet_publisher_fee_percent_default { get; init; }
    public double wallet_fee_base { get; init; }
    public int wallet_balance { get; init; }
    public int wallet_delayed_balance { get; init; }
    public int wallet_max_balance { get; init; }
    public int wallet_trade_max_balance { get; init; }
    public int success { get; init; }
    public int rwgrsn { get; init; }
}
