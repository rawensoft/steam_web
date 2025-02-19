using System.Text.Json.Serialization;

namespace SteamWeb.Script.Models;
public class AjaxNextStep
{
    [JsonPropertyName("redirect")]
    public string? Redirect { get; init; }
}