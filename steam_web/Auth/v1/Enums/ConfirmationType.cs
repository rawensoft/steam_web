namespace SteamWeb.Auth.v1.Enums;

public enum ConfirmationType : byte
{
    Unknown,
    TradeOffer = 2,
    MarketSellTransaction = 3,
    ChangeNumber = 5,
    AccountRecovery = 6,
    RegisterApiKey = 9,
}
