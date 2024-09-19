using System.Text.Json.Serialization;
using SteamWeb.Models.PurchaseHistory;

namespace SteamWeb.Script.Models;
public class MoreHistoryModel
{
    [JsonPropertyName("success")] public int Success { get; init; } = 1;
    [JsonPropertyName("html")] public string? Html { get; init; }
    [JsonPropertyName("cursor")] public PurchaseHistoryCursorModel? Cursor { get; init; }
}