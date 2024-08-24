using System.Globalization;
using System.Text.RegularExpressions;
using SteamWeb.Extensions;

namespace SteamWeb.Models;
public class SteamOffer
{
    private const string AwaitingMobileConfirmation = "Awaiting Mobile Confirmation";
    private const string TradeAccepted = "Trade Accepted";
    private const string TradeOfferCanceled = "Trade Offer Canceled";
    private static Regex RgxBanner { get; } = new(@"^((?:(?:\w{2,})|(?:\s{1}))+)(\d{1,2}\s\w{3}[,]\s\d{4}){0,1}(?:\s[@]\s((\d{1,2}[:]\d{1,2})(\w{2}))){0,1}$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(200));

    public ulong TradeOfferId { get; init; }
    public uint PartnerId { get; init; }
    public bool PartnerOnline { get; init; }
    public bool ActiveOffer { get; init; }
    public bool AcceptedOffer { get; init; }
    /// <summary>
    /// Информация о предложении обмена, если <see cref="ActiveOffer"/>==false:
    /// <para/>
    /// Awaiting Mobile Confirmation, 
    /// Trade Offer Canceled
    /// <para/>
    /// Либо информация о дате принятия трейда, если <see cref="AcceptedOffer"/>==true:
    /// <para/>
    /// Trade Accepted
    /// </summary>
    public string? Status { get; init; }
    /// <summary>
    /// Время статуса, если оно есть
    /// <para/>
    /// Время не всегда может содержать часы и минуты, например такое возможно при отмене трейда
    /// </summary>
    public DateTime? StatusTime { get; init; }
    public SteamOfferItem[] PartnerItems { get; init; }
    public SteamOfferItem[] OurItems { get; init; }

    public bool IsStatusAwaitingMobileConfirmation => Status == AwaitingMobileConfirmation;
    public bool IsStatusAccepted => Status == TradeAccepted;
    public bool IsStatusCanceled => Status == TradeOfferCanceled;
    public bool IsOtherError => Status != null && !IsStatusAwaitingMobileConfirmation && !IsStatusAccepted && !IsStatusCanceled;

    internal static (string?, DateTime?) ParseBanner(string? banner)
    {
        if (banner.IsEmpty())
            return (null, null);
        try
        {
            var match = RgxBanner.Match(banner!);
            if (match.Success)
            {
                var status = match.Groups[1].Value;
                if (char.IsWhiteSpace(status[^1]))
                    status = status.Remove(status.Length - 1, 1);

                var date = match.Groups[2].Value;
                var time = match.Groups[3].Value;

                DateTime? dt = null;
                var isDate = !date.IsEmpty();
                var isTime = !time.IsEmpty();
                if (isDate && isTime)
                {
                    var culture = CultureInfo.GetCultureInfo("en-US");
                    dt = DateTime.ParseExact(date + " @ " + time, "dd MMM, yyyy @ hh:mmtt", culture);
                }
                else if (isDate)
                {
                    var culture = CultureInfo.GetCultureInfo("en-US");
                    dt = DateTime.ParseExact(date, "dd MMM, yyyy", culture);
                }
                return (status, dt);
            }
            return (null, null);
        }
        catch (RegexMatchTimeoutException)
        {
            return (null, null);
        }
    }
}