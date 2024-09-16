using System.Text.Json.Serialization;
using SteamWeb.Enums;

namespace SteamWeb.API.Models.IFriendsListService;
public class FriendModel
{
    [JsonPropertyName("ulfriendid")] public uint FriendId { get; init; }
    [JsonPropertyName("efriendrelationship")] public EFriendRelationship FriendRelationship { get; init; }
}