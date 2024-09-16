using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IPlayerService;
public class PurchasedCustomization
{
    [JsonPropertyName("purchaseid")] public ulong PurchaseId { get; init; }
    [JsonPropertyName("customization_type")] public ushort CustomizationType { get; init; }
    [JsonPropertyName("level")] public ushort Level { get; init; }
}