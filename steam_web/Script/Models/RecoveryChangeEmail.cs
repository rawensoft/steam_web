using System.Text.Json.Serialization;
using SteamWeb.Extensions;

namespace SteamWeb.Script.Models;
public record RecoveryChangeEmail
{
    public string hash { get; init; }
    public string errorMsg { get; init; }
    /// <summary>
    /// true если нужно подтвердить код с почты
    /// </summary>
    public bool show_confirmation { get; init; } = false;

    [JsonIgnore] public bool is_hash => !hash.IsEmpty();
    [JsonIgnore] public bool is_error => errorMsg.IsEmpty();
}
