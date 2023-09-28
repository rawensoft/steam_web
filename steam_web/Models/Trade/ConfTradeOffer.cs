namespace SteamWeb.Models.Trade;
public sealed class ConfTradeOffer
{
    /// <summary>
    /// Домен почты, где находиться письмо для подтверждения трейда
    /// </summary>
    public string? email_domain { get; init; }
    /// <summary>
    /// Нужно ли подтвердить по почте
    /// </summary>
    public bool needs_email_confirmation { get; init; }
    /// <summary>
    /// Нужно ли подтвердить на телефоне
    /// </summary>
    public bool needs_mobile_confirmation { get; init; }
    /// <summary>
    /// tradeofferid созданного трейда
    /// </summary>
    public ulong tradeofferid { get; init; }
}
