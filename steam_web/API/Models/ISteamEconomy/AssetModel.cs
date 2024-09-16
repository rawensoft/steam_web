using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.ISteamEconomy;
public class AssetModel
{
    [JsonPropertyName("prices")]
#if NET8_0_OR_GREATER
    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public Dictionary<string, uint> Prices { get; } = new(44);
#elif NET5_0_OR_GREATER
    public Dictionary<string, uint> Prices { get; init; } = new(1);
#endif

    [JsonPropertyName("name")] public string Name { get; init; } = string.Empty;
    [JsonPropertyName("date")] public string Date { get; init; } = string.Empty;
    [JsonPropertyName("classid")] public string ClassId { get; init; } = string.Empty;
    [JsonPropertyName("class")] public ClassModel[] Class { get; init; } = Array.Empty<ClassModel>();
}