using System.Text.Json.Serialization;

namespace SteamWeb.Script.Models;
public class AjaxPasswordAvailable
{
    [JsonPropertyName("available")]
    public bool Available { get; init; } = false;
}