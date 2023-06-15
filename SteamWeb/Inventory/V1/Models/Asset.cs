namespace SteamWeb.Inventory.V1.Models;
public sealed class Asset
{
    public string amount { get; init; }
    public ulong appid { get; init; }
    public string assetid { get; init; }
    public string classid { get; init; }
    public string contextid { get; init; }
    public string instanceid { get; init; }
}