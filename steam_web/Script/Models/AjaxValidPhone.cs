using System.Text.Json.Serialization;

namespace SteamWeb.Script.Models;
public class AjaxValidPhone
{
    [JsonPropertyName("success")]
    public bool Success { get; set; } = false;

    [JsonPropertyName("number")]
    public string? Number { get; set; } = null;

    [JsonPropertyName("is_valid")]
    public bool IsValid { get; set; } = false;

    [JsonPropertyName("is_voip")]
    public bool IsVoIp { get; set; } = false;

    [JsonPropertyName("is_fixed")]
    public bool IsFixed { get; set; } = false;
}
