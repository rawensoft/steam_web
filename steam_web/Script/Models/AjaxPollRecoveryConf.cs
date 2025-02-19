using System.Text.Json.Serialization;

namespace SteamWeb.Script.Models;
public class AjaxPollRecoveryConf: AjaxDefault
{
    [JsonPropertyName("continue")]
    public bool Continue { get; init; } = false;
}