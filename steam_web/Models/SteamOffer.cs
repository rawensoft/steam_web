namespace SteamWeb.Models;
public class SteamOffer
{
    public ulong TradeOfferId { get; init; }
    public uint PartnerId { get; init; }
    public bool PartnerOnline { get; init; }
    public SteamOfferItem[] PartnerItems { get; init; }
    public SteamOfferItem[] OurItems { get; init; }
}