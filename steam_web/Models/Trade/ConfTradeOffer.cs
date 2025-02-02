using System.Text.Json.Serialization;

namespace SteamWeb.Models.Trade;
public class ConfTradeOffer
{
    /// <summary>
    /// Домен почты, где находиться письмо для подтверждения трейда
    /// </summary>
    [JsonPropertyName("email_domain")] public string? EmailDomain { get; init; }
    /// <summary>
    /// Нужно ли подтвердить по почте
    /// </summary>
    [JsonPropertyName("needs_email_confirmation")] public bool NeedsEmailConfirmation { get; init; } = false;
    /// <summary>
    /// Нужно ли подтвердить на телефоне
    /// </summary>
    [JsonPropertyName("needs_mobile_confirmation")] public bool NeedsMobileConfirmation { get; init; } = false;
    /// <summary>
    /// tradeofferid созданного трейда
    /// </summary>
    [JsonPropertyName("tradeofferid")] public ulong? TradeOfferId { get; init; }
}