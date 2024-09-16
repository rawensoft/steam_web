using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.ISteamApps;
public class UpToDateModel
{
    [JsonPropertyName("success")] public bool Success { get; init; } = false;
    [JsonPropertyName("up_to_date")] public bool UpToDate { get; init; } = false;
    [JsonPropertyName("version_is_listable")] public bool VersionIsListable { get; init; } = false;
    [JsonPropertyName("required_version")] public uint RequiredVersion { get; init; }
    [JsonPropertyName("message")] public string? Message { get; init; }
}