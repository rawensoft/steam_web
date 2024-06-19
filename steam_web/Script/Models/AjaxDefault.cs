using SteamWeb.Extensions;
using System.Text.Json.Serialization;

namespace SteamWeb.Script.Models;
public class AjaxDefault
{
    public bool success { get; init; } = false;
    public string? hash { get; init; } = null;
    public string? errorMsg { get; init; } = null;

    [JsonIgnore] public bool IsErrorMsg => !errorMsg.IsEmpty();
    [JsonIgnore] public bool IsHash => !hash.IsEmpty();
}
