using SteamWeb.Script.Enums;

namespace SteamWeb.Script.DTO.Historing;

public record HistoryItem
{
    public TYPE_HISTORY_ITEM Type { get; internal set; }
    public string Name { get; internal set; }
    public string Your_Price { get; internal set; }
    public string Buyer_Username { get; internal set; }
    public string Buyer_URL { get; internal set; }
    public string ListedOn { get; internal set; }
    public string ActedOn { get; internal set; }
    public string Game { get; internal set; }
    public string RemoveID0 { get; internal set; }
    public string RemoveID1 { get; internal set; }
    public double GetYour_Price
    {
        get
        {
            if (string.IsNullOrEmpty(Your_Price)) return 0;
            string splitted = Your_Price.Split(' ')[0].Replace(".", ",");
            if (double.TryParse(splitted, out var result)) return result;
            return 0;
        }
    }

    public bool Compare(string RemoveID)
    {
        if (RemoveID0 == RemoveID || RemoveID1 == RemoveID) return true;
        return false;
    }
}
