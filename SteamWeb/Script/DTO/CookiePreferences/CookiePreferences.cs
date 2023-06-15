namespace SteamWeb.Script.DTO.CookiePreferences;

public class CookiePreferences
{
    public int version { get; init; } = 1;
    public int preference_state { get; init; } = 3;
    public ContentCustomization content_customization { get; init; } = new();
    public ValveAnalytics valve_analytics { get; init; } = new();
    public ThirdPartyAnalytics third_party_analytics { get; init; } = new();
    public ThirdPartyContent third_party_content { get; init; } = new();
    public bool utm_enabled { get; init; } = false;
}
