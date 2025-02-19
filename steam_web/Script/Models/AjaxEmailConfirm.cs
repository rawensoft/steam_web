using System.Text.Json.Serialization;

namespace SteamWeb.Script.Models;
public class AjaxEmailConfirm : AjaxDefault
{
    [JsonPropertyName("show_confirmation")]
    public bool ShowConfirmation { get; init; } = false;
}