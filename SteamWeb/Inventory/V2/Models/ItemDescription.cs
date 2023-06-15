namespace SteamWeb.Inventory.V2.Models
{
    public sealed class ItemDescription
    {
        public string type { get; init; }
        public string value { get; init; }
        public string color { get; init; }
        public ItemAppData app_data { get; init; }
    }
}
