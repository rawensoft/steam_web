using System.Net;
using System.Text;
using SteamWeb.Extensions;
using RestSharp;
using System.Net.Sockets;

namespace SteamWeb.Web.DTO;
public class Response
{
    private const string HeaderValueLocation = "steammobile://lostauth/";
    private const string HeaderNameXEResult = "X-eresult";

    public CookieContainer? CookieContainer { get; set; } = null;
    public bool Success { get; init; } = false;
    public string? Cookie { get; private set; } = null;
    public int StatusCode { get; init; } = 0;
    public Exception? ErrorException { get; init; } = null;
    public string? ErrorMessage { get; init; } = null;
    public bool LostAuth { get; init; } = false;
    public EResult EResult { get; init; } = EResult.Invalid;
	public SocketError SocketErrorCode { get; private set; } = SocketError.Success;
	/// <summary>
	/// Не null, если есть Location заголовок в ответе
	/// </summary>
	public string? Location { get; set; }

	public Response(RestResponse res)
	{
		Success = res.IsSuccessful;
		StatusCode = (int)res.StatusCode;
		if (!Success && res.StatusCode == HttpStatusCode.Found)
		{
			foreach (var header in res.Headers!)
			{
				switch (header.Name)
				{
					case KnownHeaders.Location:
						var location = header.Value!.ToString();
						if (location == HeaderValueLocation)
							LostAuth = true;
						else
							Location = location;
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
			if (res.ErrorException?.InnerException != null)
			{
				ErrorException = res.ErrorException.InnerException;
				ErrorMessage = res.ErrorException.InnerException.Message;
				var socketEx = ErrorException as SocketException;
				if (socketEx != null)
					SocketErrorCode = socketEx.SocketErrorCode;
			}
			else
			{
				ErrorException = res.ErrorException;
				ErrorMessage = res.ErrorMessage;
			}
		}
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
                    case KnownHeaders.Location:
                        var location = res.Headers[KnownHeaders.Location];
                        if (location == HeaderValueLocation)
                            LostAuth = true;
                        else
                            Location = location;
						break;

					case HeaderNameXEResult:
						EResult = (EResult)res.Headers[KnownHeaders.Location]!.ParseInt32();
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
	public Response(RestResponse res, CookieContainer? cookies) : this(res) => WriteCookies(cookies);
	public Response(RestResponse res, CookieCollection? cookies) : this(res) => WriteCookies(cookies);

	private void WriteCookies(CookieContainer? cookies)
	{
		if (cookies != null)
		{
			var allCookies = cookies.GetAllCookies();
			var sb = new StringBuilder(allCookies.Count * 4 + 2);
			foreach (Cookie cookie in allCookies)
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
	private void WriteCookies(CookieCollection? cookies)
	{
		if (cookies != null)
		{
			var container = new CookieContainer();
			var sb = new StringBuilder(cookies.Count * 4 + 2);
			foreach (Cookie cookie in cookies)
			{
				sb.Append(cookie.Name);
				sb.Append('=');
				sb.Append(cookie.Value);
				sb.Append("; ");
				if (cookie.Domain.StartsWith('.'))
					container.Add(cookie);
				else
					container.Add(new Uri("https://" + cookie.Domain), cookie);
			}
			Cookie = sb.ToString();
			CookieContainer = container;
		}
	}
}
