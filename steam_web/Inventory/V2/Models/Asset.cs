namespace SteamWeb.Inventory.V2.Models
{
    public sealed class Asset
    {
        public string id { get; init; }
        public string classid { get; init; }
        public string instanceid { get; init; }
        public string amount { get; init; }
        /// <summary>
        /// 1 - Не отображяется для китая
        /// </summary>
        public int hide_in_china { get; init; }
        /// <summary>
        /// Позиция в инвентаре
        /// </summary>
        public int pos { get; init; }
    }
}
