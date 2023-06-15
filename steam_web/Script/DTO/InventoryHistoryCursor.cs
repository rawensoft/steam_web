using SteamWeb.Extensions;
using System;

namespace SteamWeb.Script.DTO;

public class InventoryHistoryCursor
{
    [System.Text.Json.Serialization.JsonPropertyName("time")] public int CursorTime { get; init; } = DateTime.UtcNow.ToTimeStamp();
    [System.Text.Json.Serialization.JsonPropertyName("time_frac")] public int CursorTimeFrac { get; init; } = 0;
    [System.Text.Json.Serialization.JsonPropertyName("s")] public string CursorS { get; init; } = "";
}
