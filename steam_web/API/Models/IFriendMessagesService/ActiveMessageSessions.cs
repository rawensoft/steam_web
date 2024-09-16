using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IFriendMessagesService;
public class ActiveMessageSessions
{
    [JsonPropertyName("message_sessions")] public MessageSession[] MessageSessions { get; init; } = Array.Empty<MessageSession>();
    [JsonPropertyName("timestamp")] public int Timestamp { get; init; }
}