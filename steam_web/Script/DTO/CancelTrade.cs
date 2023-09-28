using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO;
public class CancelTrade
{
    public bool success { get; internal set; } = false;
    /// <summary>
    /// a unique identifier for the trade offer
    /// </summary>
    public ulong tradeofferid { get; init; }
}
