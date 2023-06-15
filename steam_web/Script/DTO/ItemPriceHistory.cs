using System;

namespace SteamWeb.Script.DTO;

public readonly struct ItemPriceHistory
{
    public DateTime Time { get; init; }
    public int Count { get; init; }
    public float Price { get; init; }
}
