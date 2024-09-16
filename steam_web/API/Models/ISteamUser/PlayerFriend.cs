using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.ISteamUser;
public class PlayerFriend
{
    [JsonPropertyName("steamid")] public ulong SteamId { get; init; }
    /// <summary>
    /// friend
    /// requestrecipient
    /// ignored
    /// </summary>
    [JsonPropertyName("relationship")] public string Relationship { get; init; }
    [JsonPropertyName("friend_since")] public int FriendSince { get; init; }
}