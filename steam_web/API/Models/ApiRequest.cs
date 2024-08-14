using System.Net;

namespace SteamWeb.API.Models;
public class ApiRequest
{
    /// <summary>
    /// access_token или api key
    /// </summary>
    public string? AuthToken { get; init; }
    public IWebProxy? Proxy { get; init; }
    public CancellationToken? CancellationToken { get; init; }

    public ApiRequest() { }
    public ApiRequest(string? auth_token) => AuthToken = auth_token;
    public ApiRequest(string? auth_token, IWebProxy? proxy) : this(auth_token) => Proxy = proxy;
    public ApiRequest(string? auth_token, IWebProxy? proxy, CancellationToken? cancellationToken) : this(auth_token, proxy)
        => CancellationToken = cancellationToken;
}