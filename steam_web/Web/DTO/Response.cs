using System.Net;
using System.Text;
using SteamWeb.Extensions;
using RestSharp;

namespace SteamWeb.Web.DTO;

public class Response
{
    private const string HeaderNameLocation = "Location";
    private const string HeaderValueLocation = "steammobile://lostauth/";
    private const string HeaderNameXEResult = "X-eresult";

    public CookieContainer? CookieContainer { get; set; } = null;
    public bool Success { get; init; } = false;
    public string? Cookie { get; init; } = null;
    public int StatusCode { get; init; } = 0;
    public Exception? ErrorException { get; init; } = null;
    public string? ErrorMessage { get; init; } = null;
    public bool LostAuth { get; init; } = false;
    public EResult EResult { get; init; } = EResult.Invalid;

    public Response(RestResponse res) : this(res, null) { }
    public Response(RestResponse res, CookieContainer? cookies)
    {
        Success = res.IsSuccessful;
        StatusCode = (int)res.StatusCode;
        if (!Success && res.StatusCode == HttpStatusCode.Found)
        {
            foreach (var header in res.Headers!)
            {
                switch (header.Name)
                {
                    case HeaderNameLocation:
                        if (header.Value!.ToString() == HeaderValueLocation)
                            LostAuth = true;
                        break;
                    case HeaderNameXEResult:
                        if (header.Value != null)
                            EResult = (EResult)header.Value.ToString()!.ParseInt32();
                        break;
                }
            }
        }
        else if (res.Headers != null)
        {
            foreach (var header in res.Headers)
            {
                if (header.Name == HeaderNameXEResult && header.Value != null)
                {
                    EResult = (EResult)header.Value.ToString()!.ParseInt32();
                    break;
                }
            }
        }
        if (!Success)
        {
            ErrorException = res.ErrorException;
            ErrorMessage = res.ErrorMessage;
        }
        if (cookies != null)
        {
            var sb = new StringBuilder();
            foreach (Cookie cookie in cookies.GetAllCookies())
            {
                sb.Append(cookie.Name);
                sb.Append('=');
                sb.Append(cookie.Value);
                sb.Append("; ");
            }
            Cookie = sb.ToString();
        }
        CookieContainer = cookies;
    }
    public Response(HttpWebResponse res)
    {
        if (res == null)
            return;
        StatusCode = (int)res.StatusCode;
        Success = StatusCode >= 200 && StatusCode < 300;
        if (!Success && res.StatusCode == HttpStatusCode.Found)
        {
            foreach (string name in res.Headers.AllKeys)
            {
                switch (name)
                {
                    case HeaderNameLocation:
                        if (res.Headers[HeaderNameLocation] == HeaderValueLocation)
                            LostAuth = true;
                        break;
                    case HeaderNameXEResult:
                        EResult = (EResult)res.Headers[HeaderNameLocation]!.ParseInt32();
                        break;
                }
            }
        }
        else if (res.Headers != null)
        {
            foreach (string name in res.Headers.AllKeys)
            {
                if (name == HeaderNameXEResult)
                {
                    EResult = (EResult)res.Headers[HeaderNameXEResult]!.ParseInt32();
                    break;
                }
            }
        }
    }
}
