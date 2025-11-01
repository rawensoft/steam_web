using System.Text.Json.Serialization;

namespace SteamWeb.Script.Models;
public class PhoneAjax
{
    [JsonPropertyName("has_phone")]
    public bool? HasPhone { get; init; } = null;
}