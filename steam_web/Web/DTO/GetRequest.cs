using System.Net;
using RestSharp;
using System.Web;
using SteamWeb.Auth.Interfaces;

namespace SteamWeb.Web.DTO;
public class GetRequest
{
    public string? Accept { get; set; } = null;
    public int CurrentRedirect { get; set; } = 0;
    public CookieContainer? CookieContainer { get; set; } = null;
    public int MaxRedirects { get; set; } = 10;
    public string Url { get; set; }
    public List<KeyValuePair<string, string>> QueryParametrs { get; private set; } = new(50);
    public List<KeyValuePair<string, string>> Headers { get; private set; } = new(50);
    public IWebProxy? Proxy { get; set; }
    public ISessionProvider? Session { get; set; }
    public string? UserAgent { get; set; }
    public string? Referer { get; set; }
    public string? Cookie { get; set; }
    public bool UseVersion2 { get; set; } = false;
    public bool IsAjax { get; set; } = false;
    public bool IsMobile { get; set; } = false;
    public int Timeout { get; set; } = 30000;
	public CancellationToken? CancellationToken { get; init; } = null;

	public GetRequest(string url) => Url = url;
    public GetRequest(string url, IWebProxy? proxy) : this(url) => Proxy = proxy;
    public GetRequest(string url, ISessionProvider? session) : this(url) => Session = session;
    public GetRequest(string url, IWebProxy? proxy, ISessionProvider? session) : this(url, proxy) => Session = session;
    public GetRequest(string url, IWebProxy? proxy, ISessionProvider? session, string referer) : this(url, proxy, session) => Referer = referer;
    public GetRequest(string url, IWebProxy? proxy, ISessionProvider? session, string referer, string userAgent) : this(url, proxy, session, referer) => UserAgent = userAgent;

    public GetRequest AddHeader(string name, string value)
    {
        Headers.Add(new(name, value));
        return this;
    }
    public GetRequest AddHeaders(RestRequest request)
    {
        foreach (var header in Headers)
            request.AddHeader(header.Key, header.Value);
        return this;
    }

    public GetRequest AddQuery(string name, string value, bool encode = true)
    {
        QueryParametrs.Add(new(name, encode ? HttpUtility.UrlEncode(value) : value));
        return this;
    }
    public GetRequest AddQuery(string name, byte value)
    {
        QueryParametrs.Add(new(name, value.ToString()));
        return this;
    }
    public GetRequest AddQuery(string name, short value)
    {
        QueryParametrs.Add(new(name, value.ToString()));
        return this;
    }
    public GetRequest AddQuery(string name, ushort value)
    {
        QueryParametrs.Add(new(name, value.ToString()));
        return this;
    }
    public GetRequest AddQuery(string name, long value)
    {
        QueryParametrs.Add(new(name, value.ToString()));
        return this;
    }
    public GetRequest AddQuery(string name, ulong value)
    {
        QueryParametrs.Add(new(name, value.ToString()));
        return this;
    }
    public GetRequest AddQuery(string name, int value)
    {
        QueryParametrs.Add(new(name, value.ToString()));
        return this;
    }
    public GetRequest AddQuery(string name, uint value)
    {
        QueryParametrs.Add(new(name, value.ToString()));
        return this;
    }
    public GetRequest AddQuery(string name, float value)
    {
        QueryParametrs.Add(new(name, value.ToString()));
        return this;
    }
    public GetRequest AddQuery(string name, double value)
    {
        QueryParametrs.Add(new(name, value.ToString()));
        return this;
    }
    public GetRequest AddQuery(string name, decimal value)
    {
        QueryParametrs.Add(new(name, value.ToString()));
        return this;
    }
    public GetRequest AddQuery(string name, bool value)
    {
        QueryParametrs.Add(new(name, value.ToString()));
        return this;
    }
    public void AddQuery(RestRequest request)
    {
        foreach (var query in QueryParametrs)
            request.AddQueryParameter(query.Key, query.Value, false);
    }

    public string GetUserAgent()
    {
        if (UserAgent != null)
            return UserAgent;

        if (UseVersion2)
            return KnownUserAgents.OkHttp;
        else
            return KnownUserAgents.WindowsBrowser;
    }
    public string GetAccept()
    {
        if (Accept != null)
            return Accept;
        if (IsMobile && IsAjax)
            return "text/javascript, text/html, application/xml, text/xml, *";
        if (UseVersion2)
            return "application/json, text/plain, */*";
        return "*/*";
    }
    public string? GetXRequestedWidth()
    {
        if (IsMobile && IsAjax)
            return "com.valvesoftware.android.steam.community";
        else if (IsAjax)
            return "XMLHttpRequest";
        return null;
    }
}