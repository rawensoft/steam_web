using System.Net;
using System.Text;
using RestSharp;

namespace SteamWeb.Web.DTO;

public class ByteResponse : Response
{
	public byte[]? Data { get; init; } = null;
	public ByteResponse(RestResponse res) : base(res) => Data = res.RawBytes;
	public ByteResponse(RestResponse res, CookieContainer? cookies) : base(res, cookies) => Data = res.RawBytes;
	public ByteResponse(RestResponse res, CookieCollection? cookies) : base(res, cookies) => Data = res.RawBytes;
	public ByteResponse(HttpWebResponse res) : base(res)
	{
		if (res != null && res.ContentLength > 0)
		{
			using var sr = new StreamReader(res.GetResponseStream());
			Data = Encoding.UTF8.GetBytes(sr.ReadToEnd());
		}
	}
}