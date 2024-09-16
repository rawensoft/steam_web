using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.ISteamEconomy;
public class ClassModel
{
    [JsonPropertyName("name")] public string Name { get; init; } = string.Empty;
    [JsonPropertyName("value")] public string Value { get; init; } = string.Empty;
}