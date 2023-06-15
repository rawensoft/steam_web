namespace SteamWeb.Auth.v1.Models;

public interface IOAuth
{
    public string SteamID { get; init; }
    public string OAuthToken { get; init; }
    public string SteamLoginSecure { get; init; }
    public string WebCookie { get; init; }
}
