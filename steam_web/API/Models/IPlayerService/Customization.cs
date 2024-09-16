using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IPlayerService;
public class Customization
{
    [JsonPropertyName("customization_type")] public ushort CustomizationType { get; init; }
    [JsonPropertyName("customization_style")] public ushort CustomizationStyle { get; init; }
    [JsonPropertyName("active")] public bool Active { get; init; } = false;
    [JsonPropertyName("large")] public bool Large { get; init; } = false;
    [JsonPropertyName("level")] public ushort Level { get; init; }
    [JsonPropertyName("purchaseid")] public ulong PurchaseId { get; init; }
    [JsonPropertyName("slots")] public CustomizationSlot[] Slots { get; init; } = Array.Empty<CustomizationSlot>();
}