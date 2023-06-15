using System.Text.Json.Serialization;
namespace SteamWeb.Models;
public record InvormationProfileResponse
{
    [JsonIgnore] public bool IsSuccess => success == 1;
    public int success { get; init; } = 0;
    public string errmsg { get; init; } = "None";
}
