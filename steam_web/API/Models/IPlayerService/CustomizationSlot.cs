using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IPlayerService;
public class CustomizationSlot
{
    [JsonPropertyName("slot")] public ushort Slot { get; init; }
    [JsonPropertyName("appid")] public uint AppId { get; init; }
    [JsonPropertyName("replay_year")] public ushort replay_year { get; init; }
    [JsonPropertyName("accountid")] public uint accountid { get; init; }
    [JsonPropertyName("publishedfileid")] public ulong publishedfileid { get; init; }
    [JsonPropertyName("title")] public string? title { get; init; }
}