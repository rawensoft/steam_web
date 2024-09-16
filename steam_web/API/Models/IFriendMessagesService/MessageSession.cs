using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IFriendMessagesService;
public class MessageSession
{
    /// <summary>
    /// SteamId32 друга
    /// </summary>
    [JsonPropertyName("accountid_friend")] public uint AccountIdFriend {  get; init; }
    /// <summary>
    /// Время прихода сообщения
    /// </summary>
    [JsonPropertyName("last_message")] public int LastMessage { get; init; }
    /// <summary>
    /// Последнее время просмотра диалога
    /// </summary>
    [JsonPropertyName("last_view")] public int LastView { get; init; }
    /// <summary>
    /// Количество не прочитанных сообщений
    /// </summary>
    [JsonPropertyName("unread_message_count")] public uint UnreadMessageCount { get; init; }
}