using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.ISteamUser;
public class PlayerBans
{
    [JsonPropertyName("players")] public List<PlayerBan> Players { get; init; } = new(2);
    [JsonPropertyName("success")] public bool Success { get; set; } = false;
}