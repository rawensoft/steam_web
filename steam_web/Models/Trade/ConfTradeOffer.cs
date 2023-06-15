using System.Text.Json.Serialization;
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
    public string? tradeofferid { get; init; }

    public ConfTradeOffer() { }
    [JsonConstructor]
    public ConfTradeOffer(string email_domain, bool needs_email_confirmation, bool needs_mobile_confirmation, string tradeofferid)
    {
        this.email_domain = email_domain;
        this.needs_email_confirmation = needs_email_confirmation;
        this.needs_mobile_confirmation = needs_mobile_confirmation;
        this.tradeofferid = tradeofferid;
    }

    public override string ToString() => $"ConfTradeOffer({tradeofferid}, mob {needs_mobile_confirmation}, email {needs_email_confirmation}, {email_domain})";
}
