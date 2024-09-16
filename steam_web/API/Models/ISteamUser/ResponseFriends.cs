using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.ISteamUser;
public class ResponseFriends<T>
{
    [JsonPropertyName("success")] public bool Success { get; set; } = false;
    [JsonPropertyName("friendslist")] public FriendsList<T> FriendsList { get; init; } = new();
}