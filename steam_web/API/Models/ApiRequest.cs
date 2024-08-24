using System.Net;
using SteamWeb.Extensions;

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

    internal GetRequest AddAuthToken(GetRequest request)
    {
        if (!AuthToken.IsEmpty())
        {
            var length = AuthToken!.Length;
            if (length == 33 || length == 32)
                request.AddQuery("key", AuthToken!);
            else
                request.AddQuery("access_token", AuthToken!);
        }
        return request;
    }
    internal PostRequest AddAuthToken(PostRequest request)
    {
        if (!AuthToken.IsEmpty())
        {
            var length = AuthToken!.Length;
            if (length == 33 || length == 32)
                request.AddQuery("key", AuthToken!);
            else
                request.AddQuery("access_token", AuthToken!);
        }
        return request;
    }
}