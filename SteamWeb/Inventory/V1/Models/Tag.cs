namespace SteamWeb.Inventory.V1.Models;
public sealed class Tag
{
    public string category { get; init; }
    public string internal_name { get; init; }
    public string localized_category_name { get; init; }
    public string localized_tag_name { get; init; }
}