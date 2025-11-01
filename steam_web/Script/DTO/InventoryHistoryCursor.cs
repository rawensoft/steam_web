using SteamWeb.Extensions;

namespace SteamWeb.Script.DTO;

public class InventoryHistoryCursor
{
    [System.Text.Json.Serialization.JsonPropertyName("time")]
    public int CursorTime { get; init; } = DateTime.UtcNow.ToTimeStamp();

    [System.Text.Json.Serialization.JsonPropertyName("time_frac")]
    public int CursorTimeFrac { get; init; }

    [System.Text.Json.Serialization.JsonPropertyName("s")]
    public string CursorS { get; init; } = string.Empty;
}