using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO;

public class InventoryHistory
{
    [JsonPropertyName("success")]
    public bool Success { get; internal set; } = false;

    [JsonPropertyName("cursor")]
    public InventoryHistoryCursor Cursor { get; init; } = new();

    /// <summary>
    /// Key1 - AppID, Key2 - classid_instanceid
    /// </summary>
#if NET8_0_OR_GREATER
    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
#endif
    [JsonPropertyName("descriptions")]
    public Dictionary<string, Dictionary<string, InventoryHistoryItem>> Descriptions { get; init; } = new(20);

    [JsonPropertyName("apps")]
    public InventoryHistoryApp[] Apps { get; init; } = Array.Empty<InventoryHistoryApp>();

    [JsonPropertyName("num")]
    public int Num { get; init; }

    [JsonPropertyName("html")]
    public string? Html { get; init; }

    public List<InventoryHistoryItem> GetDescriptionsArray()
    {
        var list = new List<InventoryHistoryItem>(Num + 1);
        foreach (var app in Descriptions)
            foreach (var item in app.Value)
                list.Add(item.Value);
        list.Sort();
        return list;
    }
}
