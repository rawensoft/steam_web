using SteamWeb.Extensions;
using System.Collections.Concurrent;
using System.Net;
using SteamWeb.Auth.Interfaces;
using System.Text.Json.Serialization;
using System.Text;
using SteamWeb.Web;

namespace SteamWeb.Auth.v2.Models;
public class SessionData : ISessionProvider
{

	internal ConcurrentDictionary<string, Cookie> _cookies { get; init; } = new();
	[JsonPropertyName("session_id")] public string SessionID { get; set; }
	[JsonPropertyName("access_token")] public string AccessToken { get; set; }
	[JsonPropertyName("refresh_token")] public string RefreshToken { get; set; }
	[JsonPropertyName("browser_id")] public string BrowserId { get; set; }
	[JsonPropertyName("steam_country")] public string SteamCountry { get; set; }
	[JsonPropertyName("steam_id")] public ulong SteamID { get; set; }
	[JsonPropertyName("platform")] public EAuthTokenPlatformType PlatformType { get; init; }
	[JsonPropertyName("steam_language")] public string SteamLanguage { get; set; } = "english";

	public void AddCookieToContainer(CookieContainer? container, Uri url, bool inc_mobile_cookie)
    {
        if (inc_mobile_cookie)
            AddMobileCookieToContainer(container, url);
        AddCookieToContainer(container, url);
    }
    public void AddCookieToContainer(CookieContainer? container, Uri url)
    {
        if (container == null)
            container = new();
        switch (url.Host)
		{
			case KnownUri.HOST_COMMUNITY:
			case KnownUri.HOST_STORE:
			case KnownUri.HOST_HELP:
			case KnownUri.HOST_API:
			case KnownUri.HOST_POWERED:
				if (_sessionidCookie != null)
                    container.Add(url, _sessionidCookie);
				if (_steamLoginSecureCookie != null)
					container.Add(url, _steamLoginSecureCookie);
				if (_browseridCookie != null)
					container.Add(url, _browseridCookie);
				if (_steamCountryCookie != null)
					container.Add(url, _steamCountryCookie);
				if (_steamLanguageCookie != null)
					container.Add(url, _steamLanguageCookie);
                break;
        }
        var values = _cookies.Values;
        foreach (var cookie in values)
            container.Add(url, cookie);
	}
	private void AddMobileCookieToContainer(CookieContainer? container, Uri url)
	{
		if (container == null)
			container = new();
		switch (url.Host)
		{
			case KnownUri.HOST_COMMUNITY:
			case KnownUri.HOST_STORE:
			case KnownUri.HOST_HELP:
			case KnownUri.HOST_API:
			case KnownUri.HOST_POWERED:
				container.Add(url, KnownCookies.MobileClient);
				container.Add(url, KnownCookies.MobileClientVersion);
				break;
		}
	}

	public void RewriteCookie(CookieContainer container)
    {
        var collection = container.GetAllCookies();
		foreach (Cookie cookie in collection)
			RewriteCookie(cookie);
	}
	public void RewriteCookie(CookieCollection collection)
	{
		foreach (Cookie cookie in collection)
			RewriteCookie(cookie);
	}
	private void RewriteCookie(Cookie cookie)
	{
		if (cookie.Value.IsEmpty())
			return;
		if (cookie.Domain == KnownUri.HOST_COMMUNITY || cookie.Domain == KnownUri.HOST_API ||
			cookie.Domain == KnownUri.HOST_STORE || cookie.Domain == KnownUri.HOST_HELP)
		{
			if (cookie.Name == KnownCookies.COOKIE_NAME_SESSIONID)
				SessionID = cookie.Value;
			else if (cookie.Name == KnownCookies.COOKIE_NAME_STEAMCOUNTRY)
				SteamCountry = cookie.Value;
			else if (cookie.Name == KnownCookies.COOKIE_NAME_BROWSERID)
				BrowserId = cookie.Value;
			else if (cookie.Name == KnownCookies.COOKIE_NAME_STEAMLANGUAGE)
				SteamLanguage = cookie.Value;
			else
				_cookies.AddOrUpdate(cookie.Name, cookie, (key, oldValue) => cookie);
		}
	}

	public string ToStringCookie()
    {
        var sb = new StringBuilder(13);
        sb.Append(KnownCookies.DefaultMobileCookie);
		sb.Append("steamLoginSecure=");
		sb.Append(SteamID);
		sb.Append("%7C%7C");
		sb.Append(AccessToken);
		sb.Append("; steamCountry=");
		sb.Append(SteamCountry);
		sb.Append("; browserid=");
		sb.Append(BrowserId);
		sb.Append("; sessionid=");
		sb.Append(SessionID);
        return sb.ToString();
	}
}
