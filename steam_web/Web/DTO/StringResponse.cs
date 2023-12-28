using System.Net;
using RestSharp;

namespace SteamWeb.Web.DTO;
public class StringResponse : Response
{
    public string? Data { get; init; } = null;
    public StringResponse(RestResponse res) : base(res) => Data = res.Content;
    public StringResponse(RestResponse res, CookieContainer? cookies) : base(res, cookies) => Data = res.Content;
	public StringResponse(RestResponse res, CookieCollection? cookies) : base(res, cookies) => Data = res.Content;
	public StringResponse(HttpWebResponse res) : base(res)
    {
        if (res != null && res.ContentLength > 0)
        {
            using var sr = new StreamReader(res.GetResponseStream());
            Data = sr.ReadToEnd();
        }
    }
}