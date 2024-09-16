using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IFriendsListService;
public class FavoriteFriend
{
    [JsonPropertyName("accountid")] public uint AccountId { get; init; }
}