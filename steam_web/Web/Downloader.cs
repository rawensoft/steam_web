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
    public const string MobileCookie = "mobileClient=android; mobileClientVersion=777777 3.7.2; Steam_Language=english";
    public const string AppFormUrlEncoded = "application/x-www-form-urlencoded";
    public const string AppJson = "application/json";
    public const string AppOctetSteam = "application/octet-stream";
    public const string MultiPartForm = "multipart/form-data";
    private const string BuffLoginUrl = "https://buff.163.com/account/login/steam?back_url=/account/steam_bind/finish";
	public const string BASE_COMMUNITY = "https://steamcommunity.com";
	public const string BASE_POWERED = "https://store.steampowered.com/";

	public const string UserAgentSteam = "Mozilla/5.0 (Windows; U; Windows NT 10.0; en-US; Valve Steam Client/default/1607131459; ) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.117 Safari/537.36";
    public const string UserAgentChrome = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.0.0 Safari/537.36";
    public const string UserAgentMobile = "Mozilla/5.0 (Linux; Android 8.0.0; F8331) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Mobile Safari/537.36";
    public const string UserAgentSteamMobileApp = "Dalvik/2.1.0 (Linux; U; Android 5.1.1; SM-G977N Build/LMY48Z; Valve Steam App Version/3)";
    public const string UserAgentOkHttp = "okhttp/3.12.12";

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
		req.AddHeader(KnownHeaders.Accept, request.GetAccept());
		req.AddHeader(KnownHeaders.UserAgent, request.GetUserAgent());
		request.AddHeaders(req);
		request.AddQuery(req);

		if (request.Referer != null)
			req.AddHeader(SteamKnownHeaders.Referer, request.Referer);
		if (request.Cookie != null)
			req.AddHeader(KnownHeaders.Cookie, request.Cookie);
        if (request.Url.StartsWith(SteamCommunityUrls.Market_RemoveListing))
			req.AddHeader(SteamKnownHeaders.XPrototypeVersion, "1.7");

		if (request.IsAjax)
		{
			if (!request.IsMobile)
			{
				req.AddHeader(SteamKnownHeaders.AcceptLanguage, "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
				req.AddHeader(SteamKnownHeaders.Dnt, "1");
				req.AddHeader(SteamKnownHeaders.CacheControl, "no-cache");
				req.AddHeader(SteamKnownHeaders.UpgradeInsecureRequests, "1");
				req.AddHeader(SteamKnownHeaders.Pragma, "no-cache");
				req.AddHeader(SteamKnownHeaders.SecFetchDest, "empty");
				req.AddHeader(SteamKnownHeaders.SecFetchMode, "cors");
				req.AddHeader(SteamKnownHeaders.SecFetchSite, "same-origin");
			}
			req.AddHeader(SteamKnownHeaders.XRequestedWith, request.GetXRequestedWidth()!);
		}
		else if (!request.IsMobile)
		{
			req.AddHeader(SteamKnownHeaders.Dnt, "1");
			req.AddHeader(SteamKnownHeaders.UpgradeInsecureRequests, "1");
		}
		request.AddQuery(req);
		if (request.Timeout > 0)
			req.Timeout = request.Timeout;

		return (client, req);
	}
    private static (RestClient, RestRequest) GetRestClient(PostRequest request, Method method, CookieContainer? cookies)
    {
        var (client, req) = GetRestClient(request as GetRequest, method, cookies);

		req.AddHeader(KnownHeaders.ContentType, request.ContentType);
		req.AddStringBody(request.GetContent(), request.ContentType);
		if (!request.SecOpenIDNonce.IsEmpty() && req.CookieContainer != null)
			req.CookieContainer.Add(new Uri(BASE_COMMUNITY), new Cookie("sessionidSecureOpenIDNonce", request.SecOpenIDNonce)
            {
                HttpOnly = true,
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
		req.AddHeader(KnownHeaders.Accept, "application/octet-stream, application/json, text/plain, */*");
		if (!request.Cookie.IsEmpty())
			req.AddHeader(KnownHeaders.Cookie, request.Cookie!);
		if (!request.AccessToken.IsEmpty())
			req.AddQueryParameter("access_token", request.AccessToken);
		if (request.Timeout > 0)
			req.Timeout = request.Timeout;

		if (method == Method.Post)
		{
			req.AddHeader(KnownHeaders.ContentType, AppFormUrlEncoded);
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
        if (session == null)
            return new();
        var cookies = new CookieContainer();
		var uri = new Uri(url == BuffLoginUrl ? "https://steamcommunity.com" : url);
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
        if (url.StartsWith("https://steamcommunity.com"))
            return ("steamcommunity.com", "https://steamcommunity.com");
        if (url.StartsWith("https://steampowered.com"))
            return ("steampowered.com", "https://steampowered.com");
        if (url.StartsWith("https://store.steampowered.com"))
            return ("store.steampowered.com", "https://store.steampowered.com");
        if (url.StartsWith("https://api.steampowered.com"))
            return ("api.steampowered.com", "https://api.steampowered.com");
        return (null, null);
    }

    public static async Task<StringResponse> PostAsync(PostRequest request)
    {
        var cookies = AddCookieSession(request.Url, request.Session);
        var (client, req) = GetRestClient(request, Method.Post, cookies);
		var res = await (request.CancellationToken == null ? client.ExecuteAsync(req) : client.ExecuteAsync(req, request.CancellationToken.Value));
        if (request.Session != null)
            RewriteCookie(res.Cookies, request.Session, cookies, request.Url);
        int statusCode = (int)res.StatusCode;
        if (statusCode >= 300 && statusCode <= 399 && request.CurrentRedirect < request.MaxRedirects)
        {
            foreach (var header in res.Headers!)
            {
                if (header.Name == SteamKnownHeaders.Location)
                {
                    request.Url = header.Value!.ToString()!;
                    request.CurrentRedirect++;
                    return await PostAsync(request);
                }
            }
        }
        return new(res, res.Cookies);
    }
    public static StringResponse Post(PostRequest request)
    {
        var cookies = AddCookieSession(request.Url, request.Session);
		var (client, req) = GetRestClient(request, Method.Post, cookies);
		var res = request.CancellationToken == null ? client.Execute(req) : client.Execute(req, request.CancellationToken.Value);
        if (request.Session != null)
            RewriteCookie(res.Cookies, request.Session, cookies, request.Url);
        int statusCode = (int)res.StatusCode;
        if (statusCode >= 300 && statusCode <= 399 && request.CurrentRedirect < request.MaxRedirects)
        {
            foreach (var header in res.Headers!)
            {
                if (header.Name == "Location")
                {
                    request.Url = header.Value!.ToString()!;
                    request.CurrentRedirect++;
                    return Post(request);
                }
            }
        }
        return new(res, res.Cookies);
    }
    public static async Task<StringResponse> GetAsync(GetRequest request)
    {
        var cookies = AddCookieSession(request.Url, request.Session);
		var (client, req) = GetRestClient(request, Method.Get, cookies);
		var res = await (request.CancellationToken == null ? client.ExecuteAsync(req) : client.ExecuteAsync(req, request.CancellationToken.Value));
        if (request.Session != null)
            RewriteCookie(res.Cookies, request.Session, cookies, request.Url);
        int statusCode = (int)res.StatusCode;
        if (statusCode >= 300 && statusCode <= 399 && request.CurrentRedirect < request.MaxRedirects)
        {
            foreach (var header in res.Headers!)
            {
                if (header.Name == "Location")
                {
                    request.Url = header.Value!.ToString()!;
                    request.CurrentRedirect++;
                    return await GetAsync(request);
                }
            }
        }
        return new(res, res.Cookies);
    }
    public static StringResponse Get(GetRequest request)
	{
		var cookies = AddCookieSession(request.Url, request.Session);
		var (client, req) = GetRestClient(request, Method.Get, cookies);
		var res = request.CancellationToken == null ? client.Execute(req) : client.Execute(req, request.CancellationToken.Value);
		if (request.Session != null)
            RewriteCookie(res.Cookies, request.Session, cookies, request.Url);
		int statusCode = (int)res.StatusCode;
        if (statusCode >= 300 && statusCode <= 399 && request.CurrentRedirect < request.MaxRedirects)
        {
            foreach (var header in res.Headers!)
            {
                if (header.Name == "Location")
                {
                    request.Url = header.Value!.ToString()!;
                    request.CurrentRedirect++;
                    return Get(request);
                }
            }
        }
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
    public static async Task<(bool, byte[], string?)> GetCaptchaAsync(string captchagid, Proxy? proxy = null, ISessionProvider? session = null, CancellationToken? cts = null)
    {
        if (captchagid.IsEmpty())
            return (false, Encoding.UTF8.GetBytes("Не указан CaptchaGID"), null);
        string url = $"https://store.steampowered.com/login/rendercaptcha?gid={captchagid}";
        var cookie_container = new CookieContainer();
        if (session != null)
            session.AddCookieToContainer(cookie_container, new Uri(url));
        var client = new RestClient(new RestClientOptions()
        {
            BaseUrl = new Uri(url),
            UserAgent = UserAgentChrome,
            Proxy = proxy?.UseProxy == true ? proxy : null,
            //CookieContainer = cookie_container
        });
        var request = new RestRequest(string.Empty, Method.Get)
        {
            CookieContainer = cookie_container
        };
        request.AddHeader(KnownHeaders.Accept, "image/webp,image/apng,image/*,*/*;q=0.8");
        request.AddHeader(SteamKnownHeaders.Referer, "https://store.steampowered.com/join/");
		var response = await (cts == null ? client.ExecuteAsync(request) : client.ExecuteAsync(request, cts.Value));
		string new_cookie = response.Cookies.Count > 0 ? "" : null;
        for (int i = 0; i < response.Cookies.Count; i++)
        {
            var item = response.Cookies[i];
            new_cookie += $"{item.Name}={item.Value}; ";
        }
        if (!response.IsSuccessful)
        {
            if (response.Content?.Length > 0)
                return (false, Encoding.UTF8.GetBytes(response.Content), new_cookie);
            return (false, new byte[0], new_cookie);
        }
        client.Dispose();
        return (true, response.RawBytes, new_cookie);
    }
    public static (bool, byte[], string) GetCaptcha(string captchagid, Proxy? proxy = null, ISessionProvider? session = null, CancellationToken? cts = null)
    {
        if (captchagid.IsEmpty()) return (false, Encoding.UTF8.GetBytes("Не указан CaptchaGID"), null);
        string url = $"https://store.steampowered.com/login/rendercaptcha?gid={captchagid}";
        var cookie_container = new CookieContainer();
        if (session != null)
            session.AddCookieToContainer(cookie_container, new Uri(url));
        var client = new RestClient(new RestClientOptions
        {
            BaseUrl = new(url),
            UserAgent = UserAgentChrome,
            Proxy = proxy?.UseProxy == true ? proxy : null,
            //CookieContainer = cookie_container
        });
        var request = new RestRequest(string.Empty, Method.Get)
        {
            CookieContainer = cookie_container
        };
        request.AddHeader(KnownHeaders.Accept, "image/webp,image/apng,image/*,*/*;q=0.8");
        request.AddHeader(SteamKnownHeaders.Referer, "https://store.steampowered.com/join/");
		var response = cts == null ? client.Execute(request) : client.Execute(request, cts.Value);
		string new_cookie = response.Cookies.Count > 0 ? "" : null;
        for (int i = 0; i < response.Cookies.Count; i++)
        {
            var item = response.Cookies[i];
            new_cookie += $"{item.Name}={item.Value}; ";
        }
        if (!response.IsSuccessful)
        {
            if (response.Content?.Length > 0)
                return (false, Encoding.UTF8.GetBytes(response.Content), new_cookie);
            return (false, new byte[0], new_cookie);
        }
        client.Dispose();
        return (true, response.RawBytes, new_cookie);
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