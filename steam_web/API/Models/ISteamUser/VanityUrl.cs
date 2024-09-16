using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.ISteamUser;
public class VanityUrl
{
    [JsonIgnore] public bool Succesul => Response == 1;
    [JsonPropertyName("response")] public ushort Response { get; init; } = 0;

    /// <summary>
    /// Есть значение если b_response == false
    /// </summary>
    [JsonPropertyName("message")] public string? Message { get; init; }

    /// <summary>
    /// Есть значение если b_response == true
    /// </summary>
    [JsonPropertyName("steamid")] public ulong SteamId { get; init; }
}
