using SteamWeb.Extensions;
using System.Net;
using SteamWeb.Auth.Interfaces;
using System.Text.Json.Serialization;
using System.Text;

namespace SteamWeb.Auth.v1.Models;
public class SessionData : ISessionProvider
{
	private string? _sessionID;
	private string? _accessToken;
	private string? _refreshToken;
	private string _steamLanguage = "english";

    /// <summary>
    /// Вызывается при обновлении данных сессии
    /// </summary>
    /// <param name="session">Сессия, данные которой были обновлены</param>
    public delegate void OnSessionUpdatedHandler(SessionData session);
    /// <summary>
    /// Вызывается при обновлении данных сессии
    /// </summary>
    public event OnSessionUpdatedHandler? OnSessionUpdated;
    
    [JsonIgnore] public string SteamLanguage
    {
        get => _steamLanguage;
        set
        {
            if (_steamLanguage == value) return;
			_steamLanguage = value;
            OnSessionUpdated?.Invoke(this);
        }
    }
	public string? SessionID
	{
		get => _sessionID;
		set
		{
			if (_sessionID == value)
				return;
			_sessionID = value;
			OnSessionUpdated?.Invoke(this);
		}
	}
	public string? AccessToken
	{
		get => _accessToken;
		set
		{
			if (_accessToken == value)
				return;
			_accessToken = value;
			OnSessionUpdated?.Invoke(this);
		}
	}
	public string? RefreshToken
	{
		get => _refreshToken;
		set
		{
			if (_refreshToken == value)
				return;
			_refreshToken = value;
			OnSessionUpdated?.Invoke(this);
		}
	}
	public ulong SteamID { get; set; }


	/// <summary>
	/// Добавляет мобильные куки
	/// </summary>
	/// <param name="cookies">Куда добавлять</param>
	private void AddCookies(CookieContainer? container, Uri url)
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
                container.Add(url, new Cookie("mobileClientVersion", "777777 3.6.4") { Secure = true });
                container.Add(url, new Cookie("Steam_Language", SteamLanguage) { Secure = true });
				container.Add(url, new Cookie("steamLoginSecure", GetSteamLoginSecure()) { Secure = true, HttpOnly = true });
				if (!SessionID.IsEmpty())
                    container.Add(url, new Cookie("sessionid", SessionID) { Secure = true });
                break;
        }
    }

    public void AddCookieToContainer(CookieContainer? container, Uri url)
    {
        if (container == null)
            container = new();
		AddCookies(container, url);
	}
    public string ToStringCookie()
    {
        var sb = new StringBuilder(10);
		sb.Append("Steam_Language=");
		sb.Append(SteamLanguage);
		sb.Append("; mobileClientVersion=777777 3.6.4; mobileClient=android; ");
        if (!SessionID.IsEmpty())
		{
			sb.Append("sessionid=");
			sb.Append(SessionID);
			sb.Append("; ");
		}
		sb.Append("steamLoginSecure=");
		sb.Append(SteamID);
		sb.Append("%7C%7C");
		sb.Append(AccessToken);
        return sb.ToString();
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
                if (cookie.Name == "sessionid") SessionID = cookie.Value;
                else if (cookie.Name == "Steam_Language") SteamLanguage = cookie.Value;
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
				if (cookie.Name == "sessionid") SessionID = cookie.Value;
				else if (cookie.Name == "Steam_Language") SteamLanguage = cookie.Value;
			}
		}
	}

	private string GetSteamLoginSecure() => SteamID + "%7C%7C" + AccessToken;
}
