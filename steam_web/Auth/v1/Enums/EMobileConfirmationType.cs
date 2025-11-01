namespace SteamWeb.Auth.v1.Enums;

public enum EMobileConfirmationType : byte
{
    Invalid = 0,
    Unknown = 1,
    TradeOffer = 2,
    MarketSellTransaction = 3,
    FeatureOptOut = 4,
    PhoneNumberChange = 5,
    AccountRecovery = 6,
    ApiKeyCreation = 9,
    JoinSteamFamily = 11,
}