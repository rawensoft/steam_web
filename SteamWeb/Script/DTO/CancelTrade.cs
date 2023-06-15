using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO;
public class CancelTrade
{
    public bool success { get; internal set; } = false;
    /// <summary>
    /// a unique identifier for the trade offer
    /// </summary>
    public string tradeofferid { get; init; }
    /// <summary>
    /// a unique identifier for the trade offer
    /// </summary>
    [JsonIgnore]
    public ulong u_tradeofferid
    {
        get
        {
            if (ulong.TryParse(tradeofferid, out var result)) return result;
            return 0;
        }
    }
}
