using SteamWeb.Auth.v1.Enums;
using SteamWeb.Extensions;
using System.Net;
using SteamWeb.Auth.Interfaces;

namespace SteamWeb.Auth.v1.Models;
public class SessionData : ISessionProvider
{
    private string sessionID;
    private string steamLogin;
    private string steamLoginSecure;
    private string steamMachineAuth;
    private string steamRememberLogin;
    private string oAuthToken;
    private string webCookie;
    private string steamCountry;
    private string browserID;
    private string steamLanguage = "english";

    /// <summary>
    /// Вызывается при обновлении данных сессии
    /// </summary>
    /// <param name="session">Сессия, данные которой были обновлены</param>
    public delegate void OnSessionUpdatedHandler(SessionData session);
    /// <summary>
    /// Вызывается при обновлении данных сессии
    /// </summary>
    public event OnSessionUpdatedHandler OnSessionUpdated;
    public string SessionID
    {
        get => sessionID;
        set
        {
            if (sessionID == value) return;
            sessionID = value;
            OnSessionUpdated?.Invoke(this);
        }
    }
    public string SteamLogin
    {
        get => steamLogin;
        set
        {
            if (steamLogin == value) return;
            steamLogin = value;
            OnSessionUpdated?.Invoke(this);
        }
    }
    public string SteamLoginSecure
    {
        get => steamLoginSecure;
        set
        {
            if (steamLoginSecure == value) return;
            steamLoginSecure = value;
            OnSessionUpdated?.Invoke(this);
        }
    }
    public string SteamMachineAuth
    {
        get => steamMachineAuth;
        set
        {
            if (steamMachineAuth == value) return;
            steamMachineAuth = value;
            OnSessionUpdated?.Invoke(this);
        }
    }
    public string SteamRememberLogin
    {
        get => steamRememberLogin;
        set
        {
            if (steamRememberLogin == value) return;
            steamRememberLogin = value;
            OnSessionUpdated?.Invoke(this);
        }
    }
    public string OAuthToken
    {
        get => oAuthToken;
        set
        {
            if (oAuthToken == value) return;
            oAuthToken = value;
            OnSessionUpdated?.Invoke(this);
        }
    }
    public string WebCookie
    {
        get => webCookie;
        set
        {
            if (webCookie == value) return;
            webCookie = value;
            OnSessionUpdated?.Invoke(this);
        }
    }
    public string SteamCountry
    {
        get => steamCountry;
        set
        {
            if (steamCountry == value) return;
            steamCountry = value;
            OnSessionUpdated?.Invoke(this);
        }
    }
    public string BrowserID
    {
        get => browserID;
        set
        {
            if (browserID == value) return;
            browserID = value;
            OnSessionUpdated?.Invoke(this);
        }
    }
    public string SteamLanguage
    {
        get => steamLanguage;
        set
        {
            if (steamLanguage == value) return;
            steamLanguage = value;
            OnSessionUpdated?.Invoke(this);
        }
    }
    public ulong SteamID { get; set; }
    public SignInPlatform Platform { get; init; } = SignInPlatform.Mobile;

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
                container.Add(url, new Cookie("mobileClientVersion", "0 (2.1.3)") { Secure = true });
                container.Add(url, new Cookie("Steam_Language", SteamLanguage) { Secure = true });
                container.Add(url, new Cookie("dob", "") { Secure = true });
                if (SteamID > 0)
                    container.Add(url, new Cookie("steamid", SteamID.ToString()) { Secure = true });
                if (!SessionID.IsEmpty())
                    container.Add(url, new Cookie("sessionid", SessionID) { Secure = true });
                if (!SteamLogin.IsEmpty())
                    container.Add(url, new Cookie("steamLogin", SteamLogin) { HttpOnly = true });
                if (!SteamLoginSecure.IsEmpty())
                    container.Add(url, new Cookie("steamLoginSecure", SteamLoginSecure) { Secure = true, HttpOnly = true });
                break;
        }
    }

    public void AddCookieToContainer(CookieContainer? container, Uri url)
    {
        if (container == null)
            container = new();
        if (Platform == SignInPlatform.Mobile)
            AddCookies(container, url);
        else
        {
            switch (url.Host)
            {
                case "steamcommunity.com":
                case "store.steampowered.com":
                case "help.steampowered.com":
                case "api.steampowered.com":
                case "steampowered.com":
                    container.Add(url, new Cookie("Steam_Language", SteamLanguage) { Secure = true });
                    if (!SessionID.IsEmpty())
                        container.Add(url, new Cookie("sessionid", SessionID) { Secure = true });
                    if (!SteamLoginSecure.IsEmpty())
                        container.Add(url, new Cookie("steamLoginSecure", SteamLoginSecure) { Secure = true, HttpOnly = true });
                    if (!SteamRememberLogin.IsEmpty())
                        container.Add(url, new Cookie("steamRememberLogin", SteamRememberLogin) { Secure = true });
                    if (!SteamMachineAuth.IsEmpty())
                        container.Add(url, new Cookie("steamMachineAuth" + SteamID, SteamMachineAuth));
                    break;
            }
        }
    }
    public string ToStringCookie()
    {
        string cookie = "";
        if (!SessionID.IsEmpty())
            cookie += $"sessionid={SessionID}; ";
        if (!SteamLogin.IsEmpty())
            cookie += $"steamLogin={SteamLogin}; ";
        if (!SteamLoginSecure.IsEmpty())
            cookie += $"steamLoginSecure={SteamLoginSecure}; ";
        if (!WebCookie.IsEmpty())
            cookie += $"steamMachineAuth{SteamID}={WebCookie}; ";
        if (cookie == "")
            return null;
        cookie += "Steam_Language=english; ";
        return cookie;
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
                else if (cookie.Name == "steamLoginSecure") SteamLoginSecure = cookie.Value;
                else if (cookie.Name == "steamRememberLogin") SteamRememberLogin = cookie.Value;
                else if (cookie.Name.StartsWith("steamMachineAuth")) SteamMachineAuth = cookie.Value;
                else if (cookie.Name == "steamCountry") SteamCountry = cookie.Value;
                else if (cookie.Name == "browserid") BrowserID = cookie.Value;
                else if (cookie.Name == "Steam_Language") SteamLanguage = cookie.Value;
            }
        }
    }
}
