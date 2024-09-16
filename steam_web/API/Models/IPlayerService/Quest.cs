using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IPlayerService;
public class Quest
{
    [JsonPropertyName("questid")] public ushort QuestId { get; init; }
    [JsonPropertyName("completed")] public bool Completed { get; init; } = false;
}