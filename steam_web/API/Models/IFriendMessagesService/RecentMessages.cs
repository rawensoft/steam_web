using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IFriendMessagesService;
public class RecentMessages
{
    [JsonPropertyName("messages")] public MessageModel[] Messages { get; init; } = Array.Empty<MessageModel>();
    [JsonPropertyName("more_available")] public bool MoreAvailable { get; init; } = false;
}