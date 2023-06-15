namespace SteamWeb.Inventory.V1.Models;
public sealed class Description
{
    public Action[] actions { get; init; } = new Action[0];
    public uint appid { get; init; }
    public string background_color { get; init; }
    public string classid { get; init; }
    public ulong commodity { get; init; }
    public ulong currency { get; init; }
    public DescriptionItem[] descriptions { get; init; } = new DescriptionItem[0];
    public string icon_url { get; init; }
    public string icon_url_large { get; init; }
    public string instanceid { get; init; }
    public Action[] market_actions { get; init; } = new Action[0];
    public string market_hash_name { get; init; }
    public string market_name { get; init; }
    public uint market_tradable_restriction { get; init; }
    public uint marketable { get; init; }
    public string name { get; init; }
    public string name_color { get; init; }
    public Tag[] tags { get; init; } = new Tag[0];
    public uint tradable { get; init; }
    public string type { get; init; }
}