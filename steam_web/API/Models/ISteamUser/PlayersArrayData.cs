using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.ISteamUser;
public class PlayersArrayData<T>
{
    [JsonPropertyName("players")] public List<T> Players { get; init; } = new(1);
}
