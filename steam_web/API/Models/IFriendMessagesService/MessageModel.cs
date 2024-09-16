using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IFriendMessagesService;
public class MessageModel
{
    [JsonPropertyName("accountid")] public uint AccountId { get; init; }
    [JsonPropertyName("timestamp")] public int Timestamp { get; init; }
    [JsonPropertyName("message")] public string Message {  get; init; }
}