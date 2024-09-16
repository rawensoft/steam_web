using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IFriendsListService;
public class FriendsList
{
    [JsonPropertyName("bincremental")] public bool bIncremental { get; init; } = false;
    [JsonPropertyName("friends")] public FriendModel[] Friends { get; init; } = Array.Empty<FriendModel>();
}