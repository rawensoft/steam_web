using System.Net;

namespace SteamWeb.Web.Models;

readonly struct ClientIndexData
{
    public int UriHash { get; }
    public int ProxyHash { get; }
    public int UserAgent { get; }

    public ClientIndexData(string url, IWebProxy? proxy, string ua) : this(new Uri(url), proxy, ua) { }
    public ClientIndexData(Uri uri, IWebProxy? proxy, string ua)
    {
        UriHash = uri.Authority.GetHashCode();
        ProxyHash = proxy?.GetProxy(uri)?.GetHashCode() ?? -1;
        UserAgent = ua.GetHashCode();
    }
}
