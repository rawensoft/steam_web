using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO;
public class StoreUserConfig
{
    [JsonPropertyName("webapi_token")] public string? WebApiToken { get; init; }
}