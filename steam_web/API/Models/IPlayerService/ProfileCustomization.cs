using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IPlayerService;
public class ProfileCustomization
{
    /// <summary>
    /// Доступно если (include_inactive_customizations || include_purchased_customizations)
    /// </summary>
    [JsonPropertyName("customizations")] public Customization[] Customizations { get; init; } = Array.Empty<Customization>();
    [JsonPropertyName("slots_available")] public ushort SlotsAvailable { get; init; }
    [JsonPropertyName("profile_theme")] public ProfileTheme ProfileTheme { get; init; } = new();
    [JsonPropertyName("purchased_customizations")] public PurchasedCustomization[] PurchasedCustomizations { get; init; } = Array.Empty<PurchasedCustomization>();
    [JsonPropertyName("profile_preferences")] public ProfilePreferences ProfilePreferences { get; init; } = new();
}