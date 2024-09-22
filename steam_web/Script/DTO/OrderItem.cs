using SteamWeb.Extensions;

namespace SteamWeb.Script.DTO;
public class OrderItem
{
    public string Name { get; init; }
    public string Game { get; init; }
    public ushort Count { get; init; }
    public ulong Id { get; init; }
    public string Price { get; init; }
    public uint AppId { get; init; }
    public string MarketHashName { get; init; }

    /// <summary>
    /// Парсит строку цены, превращая её, в decimal число
    /// </summary>
    /// <returns>-1 в случае, если неудалось спарсить, либо цена в decimal формате</returns>
    public decimal GetDecimalPrice() => Price.ToDecimalPrice();
    /// <summary>
    /// Парсит строку цены, превращая её, в uint32 число
    /// </summary>
    /// <returns>0 в случае, если неудалось спарсить, либо цена в decimal формате</returns>
    public uint GetUInt32Price() => Price.ToUInt32Price();
}