namespace SteamWeb.Auth.Interfaces;
public interface ISessionProvider
{
    public string SessionID { get; set; }
    public string AccessToken { get; set; }
    public ulong SteamID { get; set; }
    public string SteamLanguage { get; set; }

    public void AddCookieToContainer(System.Net.CookieContainer container, Uri url);
    public void RewriteCookie(System.Net.CookieContainer container);
	public void RewriteCookie(System.Net.CookieCollection collection);

	public string ToStringCookie();
}
