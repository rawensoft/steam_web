using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IPlayerService;
public class PlayerQuests
{
    [JsonPropertyName("quests")] public Quest[] Quests { get; init; } = Array.Empty<Quest>();
}