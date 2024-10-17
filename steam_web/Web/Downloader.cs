global using SteamWeb.Web.DTO;
using System.Net;
using System.Text;
using SteamWeb.Extensions;
using RestSharp;
using System.Web;
using SteamWeb.Auth.Interfaces;
#if !FACTORY
using System.Collections.Concurrent;
#endif

namespace SteamWeb.Web;
public static class Downloader
{
    public const string AppFormUrlEncoded = "application/x-www-form-urlencoded";
    public const string AppJson = "application/json";
    public const string AppOctetSteam = "application/octet-stream";
    public const string MultiPartForm = "multipart/form-data";
    
#if !FACTORY
    private static readonly ConcurrentDictionary<int, RestClient> _clients = new(4, 1000);

	private static RestClient GetRestClient(IWebProxy? proxy)
	{
		var client = new RestClient(new RestClientOptions()
		{
			AutomaticDecompression = DecompressionMethods.All,
			FollowRedirects = false,
			UserAgent = null,
			AllowMultipleDefaultParametersWithSameName = true,
			Proxy = proxy,
			// если скрыть, то при Redirect куки сбрасываются
			CookieContainer = null, // из-за этого происходит дублирование кук и если какие-то куки изменились, то будут изменённые и не изменённые
			ConfigureMessageHandler = h =>
			{
				((HttpClientHandler)h).UseCookies = false; // если это не установить, то Set-Cookie не работает
														   // если скрыть, то куки записываются в этот контейнер, в который мы не можем попасть
				((HttpClientHandler)h).CookieContainer = new(); // если это не установить, то куки из Set-Cookie не добавляются в наш CookieContainer
				((HttpClientHandler)h).UseProxy = proxy != null;
				((HttpClientHandler)h).Proxy = proxy;
				return h;
			}
		});
		return client;
	}
#endif
    private static (RestClient, RestRequest) GetRestClient(GetRequest request, Method method, CookieContainer? cookies)
    {
        var uri = new Uri(request.Url);
#if FACTORY
		var client = RestClientFactory.Factory.GetClient(request.Proxy, uri);
#else
		var client = GetClient(request.Proxy, uri);
#endif
		var req = new RestRequest(request.Url, method)
        {
            CookieContainer = cookies
		};
		req.AddHeader(RestSharp.KnownHeaders.Accept, request.GetAccept());
		req.AddHeader(RestSharp.KnownHeaders.UserAgent, request.GetUserAgent());
		request.AddHeaders(req);
		request.AddQuery(req);

		if (request.Referer != null)
			req.AddHeader(KnownHeaders.Referer, request.Referer);
		if (request.Cookie != null)
			req.AddHeader(RestSharp.KnownHeaders.Cookie, request.Cookie);
        if (request.Url.StartsWith(SteamCommunityUrls.Market_RemoveListing))
			req.AddHeader(KnownHeaders.XPrototypeVersion, "1.7");

		if (request.IsAjax)
		{
			if (!request.IsMobile)
			{
				req.AddHeader(KnownHeaders.AcceptLanguage, "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
				req.AddHeader(KnownHeaders.Dnt, "1");
				req.AddHeader(KnownHeaders.CacheControl, "no-cache");
				req.AddHeader(KnownHeaders.UpgradeInsecureRequests, "1");
				req.AddHeader(KnownHeaders.Pragma, "no-cache");
				req.AddHeader(KnownHeaders.SecFetchDest, "empty");
				req.AddHeader(KnownHeaders.SecFetchMode, "cors");
				req.AddHeader(KnownHeaders.SecFetchSite, "same-origin");
			}
			req.AddHeader(KnownHeaders.XRequestedWith, request.GetXRequestedWidth()!);
		}
		else if (!request.IsMobile)
		{
			req.AddHeader(KnownHeaders.Dnt, "1");
			req.AddHeader(KnownHeaders.UpgradeInsecureRequests, "1");
		}
		if (request.Timeout > 0)
			req.Timeout = request.Timeout;

		return (client, req);
	}
    private static (RestClient, RestRequest) GetRestClient(PostRequest request, Method method, CookieContainer? cookies)
    {
        var (client, req) = GetRestClient(request as GetRequest, method, cookies);

		req.AddHeader(RestSharp.KnownHeaders.ContentType, request.ContentType);
		req.AddStringBody(request.GetContent(), request.ContentType);
		if (!request.SecOpenIDNonce.IsEmpty() && req.CookieContainer != null)
			req.CookieContainer.Add(new Uri(KnownUri.BASE_COMMUNITY), new Cookie("sessionidSecureOpenIDNonce", request.SecOpenIDNonce)
            {
                HttpOnly = true,
                Secure = true
            });
		if (!request.SteamRefresh_Steam.IsEmpty() && req.CookieContainer != null)
			req.CookieContainer.Add(new Uri(KnownUri.BASE_LOGIN_POWERED), new Cookie("steamRefresh_steam", request.SteamRefresh_Steam)
			{
				HttpOnly = false,
				Secure = true
			});

		return (client, req);
	}
	private static (RestClient, RestRequest) GetRestClient(ProtobufRequest request, Method method)
	{
		var uri = new Uri(request.Url);
#if FACTORY
		var client = RestClientFactory.Factory.GetClient(request.Proxy, uri);
#else
		var client = GetClient(request.Proxy, uri);
#endif
		var req = new RestRequest(request.Url, method)
		{
			CookieContainer = new()
		};
		req.AddHeader(RestSharp.KnownHeaders.Accept, "application/octet-stream, application/json, text/plain, */*");
		if (!request.Cookie.IsEmpty())
			req.AddHeader(RestSharp.KnownHeaders.Cookie, request.Cookie!);
		if (!request.AccessToken.IsEmpty())
			req.AddQueryParameter("access_token", request.AccessToken);
		if (request.Timeout > 0)
			req.Timeout = request.Timeout;

		if (method == Method.Post)
		{
			req.AddHeader(RestSharp.KnownHeaders.ContentType, AppFormUrlEncoded);
			var sb = new StringBuilder();
			sb.Append("input_protobuf_encoded=");
			sb.Append(HttpUtility.UrlEncode(request.ProtoData));
			if (request.IsMobile)
				sb.Append("&origin=SteamMobile");
			req.AddStringBody(sb.ToString(), DataFormat.None);
		}
		else if (method == Method.Get)
        {
			req.AddQueryParameter("input_protobuf_encoded", request.ProtoData);
			if (request.IsMobile)
				req.AddQueryParameter("origin", "SteamMobile");
		}

		return (client, req);
	}
#if !FACTORY
	private static RestClient GetClient(IWebProxy? proxy, Uri uri)
	{
		var hash = proxy?.GetProxy(uri)?.GetHashCode() ?? -1;
		RestClient? client = null;
		_clients!.AddOrUpdate(hash, (key) =>
		{
			client = GetRestClient(proxy);
			return client!;
		},
		(key, oldValue) =>
		{
			client = oldValue;
			return oldValue;
		});
		return client!;
	}
#endif


	private static CookieContainer AddCookieSession(string url, ISessionProvider? session)
	{
		var cookies = new CookieContainer();
		if (session == null)
            return cookies;
		var uri = new Uri(url == KnownUri.BuffLoginUrl ? KnownUri.BASE_COMMUNITY : url);
        session.AddCookieToContainer(cookies, uri);
        return cookies;
	}
	private static void RewriteCookie(CookieCollection? collection, ISessionProvider? session, CookieContainer containerForUpdate, string url)
	{
		if (session == null || collection == null || collection.Count == 0)
			return;
		session.RewriteCookie(collection);
		var uri = new Uri(url);
		foreach (Cookie cookie in collection)
		{
			if (cookie.Domain == uri.Host)
			    containerForUpdate.Add(uri, cookie);
		}
	}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="url"></param>
	/// <returns>Host, Origin</returns>
	private static (string?, string?) GetHostOrigin(string url)
    {
        if (url.StartsWith(KnownUri.BASE_COMMUNITY))
            return ("steamcommunity.com", KnownUri.BASE_COMMUNITY);
        if (url.StartsWith("https://steampowered.com"))
            return ("steampowered.com", "https://steampowered.com");
        if (url.StartsWith(KnownUri.BASE_POWERED))
            return ("store.steampowered.com", KnownUri.BASE_POWERED);
        if (url.StartsWith("https://api.steampowered.com"))
            return ("api.steampowered.com", "https://api.steampowered.com");
        return (null, null);
    }
    private static bool IsRedirect(RestResponse response, GetRequest request)
    {
		int statusCode = (int)response.StatusCode;
		if (statusCode >= 300 && statusCode <= 399 && request.CurrentRedirect < request.MaxRedirects)
		{
			foreach (var header in response.Headers!)
			{
				if (header.Name == KnownHeaders.Location)
				{
					request.Url = header.Value!.ToString()!;
					request.CurrentRedirect++;
					return true;
				}
			}
		}
        return false;
	}


    public static async Task<StringResponse> PostAsync(PostRequest request)
    {
        var cookies = AddCookieSession(request.Url, request.Session);
        var (client, req) = GetRestClient(request, Method.Post, cookies);
		var res = await (request.CancellationToken == null ? client.ExecuteAsync(req) : client.ExecuteAsync(req, request.CancellationToken.Value));
        if (request.Session != null)
            RewriteCookie(res.Cookies, request.Session, cookies, request.Url);
        if (IsRedirect(res, request))
			return await PostAsync(request);
		return new(res, res.Cookies);
    }
    public static StringResponse Post(PostRequest request)
    {
        var cookies = AddCookieSession(request.Url, request.Session);
		var (client, req) = GetRestClient(request, Method.Post, cookies);
		var res = request.CancellationToken == null ? client.Execute(req) : client.Execute(req, request.CancellationToken.Value);
        if (request.Session != null)
            RewriteCookie(res.Cookies, request.Session, cookies, request.Url);
		if (IsRedirect(res, request))
			return Post(request);
		return new(res, res.Cookies);
    }
    public static async Task<StringResponse> GetAsync(GetRequest request)
    {
        var cookies = AddCookieSession(request.Url, request.Session);
		var (client, req) = GetRestClient(request, Method.Get, cookies);
		var res = await (request.CancellationToken == null ? client.ExecuteAsync(req) : client.ExecuteAsync(req, request.CancellationToken.Value));
        if (request.Session != null)
            RewriteCookie(res.Cookies, request.Session, cookies, request.Url);
		if (IsRedirect(res, request))
			return await GetAsync(request);
		return new(res, res.Cookies);
    }
    public static StringResponse Get(GetRequest request)
	{
		var cookies = AddCookieSession(request.Url, request.Session);
		var (client, req) = GetRestClient(request, Method.Get, cookies);
		var res = request.CancellationToken == null ? client.Execute(req) : client.Execute(req, request.CancellationToken.Value);
		if (request.Session != null)
            RewriteCookie(res.Cookies, request.Session, cookies, request.Url);
		if (IsRedirect(res, request))
			return Get(request);
		return new(res, res.Cookies);
    }

    public static async Task<StringResponse> UploadFilesToRemoteUrlAsync(PostRequest request, string filename)
    {
        var (_, origin) = GetHostOrigin(request.Url);
        string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(request.Url);
		req.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip | DecompressionMethods.Brotli;
        req.Method = "POST";
        req.Accept = "application/json, text/plain, */*";
		req.UserAgent = request.GetUserAgent();
		if (!request.Referer.IsEmpty())
            req.Referer = request.Referer;
		if (request.Proxy != null)
            req.Proxy = request.Proxy;

        var cookie_container = new CookieContainer();
        if (request.Session != null)
            request.Session.AddCookieToContainer(cookie_container, new Uri(request.Url));
        req.CookieContainer = cookie_container;

        req.ContentType = "multipart/form-data; boundary=" +
                                boundary;
        req.KeepAlive = true;

        using var memStream = new MemoryStream();
        var boundarybytes = Encoding.ASCII.GetBytes("\r\n--" +
                                                                boundary + "\r\n");
        var endBoundaryBytes = Encoding.ASCII.GetBytes("\r\n--" +
                                                                    boundary + "--");
        string formdataTemplate = "\r\n--" + boundary +
                                    "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";

        foreach (var item in request.PostData)
        {
            string formitem = string.Format(formdataTemplate, item.Key, item.Value);
            byte[] formitembytes = Encoding.UTF8.GetBytes(formitem);
            memStream.Write(formitembytes, 0, formitembytes.Length);
        }

        string headerTemplate =
            "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
            "Content-Type: application/octet-stream\r\n\r\n";

        memStream.Write(boundarybytes, 0, boundarybytes.Length);
        var header = string.Format(headerTemplate, "avatar", "blob");
        var headerbytes = Encoding.UTF8.GetBytes(header);

        memStream.Write(headerbytes, 0, headerbytes.Length);

        using var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
        var buffer = new byte[1024];
        var bytesRead = 0;
        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
        {
            memStream.Write(buffer, 0, bytesRead);
        }

        memStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
        req.ContentLength = memStream.Length;

        using var requestStream = req.GetRequestStream();
        memStream.Position = 0;
        byte[] tempBuffer = new byte[memStream.Length];
        memStream.Read(tempBuffer, 0, tempBuffer.Length);
        memStream.Close();
        requestStream.Write(tempBuffer, 0, tempBuffer.Length);

        try
        {
            var response = (HttpWebResponse)req.GetResponse();
            using var sr = new StreamReader(response.GetResponseStream());
            string data = sr.ReadToEnd();
            string cookie = null;
            var cookies = cookie_container.GetCookies(new Uri(origin ?? "https://steampowered.com/"));
            if (cookies.Count > 0) cookie = "";
            foreach (Cookie item in cookies) cookie += $"{item.Name}={item.Value}; ";
            return new(response);
        }
        catch (WebException ex)
        {
            if (ex.Message.Contains("302"))
            {
                request.Url = ex.Response.Headers["Location"];
                return await UploadFilesToRemoteUrlAsync(request, filename);
            }
            string data = null;
            if (ex.Message.Contains("429")) data = "429";
            return new((HttpWebResponse)ex.Response) { ErrorException = ex, ErrorMessage = ex.Message };
        }
        catch (Exception ex)
        {
            return new((HttpWebResponse)null) { ErrorException = ex, ErrorMessage = ex.Message };
        }
    }
    public static async Task<(bool, byte[], string?)> GetCaptchaAsync(string captchagid, IWebProxy? proxy = null, ISessionProvider? session = null, CancellationToken? cts = null)
    {
        if (captchagid.IsEmpty())
            return (false, Encoding.UTF8.GetBytes("Не указан CaptchaGID"), string.Empty);
        string url = "https://store.steampowered.com/login/rendercaptcha?gid=" + captchagid;
        var cookie_container = new CookieContainer();
        if (session != null)
            session.AddCookieToContainer(cookie_container, new Uri(url));
        var client = new RestClient(new RestClientOptions()
        {
            BaseUrl = new Uri(url),
            UserAgent = KnownUserAgents.WindowsBrowser,
            Proxy = proxy,
        });
        var request = new RestRequest(string.Empty, Method.Get)
        {
            CookieContainer = cookie_container
        };
        request.AddHeader(RestSharp.KnownHeaders.Accept, "image/webp,image/apng,image/*,*/*;q=0.8");
        request.AddHeader(KnownHeaders.Referer, "https://store.steampowered.com/join/");
		var response = await (cts == null ? client.ExecuteAsync(request) : client.ExecuteAsync(request, cts.Value));
        var sb = new StringBuilder(11);
        for (int i = 0; i < response.Cookies?.Count; i++)
        {
            var item = response.Cookies[i];
            sb.Append(item.Name);
            sb.Append('=');
            sb.Append(item.Value);
            sb.Append("; ");
        }
        if (!response.IsSuccessful)
        {
            if (response.Content?.Length > 0)
                return (false, Encoding.UTF8.GetBytes(response.Content), sb.ToString());
            return (false, Array.Empty<byte>(), sb.ToString());
        }
        client.Dispose();
        return (true, response.RawBytes ?? Array.Empty<byte>(), sb.ToString());
    }
    public static (bool, byte[], string) GetCaptcha(string captchagid, IWebProxy? proxy = null, ISessionProvider? session = null, CancellationToken? cts = null)
    {
        if (captchagid.IsEmpty()) return (false, Encoding.UTF8.GetBytes("Не указан CaptchaGID"), string.Empty);
        string url = "https://store.steampowered.com/login/rendercaptcha?gid=" + captchagid;
        var cookie_container = new CookieContainer();
        if (session != null)
            session.AddCookieToContainer(cookie_container, new Uri(url));
        var client = new RestClient(new RestClientOptions
        {
            BaseUrl = new(url),
            UserAgent = KnownUserAgents.WindowsBrowser,
            Proxy = proxy,
        });
        var request = new RestRequest(string.Empty, Method.Get)
        {
            CookieContainer = cookie_container
        };
        request.AddHeader(RestSharp.KnownHeaders.Accept, "image/webp,image/apng,image/*,*/*;q=0.8");
        request.AddHeader(KnownHeaders.Referer, "https://store.steampowered.com/join/");
		var response = cts == null ? client.Execute(request) : client.Execute(request, cts.Value);
        var sb = new StringBuilder(11);
        for (int i = 0; i < response.Cookies?.Count; i++)
        {
            var item = response.Cookies[i];
            sb.Append(item.Name);
            sb.Append('=');
            sb.Append(item.Value);
            sb.Append("; ");
        }
        if (!response.IsSuccessful)
        {
            if (response.Content?.Length > 0)
                return (false, Encoding.UTF8.GetBytes(response.Content), sb.ToString());
            return (false, Array.Empty<byte>(), sb.ToString());
        }
        client.Dispose();
        return (true, response.RawBytes ?? Array.Empty<byte>(), sb.ToString());
    }

    public static async Task<MemoryResponse> GetProtobufAsync(ProtobufRequest request)
    {
        var (client, req) = GetRestClient(request, Method.Get);
		var res = await (request.CancellationToken == null ? client.ExecuteAsync(req) : client.ExecuteAsync(req, request.CancellationToken.Value));
		var obj = new MemoryResponse(res);
		return obj;
    }
    public static MemoryResponse GetProtobuf(ProtobufRequest request)
	{
		var (client, req) = GetRestClient(request, Method.Get);
		var res = request.CancellationToken == null ? client.Execute(req) : client.Execute(req, request.CancellationToken.Value);
		var obj = new MemoryResponse(res);
		return obj;
	}
    public static async Task<MemoryResponse> PostProtobufAsync(ProtobufRequest request)
    {
        var (client, req) = GetRestClient(request, Method.Post);
		var res = await (request.CancellationToken == null ? client.ExecuteAsync(req) : client.ExecuteAsync(req, request.CancellationToken.Value));
		var obj = new MemoryResponse(res);
		return obj;
	}
    public static MemoryResponse PostProtobuf(ProtobufRequest request)
	{
		var (client, req) = GetRestClient(request, Method.Post);
		var res = request.CancellationToken == null ? client.Execute(req) : client.Execute(req, request.CancellationToken.Value);
		var obj = new MemoryResponse(res);
		return obj;
	}
}