namespace SteamWeb.Web;
public static class KnownCookies
{
    internal const string COOKIE_NAME_SESSIONID = "sessionid";
    internal const string COOKIE_NAME_STEAMCOUNTRY = "steamCountry";
    internal const string COOKIE_NAME_BROWSERID = "browserid";
    internal const string COOKIE_NAME_STEAMLANGUAGE = "Steam_Language";
	internal const string COOKIE_NAME_STEAMLOGINSECURE = "steamLoginSecure";
    internal const string COOKIE_NAME_СOOKIESETTINGS = "cookieSettings";
    internal const string COOKIE_NAME_СOOKIESETTINGS_VALUE = "%7B%22version%22%3A1%2C%22preference_state%22%3A1%2C%22content_customization%22%3Anull%2C%22valve_analytics%22%3Anull%2C%22third_party_analytics%22%3Anull%2C%22third_party_content%22%3Anull%2C%22utm_enabled%22%3Atrue%7D";


    public const string DefaultMobileCookie = "mobileClient=android; mobileClientVersion=777777 3.7.2; ";

    internal static System.Net.Cookie MobileClient { get; } = new System.Net.Cookie("mobileClient", "android") { Secure = true };
    internal static System.Net.Cookie MobileClientVersion { get; } = new System.Net.Cookie("mobileClientVersion", "777777 3.7.2") { Secure = true };
}