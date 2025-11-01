using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO;

public class MarketConfirmation
{
    [JsonPropertyName("confirmation_id")]
    public ulong ConfirmationId { get; init; }
}