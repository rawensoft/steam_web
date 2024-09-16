using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IPlayerService;
public class PlayerSteamLevel
{
    [JsonPropertyName("player_level")] public ushort PlayerLevel { get; set; }
}