using System.Text.Json.Serialization;
using SteamWeb.Extensions;

namespace SteamWeb.Script.Models;

public record NextRedirect
{
    public string hash { get; init; }
    public string errorMsg { get; init; }
    [JsonIgnore] public bool success => !hash.IsEmpty();
    [JsonIgnore] public bool is_error => errorMsg.IsEmpty();
}
