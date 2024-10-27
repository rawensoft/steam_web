using SteamWeb.Extensions;
using System.Collections.Concurrent;
using System.Net;
using SteamWeb.Auth.Interfaces;
using System.Text.Json.Serialization;
using System.Text;
using SteamWeb.Web;

namespace SteamWeb.Auth.v2.Models;
public sealed class SessionData : ISessionProvider, IEquatable<SessionData>
{
	#region private fields
	private Cookie? _sessionidCookie = null;
	private Cookie? _steamLoginSecureCookie = null;
	private Cookie? _browseridCookie = null;
	private Cookie? _steamCountryCookie = null;
	private Cookie? _steamLanguageCookie = null;

	private string? _sessionID;
	private string? _accessToken;
	private string? _browserId;
	private string? _steamCountry;
	private string _steamLanguage = "english";
	private ulong _steamID;
	#endregion
	#region public fields
	internal ConcurrentDictionary<string, Cookie> _cookies { get; init; } = new(2, 24);
	[JsonPropertyName("session_id")] public string? SessionID
	{
		get => _sessionID;
		set
		{
			_sessionID = value;
			if (value.IsEmpty())
				_sessionidCookie = null;
			else if (_sessionidCookie == null)
				_sessionidCookie = new Cookie("sessionid", SessionID) { Secure = true };
			else
				_sessionidCookie.Value = value;
		}
	}
	[JsonPropertyName("access_token")] public string? AccessToken
	{
		get => _accessToken;
		set
		{
			_accessToken = value;
			if (value.IsEmpty() || SteamID == 0)
				_steamLoginSecureCookie = null;
			else if (_steamLoginSecureCookie == null)
				_steamLoginSecureCookie = new Cookie("steamLoginSecure", SteamID + "%7C%7C" + value) { Secure = true, HttpOnly = true };
			else
				_steamLoginSecureCookie.Value = SteamID + "%7C%7C" + value;
		}
	}
	[JsonPropertyName("refresh_token")] public string? RefreshToken { get; set; }
	[JsonPropertyName("browser_id")] public string? BrowserId
	{
		get => _browserId;
		set
		{
			_browserId = value;
			if (value.IsEmpty())
				_browseridCookie = null;
			else if (_browseridCookie == null)
				_browseridCookie = new Cookie("browserid", BrowserId) { Secure = true };
			else
				_browseridCookie.Value = value;
		}
	}
	[JsonPropertyName("steam_country")] public string? SteamCountry
	{
		get => _steamCountry;
		set
		{
			_steamCountry = value;
			if (value.IsEmpty())
				_steamCountryCookie = null;
			else if (_steamCountryCookie == null)
				_steamCountryCookie = new Cookie("steamCountry", SteamCountry) { Secure = true, HttpOnly = true };
			else
				_steamCountryCookie.Value = value;
		}
	}
	[JsonPropertyName("steam_id")] public ulong SteamID
	{
		get => _steamID;
		set
		{
			_steamID = value;
			if (AccessToken.IsEmpty() || value == 0)
				_steamLoginSecureCookie = null;
			else if (_steamLoginSecureCookie == null)
				_steamLoginSecureCookie = new Cookie("steamLoginSecure", value + "%7C%7C" + AccessToken) { Secure = true, HttpOnly = true };
			else
				_steamLoginSecureCookie.Value = value + "%7C%7C" + AccessToken;
		}
	}
	[JsonPropertyName("platform")] public EAuthTokenPlatformType PlatformType { get; init; }
	[JsonPropertyName("steam_language")] public string SteamLanguage
	{
		get => _steamLanguage;
		set
		{
			_steamLanguage = value;
			if (value.IsEmpty())
				_steamLanguageCookie = null;
			else if (_steamLanguageCookie == null)
				_steamLanguageCookie = new Cookie("Steam_Language", SteamLanguage) { Secure = true };
			else
				_steamLanguageCookie.Value = value;
		}
	}
	#endregion

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
		if (!cookie.Value.IsEmpty() && (cookie.Domain == KnownUri.HOST_COMMUNITY || cookie.Domain == KnownUri.HOST_API ||
			cookie.Domain == KnownUri.HOST_STORE || cookie.Domain == KnownUri.HOST_HELP))
		{
			if (cookie.Name == KnownCookies.COOKIE_NAME_SESSIONID)
				SessionID = cookie.Value;
			else if (cookie.Name == KnownCookies.COOKIE_NAME_STEAMCOUNTRY)
				SteamCountry = cookie.Value;
			else if (cookie.Name == KnownCookies.COOKIE_NAME_BROWSERID)
				BrowserId = cookie.Value;
			else if (cookie.Name == KnownCookies.COOKIE_NAME_STEAMLANGUAGE)
				SteamLanguage = cookie.Value;
			else if (cookie.Name == KnownCookies.COOKIE_NAME_STEAMLOGINSECURE)
			{
				var splitted = cookie.Value.Split("%7C%7C");
                if (splitted.Length == 2)
					AccessToken = splitted[1];
			}
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

	public bool Equals(SessionData? other)
	{
		if (SessionID != other?.SessionID)
			return false;
		if (AccessToken != other?.AccessToken)
			return false;
		if (RefreshToken != other?.RefreshToken)
			return false;
		if (BrowserId != other?.BrowserId)
			return false;
		if (SteamCountry != other?.SteamCountry)
			return false;
		if (PlatformType != other?.PlatformType)
			return false; 
		if (SteamLanguage != other?.SteamLanguage)
			return false;
		if (SteamID != other.SteamID)
			return false;

		return true;
	}
}