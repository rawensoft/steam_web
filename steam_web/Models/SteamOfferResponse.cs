using SteamWeb.Extensions;

namespace SteamWeb.Models;

public class SteamOfferResponse
{
    public bool Success { get; init; } = false;
    public string? Error { get; init; }
    public bool IsError => Error.IsEmpty() || !Success;
    public SteamOffer[] Trades { get; init; } = Array.Empty<SteamOffer>();
}
