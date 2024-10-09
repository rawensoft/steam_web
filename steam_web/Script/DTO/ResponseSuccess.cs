using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO;
public class ResponseSuccess
{
    [JsonPropertyName("success")] public byte Success { get; init; } = 0;
}