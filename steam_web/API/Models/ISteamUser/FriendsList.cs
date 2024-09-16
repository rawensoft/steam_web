using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.ISteamUser;
public class FriendsList<T>
{
    [JsonPropertyName("friends")] public T[] Friends { get; init; } = Array.Empty<T>();
}