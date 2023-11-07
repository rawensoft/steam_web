using SteamWeb.Extensions;
using System.Collections.Concurrent;
using System.Net;
using SteamWeb.Auth.Interfaces;

namespace SteamWeb.Auth.v2.Models;
public class SessionData : ISessionProvider
{
    public const string DefaultMobileCookie = "mobileClient=android; mobileClientVersion=777777 3.6.3; ";
    public const string UserAgentMobile = "Mozilla/5.0 (Linux; Android 5.1.1; SM-G977N Build/LMY48Z; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/74.0.3729.136 Mobile Safari/537.36";
    public const string UserAgentBrowser = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.0.0 Safari/537.36";

    internal ConcurrentDictionary<string, Cookie> _cookies { get; init; } = new();
    public string SessionID { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string BrowserId { get; set; }
    public string SteamCountry { get; set; }
    public ulong SteamID { get; set; }
    public EAuthTokenPlatformType PlatformType { get; init; }
    public string SteamLanguage { get; set; } = "english";

    public void AddCookieToContainer(CookieContainer? container, Uri url, bool inc_mobile_cookie)
    {
        if (inc_mobile_cookie)
            AddMobileCookieToContainer(container, url);
        AddCookieToContainer(container, url);
    }
    private void AddMobileCookieToContainer(CookieContainer? container, Uri url)
    {
        if (container == null)
            container = new();
        switch (url.Host)
        {
            case "steamcommunity.com":
            case "store.steampowered.com":
            case "help.steampowered.com":
            case "api.steampowered.com":
            case "steampowered.com":
                container.Add(url, new Cookie("mobileClient", "android") { Secure = true });
                container.Add(url, new Cookie("mobileClientVersion", "777777 3.1.0") { Secure = true });
                break;
        }
    }
    public void AddCookieToContainer(CookieContainer? container, Uri url)
    {
        if (container == null)
            container = new();
        switch (url.Host)
        {
            case "steamcommunity.com":
            case "store.steampowered.com":
            case "help.steampowered.com":
            case "api.steampowered.com":
            case "steampowered.com":
                if (!SessionID.IsEmpty())
                    container.Add(url, new Cookie("sessionid", SessionID) { Secure = true });
                if (!AccessToken.IsEmpty())
                    container.Add(url, new Cookie("steamLoginSecure", $"{SteamID}%7C%7C{AccessToken}") { Secure = true, HttpOnly = true });
                if (!BrowserId.IsEmpty())
                    container.Add(url, new Cookie("browserid", BrowserId) { Secure = true });
                if (!SteamCountry.IsEmpty())
                    container.Add(url, new Cookie("steamCountry", SteamCountry) { Secure = true, HttpOnly = true });
                container.Add(url, new Cookie("Steam_Language", SteamLanguage) { Secure = true });
                break;
        }
        var values = _cookies.Values;
        foreach (var cookie in values)
            container.Add(url, cookie);
    }

    public void RewriteCookie(CookieContainer container)
    {
        var collection = container.GetAllCookies();
        foreach (Cookie cookie in collection)
        {
            if (cookie.Value.IsEmpty()) continue;
            if (cookie.Domain == "steamcommunity.com" || cookie.Domain == "store.steampowered.com" ||
                cookie.Domain == "help.steampowered.com" || cookie.Domain == "api.steampowered.com")
            {
                if (cookie.Name == "sessionid")
                    SessionID = cookie.Value;
                else if (cookie.Name == "steamCountry")
                    SteamCountry = cookie.Value;
                else if (cookie.Name == "browserid")
                    BrowserId = cookie.Value;
                else if (cookie.Name == "Steam_Language")
                    SteamLanguage = cookie.Value;
                else _cookies.AddOrUpdate(cookie.Name, cookie, (key, oldValue) => cookie);
            }
        }
	}
	public void RewriteCookie(CookieCollection collection)
	{
		foreach (Cookie cookie in collection)
		{
			if (cookie.Value.IsEmpty()) continue;
			if (cookie.Domain == "steamcommunity.com" || cookie.Domain == "store.steampowered.com" ||
				cookie.Domain == "help.steampowered.com" || cookie.Domain == "api.steampowered.com")
			{
				if (cookie.Name == "sessionid")
					SessionID = cookie.Value;
				else if (cookie.Name == "steamCountry")
					SteamCountry = cookie.Value;
				else if (cookie.Name == "browserid")
					BrowserId = cookie.Value;
				else if (cookie.Name == "Steam_Language")
					SteamLanguage = cookie.Value;
				else _cookies.AddOrUpdate(cookie.Name, cookie, (key, oldValue) => cookie);
			}
		}
	}

	public string ToStringCookie() => $"{DefaultMobileCookie}steamLoginSecure={SteamID}%7C%7C{AccessToken}; steamCountry={SteamCountry}; browserid={BrowserId}; sessionid={SessionID}";
}
