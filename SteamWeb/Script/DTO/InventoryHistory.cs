using System.Collections.Generic;

namespace SteamWeb.Script.DTO;

public class InventoryHistory
{
    public bool success { get; internal set; } = false;
    public InventoryHistoryCursor cursor { get; init; } = new();
    /// <summary>
    /// Key1 - AppID, Key2 - classid_instanceid
    /// </summary>
    public Dictionary<string, Dictionary<string, InventoryHistoryItem>> descriptions { get; init; } = new(0);
    public InventoryHistoryApp[] apps { get; init; } = new InventoryHistoryApp[0];
    public int num { get; init; }
    public string html { get; init; }

    public InventoryHistoryItem[] GetDescriptionsArray()
    {
        var list = new List<InventoryHistoryItem>(num);
        foreach (var app in descriptions)
        {
            foreach (var item in app.Value) list.Add(item.Value);
        }
        list.Sort();
        return list.ToArray();
    }
}
