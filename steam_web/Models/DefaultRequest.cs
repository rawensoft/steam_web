using System.Net;
using SteamWeb.Auth.Interfaces;

namespace SteamWeb.Models;
public class DefaultRequest
{
    public ISessionProvider? Session { get; init; }
    public IWebProxy? Proxy { get; init; }
    public CancellationToken? CancellationToken { get; init; }

    public DefaultRequest() { }
    public DefaultRequest(IWebProxy? proxy) => Proxy = proxy;
    public DefaultRequest(ISessionProvider? session) => Session = session;
    public DefaultRequest(ISessionProvider? session, IWebProxy? proxy) : this(session) => Proxy = proxy;
    public DefaultRequest(ISessionProvider? session, IWebProxy? proxy, CancellationToken? cancellationToken) : this(session, proxy)
        => CancellationToken = cancellationToken;
}