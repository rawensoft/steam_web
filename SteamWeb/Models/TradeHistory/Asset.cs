namespace SteamWeb.Models.TradeHistory;
public class Asset
{
    public ulong appid { get; init; }
    public string contextid { get; init; }
    public string assetid { get; init; }
    public string classid { get; init; }
    public string instanceid { get; init; }
    public string amount { get; init; }
    public string est_usd { get; init; }
    public bool missing { get; init; }
}
