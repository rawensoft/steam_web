using System.Net;
using SteamWeb.Auth.Interfaces;

namespace SteamWeb.Web.DTO;

public class ProtobufRequest
{
    public string Url { get; init; }
    public string? AccessToken { get; init; }
    public string ProtoData { get; init; }
    public IWebProxy? Proxy { get; init; }
    public ISessionProvider? Session { get; init; }
    public string? UserAgent { get; init; }
    public string? Cookie { get; init; }
    public bool IsMobile { get; init; } = true;
    public int Timeout { get; set; } = 30000;
	public CancellationToken? CancellationToken { get; init; } = null;

	public ProtobufRequest(string url, string protoData)
    {
        Url = url;
        ProtoData = protoData;
    }
}
