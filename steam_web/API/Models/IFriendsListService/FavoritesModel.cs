using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IFriendsListService;
public class FavoritesModel
{
    [JsonPropertyName("favorites")] public FavoriteFriend[] Favorites { get; init; } = Array.Empty<FavoriteFriend>();
}