using SteamWeb.Inventory.V2.Models;

namespace SteamWeb.Script.DTO;

public class InventoryHistoryItem
{
    public int commodity { get; init; }
    public string background_color { get; init; }
    public string classid { get; init; }
    public string icon_drag_url { get; init; }
    public string icon_url { get; init; }
    public string icon_url_large { get; init; }
    public string instanceid { get; init; }
    public string market_fee_app { get; init; }
    public string market_hash_name { get; init; }
    public string market_marketable_restriction { get; init; }
    public string market_name { get; init; }
    public string market_tradable_restriction { get; init; }
    public int marketable { get; init; }
    public string name { get; init; }
    public string name_color { get; init; }
    public int tradable { get; init; }
    public string type { get; init; }
    public ItemTag[] tags { get; init; } = new ItemTag[0];
}
