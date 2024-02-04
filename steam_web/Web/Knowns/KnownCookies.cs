namespace SteamWeb.Web;
public static class KnownCookies
{
    internal const string COOKIE_NAME_SESSIONID = "sessionid";
    internal const string COOKIE_NAME_STEAMCOUNTRY = "steamCountry";
    internal const string COOKIE_NAME_BROWSERID = "browserid";
    internal const string COOKIE_NAME_STEAMLANGUAGE = "Steam_Language";

    public const string DefaultMobileCookie = "mobileClient=android; mobileClientVersion=777777 3.7.2; ";

    internal static System.Net.Cookie MobileClient { get; } = new System.Net.Cookie("mobileClient", "android") { Secure = true };
    internal static System.Net.Cookie MobileClientVersion { get; } = new System.Net.Cookie("mobileClientVersion", "777777 3.7.2") { Secure = true };
}