using System.Text.Json.Serialization;
using Response = SteamWeb.Script.DTO.Response;

namespace SteamWeb.Script.Models;
public class RecoveryConfirmation : Response
{
    [JsonPropertyName("continue")] public bool Continue { get; init; } = false;
}