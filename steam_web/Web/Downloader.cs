using System.Net;
using System.Text;
using SteamWeb.Extensions;
using RestSharp;
using System.Text.RegularExpressions;
using System.Web;

namespace SteamWeb.Web;
public static class Downloader
{
    private const string UrlRemoveListing = "https://steamcommunity.com/market/removelisting/";
    public const string MobileCookie = "mobileClient=android; mobileClientVersion=777777 3.0.0; Steam_Language=english";
    public const string AppFormUrlEncoded = "application/x-www-form-urlencoded";
    public const string AppJson = "application/json";
    public const string AppOctetSteam = "application/octet-stream";
    public const string MultiPartForm = "multipart/form-data";
    private const string BuffLoginUrl = "https://buff.163.com/account/login/steam?back_url=/account/steam_bind/finish";

    //public const string UserAgentSteamMobile = "Steam App/Android/2.3.13/6549178";
    public const string UserAgentSteam = "Mozilla/5.0 (Windows; U; Windows NT 10.0; en-US; Valve Steam Client/default/1607131459; ) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.117 Safari/537.36";
    public const string UserAgentChrome = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.0.0 Safari/537.36";
    public const string UserAgentMobile = "Mozilla/5.0 (Linux; Android 8.0.0; F8331) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Mobile Safari/537.36";
    public const string UserAgentSteamMobileApp = "Dalvik/2.1.0 (Linux; U; Android 5.1.1; SM-G977N Build/LMY48Z; Valve Steam App Version/3)";
    public const string UserAgentOkHttp = "okhttp/3.12.12";

    private static (RestClient, RestRequest, CookieContainer) GetRestClient(string url, Method method, IWebProxy proxy, string ua, CookieContainer? container = null)
    {
        if (container == null)
            container = new CookieContainer();
        var containerResponse = new CookieContainer();
        var client = new RestClient(new RestClientOptions() 
        {
            AutomaticDecompression = DecompressionMethods.All,
            FollowRedirects = false,
            //MaxRedirects = 10,
            UserAgent = ua,
            BaseUrl = new Uri(url),
            AllowMultipleDefaultParametersWithSameName = true,
            Proxy = proxy,
            RemoteCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; },
            // если скрыть, то при Redirect куки сбрасываются
            CookieContainer = container, // из-за этого происходит дублирование кук и если какие-то куки изменились, то будут изменённые и не изменённые
            ConfigureMessageHandler = h =>
            {
                ((HttpClientHandler)h).UseCookies = true; // если это не установить, то Set-Cookie не работает
                // если скрыть, то куки записываются в этот контейнер, в который мы не можем попасть
                ((HttpClientHandler)h).CookieContainer = containerResponse; // если это не установить, то куки из Set-Cookie не добавляются в наш CookieContainer
                ((HttpClientHandler)h).UseProxy = proxy != null;
                ((HttpClientHandler)h).Proxy = proxy;
                return h;
            }
        });
        var request = new RestRequest(string.Empty, method);
        return (client, request, containerResponse);
    }
    private static CookieContainer AddCookieSession(string url, ISessionProvider session)
    {
        if (session == null)
            return new();
        var cookie_container = new CookieContainer();
        var uri = new Uri(url == BuffLoginUrl ? "https://steamcommunity.com" : url);
        session.AddCookieToContainer(cookie_container, uri);
        return cookie_container;
    }
    private static void RewriteCookie(CookieContainer container, ISessionProvider session)
    {
        if (session == null || container == null || container.Count == 0)
            return;
        session.RewriteCookie(container);
    }
    private static void RewriteCookie(CookieContainer container, ISessionProvider session, CookieContainer containerForUpdate, string url)
    {
        if (session == null || container == null || container.Count == 0)
            return;
        session.RewriteCookie(container);
        var uri = new Uri(url);
        var cookies = container.GetCookies(uri);
        foreach (Cookie cookie in cookies)
        {
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
        if (url.StartsWith("https://steamcommunity.com")) return ("steamcommunity.com", "https://steamcommunity.com");
        if (url.StartsWith("https://steampowered.com")) return ("steampowered.com", "https://steampowered.com");
        if (url.StartsWith("https://store.steampowered.com")) return ("store.steampowered.com", "https://store.steampowered.com");
        if (url.StartsWith("https://api.steampowered.com")) return ("api.steampowered.com", "https://api.steampowered.com");
        return (null, null);
    }

    public static async Task<StringResponse> PostAsync(PostRequest request)
    {
        var cookies = AddCookieSession(request.Url, request.Session);
        var (client, req, resCookie) = GetRestClient(request.Url, Method.Post, request.Proxy, request.GetUserAgent(), cookies);
        request.AddHeaders(req);
        req.AddHeader("Accept", request.GetAccept());
        req.AddHeader("Content-Type", request.ContentType);
        req.AddBody(request.GetContent(), request.ContentType);
        if (request.Referer != null)
            req.AddHeader("Referer", request.Referer);
        if (request.Cookie != null)
            req.AddHeader("Cookie", request.Cookie);
        if (!request.SecOpenIDNonce.IsEmpty() && req.CookieContainer != null)
            req.CookieContainer.Add(new Cookie("sessionidSecureOpenIDNonce", request.SecOpenIDNonce, "/", "steamcommunity.com") { HttpOnly = true, Secure = true });
        if (request.Url.StartsWith(UrlRemoveListing))
            req.AddHeader("X-Prototype-Version", "1.7");
        if (request.IsAjax)
        {
            if (!request.IsMobile)
            {
                req.AddHeader("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
                req.AddHeader("DNT", "1");
                req.AddHeader("Cache-Control", "no-cache");
                req.AddHeader("Upgrade-Insecure-Requests", "1");
                req.AddHeader("Pragma", "no-cache");
                req.AddHeader("Sec-Fetch-Dest", "empty");
                req.AddHeader("Sec-Fetch-Mode", "cors");
                req.AddHeader("Sec-Fetch-Site", "same-origin");
            }
            req.AddHeader("X-Requested-With", request.GetXRequestedWidth());
        }
        else if (!request.IsMobile)
        {
            req.AddHeader("DNT", "1");
            req.AddHeader("Upgrade-Insecure-Requests", "1");
        }
        request.AddQuery(req);
        if (request.Timeout > 0)
            req.Timeout = request.Timeout;
        var res = await client.ExecuteAsync(req);
        client.Dispose();
        if (request.Session != null)
            RewriteCookie(resCookie, request.Session, cookies, request.Url);
        int statusCode = (int)res.StatusCode;
        if (statusCode >= 300 && statusCode <= 399 && request.CurrentRedirect < request.MaxRedirects)
        {
            foreach (var header in res.Headers!)
            {
                if (header.Name == "Location")
                {
                    request.Url = header.Value!.ToString()!;
                    request.CurrentRedirect++;
                    return await PostAsync(request);
                }
            }
        }
        return new(res, cookies);
    }
    public static StringResponse Post(PostRequest request)
    {
        var cookies = AddCookieSession(request.Url, request.Session);
        var (client, req, resCookie) = GetRestClient(request.Url, Method.Post, request.Proxy, request.GetUserAgent(), cookies);
        request.AddHeaders(req);
        req.AddHeader("Accept", request.GetAccept());
        req.AddHeader("Content-Type", request.ContentType);
        req.AddBody(request.GetContent(), request.ContentType);
        if (request.Referer != null)
            req.AddHeader("Referer", request.Referer);
        if (request.Cookie != null)
            req.AddHeader("Cookie", request.Cookie);
        if (!request.SecOpenIDNonce.IsEmpty() && req.CookieContainer != null)
            req.CookieContainer.Add(new Cookie("sessionidSecureOpenIDNonce", request.SecOpenIDNonce, "/", "steamcommunity.com") { HttpOnly = true, Secure = true });
        if (request.Url.StartsWith(UrlRemoveListing))
            req.AddHeader("X-Prototype-Version", "1.7");
        if (request.IsAjax)
        {
            if (!request.IsMobile)
            {
                req.AddHeader("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
                req.AddHeader("DNT", "1");
                req.AddHeader("Cache-Control", "no-cache");
                req.AddHeader("Upgrade-Insecure-Requests", "1");
                req.AddHeader("Pragma", "no-cache");
                req.AddHeader("Sec-Fetch-Dest", "empty");
                req.AddHeader("Sec-Fetch-Mode", "cors");
                req.AddHeader("Sec-Fetch-Site", "same-origin");
            }
            req.AddHeader("X-Requested-With", request.GetXRequestedWidth());
        }
        else if (!request.IsMobile)
        {
            req.AddHeader("DNT", "1");
            req.AddHeader("Upgrade-Insecure-Requests", "1");
        }
        request.AddQuery(req);
        if (request.Timeout > 0)
            req.Timeout = request.Timeout;
        var res = client.Execute(req);
        client.Dispose();
        if (request.Session != null)
            RewriteCookie(resCookie, request.Session, cookies, request.Url);
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
        return new(res, cookies);
    }
    public static async Task<StringResponse> GetAsync(GetRequest request)
    {
        var cookies = AddCookieSession(request.Url, request.Session);
        var (client, req, resCookie) = GetRestClient(request.Url, Method.Get, request.Proxy, request.GetUserAgent(), cookies);
        request.AddHeaders(req);
        req.AddHeader("Accept", request.GetAccept());
        if (request.Referer != null)
            req.AddHeader("Referer", request.Referer);
        if (request.Cookie != null)
            req.AddHeader("Cookie", request.Cookie);
        if (request.Url.StartsWith(UrlRemoveListing))
            req.AddHeader("X-Prototype-Version", "1.7");
        if (request.IsAjax)
        {
            if (!request.IsMobile)
            {
                req.AddHeader("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
                req.AddHeader("DNT", "1");
                req.AddHeader("Cache-Control", "no-cache");
                req.AddHeader("Upgrade-Insecure-Requests", "1");
                req.AddHeader("Pragma", "no-cache");
                req.AddHeader("Sec-Fetch-Dest", "empty");
                req.AddHeader("Sec-Fetch-Mode", "cors");
                req.AddHeader("Sec-Fetch-Site", "same-origin");
            }
            req.AddHeader("X-Requested-With", request.GetXRequestedWidth());
        }
        else if (!request.IsMobile)
        {
            req.AddHeader("DNT", "1");
            req.AddHeader("Upgrade-Insecure-Requests", "1");
        }
        request.AddQuery(req);
        if (request.Timeout > 0)
            req.Timeout = request.Timeout;
        var res = await client.ExecuteAsync(req);
        client.Dispose();
        if (request.Session != null)
            RewriteCookie(resCookie, request.Session, cookies, request.Url);
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
        return new(res, cookies);
    }
    public static StringResponse Get(GetRequest request)
    {
        var cookies = AddCookieSession(request.Url, request.Session);
        var (client, req, resCookie) = GetRestClient(request.Url, Method.Get, request.Proxy, request.GetUserAgent(), cookies);
        request.AddHeaders(req);
        req.AddHeader("Accept", request.GetAccept());
        req.AddHeader("Cache-Control", "no-cache");
        req.AddHeader("Pragma", "no-cache");
        if (request.Referer != null)
            req.AddHeader("Referer", request.Referer);
        if (request.Cookie != null)
            req.AddHeader("Cookie", request.Cookie);
        if (request.Url.StartsWith(UrlRemoveListing))
            req.AddHeader("X-Prototype-Version", "1.7");
        if (request.IsAjax)
        {
            if (!request.IsMobile)
            {
                req.AddHeader("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
                req.AddHeader("DNT", "1");
                req.AddHeader("Cache-Control", "no-cache");
                req.AddHeader("Upgrade-Insecure-Requests", "1");
                req.AddHeader("Pragma", "no-cache");
                req.AddHeader("Sec-Fetch-Dest", "empty");
                req.AddHeader("Sec-Fetch-Mode", "cors");
                req.AddHeader("Sec-Fetch-Site", "same-origin");
            }
            req.AddHeader("X-Requested-With", request.GetXRequestedWidth());
        }
        else if (!request.IsMobile)
        {
            req.AddHeader("DNT", "1");
            req.AddHeader("Upgrade-Insecure-Requests", "1");
        }
        request.AddQuery(req);
        if (request.Timeout > 0)
            req.Timeout = request.Timeout;
        var res = client.Execute(req);
        client.Dispose();
        if (request.Session != null)
            RewriteCookie(resCookie, request.Session, cookies, request.Url);
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
        return new(res, cookies);
    }

    public static async Task<StringResponse> UploadFilesToRemoteUrlAsync(PostRequest request, string filename)
    {
        var (_, origin) = GetHostOrigin(request.Url);
        string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(request.Url);
        req.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip | DecompressionMethods.Brotli;
        req.Method = "POST";
        req.Accept = "application/json, text/plain, */*";
        if (!request.Referer.IsEmpty())
            req.Referer = request.Referer;
        if (request.UserAgent == null)
            req.UserAgent = UserAgentChrome;
        else req.UserAgent = request.UserAgent;
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
    public static async Task<(bool, byte[], string?)> GetCaptchaAsync(string captchagid, Proxy? proxy = null, ISessionProvider? session = null)
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
        request.AddHeader("Accept", "image/webp,image/apng,image/*,*/*;q=0.8");
        request.AddHeader("Referer", "https://store.steampowered.com/join/");
        var response = await client.ExecuteAsync(request);
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
    public static (bool, byte[], string) GetCaptcha(string captchagid, Proxy? proxy = null, ISessionProvider? session = null)
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
        request.AddHeader("Accept", "image/webp,image/apng,image/*,*/*;q=0.8");
        request.AddHeader("Referer", "https://store.steampowered.com/join/");
        var response = client.Execute(request);
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
        var (client, req, resCookie) = GetRestClient(request.Url, Method.Get, request.Proxy, UserAgentOkHttp);
        req.AddHeader("Accept", "application/json, text/plain, */*");
        req.AddHeader("Accept-Encoding", "gzip");
        req.AddHeader("Content-Type", AppOctetSteam);
        if (!request.Cookie.IsEmpty())
            req.AddHeader("Cookie", request.Cookie);
        req.AddQueryParameter("input_protobuf_encoded", request.ProtoData);
        if (request.IsMobile)
            req.AddQueryParameter("origin", "SteamMobile");
        if (!request.AccessToken.IsEmpty())
            req.AddQueryParameter("access_token", request.AccessToken);
        if (request.Timeout > 0)
            req.Timeout = request.Timeout;
        var res = await client.ExecuteAsync(req);
        client.Dispose();
        return new(res);
    }
    public static MemoryResponse GetProtobuf(ProtobufRequest request)
    {
        var (client, req, resCookie) = GetRestClient(request.Url, Method.Get, request.Proxy, UserAgentOkHttp);
        req.AddHeader("Accept", "application/json, text/plain, */*");
        req.AddHeader("Accept-Encoding", "gzip");
        req.AddHeader("Content-Type", AppOctetSteam);
        if (!request.Cookie.IsEmpty())
            req.AddHeader("Cookie", request.Cookie);
        req.AddQueryParameter("input_protobuf_encoded", request.ProtoData);
        if (request.IsMobile)
            req.AddQueryParameter("origin", "SteamMobile");
        if (!request.AccessToken.IsEmpty())
            req.AddQueryParameter("access_token", request.AccessToken);
        if (request.Timeout > 0)
            req.Timeout = request.Timeout;
        var res = client.Execute(req);
        //client.Dispose();
        return new(res);
    }
    public static async Task<MemoryResponse> PostProtobufAsync(ProtobufRequest request)
    {
        var (client, req, resCookie) = GetRestClient(request.Url, Method.Post, request.Proxy, UserAgentOkHttp);
        req.AddHeader("Accept", "application/json, text/plain, */*");
        req.AddHeader("Accept-Encoding", "gzip");
        req.AddHeader("Content-Type", AppFormUrlEncoded);
        if (!request.Cookie.IsEmpty())
            req.AddHeader("Cookie", request.Cookie);

        var sb = new StringBuilder();
        sb.Append("input_protobuf_encoded=");
        sb.Append(HttpUtility.UrlEncode(request.ProtoData));
        if (request.IsMobile)
            sb.Append("&origin=SteamMobile");
        req.AddStringBody(sb.ToString(), DataFormat.None);

        if (!request.AccessToken.IsEmpty())
            req.AddQueryParameter("access_token", request.AccessToken);
        if (request.Timeout > 0)
            req.Timeout = request.Timeout;
        var res = await client.ExecuteAsync(req);
        client.Dispose();
        return new(res);
    }
    public static MemoryResponse PostProtobuf(ProtobufRequest request)
    {
        var (client, req, resCookie) = GetRestClient(request.Url, Method.Post, request.Proxy, UserAgentOkHttp);
        req.AddHeader("Accept", "application/json, text/plain, */*");
        req.AddHeader("Accept-Encoding", "gzip");
        req.AddHeader("Content-Type", AppFormUrlEncoded);
        if (!request.Cookie.IsEmpty())
            req.AddHeader("Cookie", request.Cookie);

        var sb = new StringBuilder();
        sb.Append("input_protobuf_encoded=");
        sb.Append(HttpUtility.UrlEncode(request.ProtoData));
        if (request.IsMobile)
            sb.Append("&origin=SteamMobile");
        req.AddStringBody(sb.ToString(), DataFormat.None);

        if (!request.AccessToken.IsEmpty())
            req.AddQueryParameter("access_token", request.AccessToken);
        if (request.Timeout > 0)
            req.Timeout = request.Timeout;
        var res = client.Execute(req);
        //var base64 = Convert.ToBase64String(res.RawBytes);
        //client.Dispose();
        return new(res);
    }

    internal static StringResponse GetMobile(GetRequest request)
    {
        var (client, req, resCookie) = GetRestClient(request.Url, Method.Post, request.Proxy, request.UserAgent, AddCookieSession(request.Url, request.Session));
        request.AddHeaders(req);
        req.AddHeader("Accept", "text/javascript, text/html, application/xml, text/xml, *");
        if (!request.Cookie.IsEmpty()) req.AddHeader("Cookie", request.Cookie);
        if (!request.Referer.IsEmpty()) req.AddHeader("Referer", request.Referer);
        req.AddHeader("X-Requested-With", "com.valvesoftware.android.steam.community");

        request.AddQuery(req);
        if (request.Timeout > 0)
            req.Timeout = request.Timeout;
        var res = client.Execute(req);
        client.Dispose();
        return new(res, resCookie);
    }
    internal static StringResponse PostMobile(PostRequest request)
    {
        var (client, req, resCookie) = GetRestClient(request.Url, Method.Post, request.Proxy, request.UserAgent, AddCookieSession(request.Url, request.Session));
        request.AddHeaders(req);
        req.AddHeader("Accept", "text/javascript, text/html, application/xml, text/xml, *");
        if (!request.Cookie.IsEmpty()) req.AddHeader("Cookie", request.Cookie);
        if (!request.Referer.IsEmpty()) req.AddHeader("Referer", request.Referer);
        req.AddHeader("X-Requested-With", "com.valvesoftware.android.steam.community");
        req.AddHeader("Content-Type", request.ContentType);
        req.AddStringBody(request.GetContent(), DataFormat.None);

        request.AddQuery(req);
        if (request.Timeout > 0)
            req.Timeout = request.Timeout;
        var res = client.Execute(req);
        client.Dispose();
        return new(res, resCookie);
    }
    internal static async Task<StringResponse> GetMobileAsync(GetRequest request)
    {
        var (client, req, resCookie) = GetRestClient(request.Url, Method.Post, request.Proxy, request.UserAgent, AddCookieSession(request.Url, request.Session));
        request.AddHeaders(req);
        req.AddHeader("Accept", "text/javascript, text/html, application/xml, text/xml, *");
        if (!request.Referer.IsEmpty()) req.AddHeader("Referer", request.Referer);
        if (!request.Cookie.IsEmpty()) req.AddHeader("Cookie", request.Cookie);
        req.AddHeader("X-Requested-With", "com.valvesoftware.android.steam.community");

        request.AddQuery(req);
        if (request.Timeout > 0)
            req.Timeout = request.Timeout;
        var res = await client.ExecuteAsync(req);
        client.Dispose();
        return new(res, resCookie);
    }
    internal static async Task<StringResponse> PostMobileAsync(PostRequest request)
    {
        var (client, req, resCookie) = GetRestClient(request.Url, Method.Post, request.Proxy, request.UserAgent, AddCookieSession(request.Url, request.Session));
        request.AddHeaders(req);
        req.AddHeader("Accept", "text/javascript, text/html, application/xml, text/xml, *");
        if (!request.Cookie.IsEmpty()) req.AddHeader("Cookie", request.Cookie);
        if (!request.Referer.IsEmpty()) req.AddHeader("Referer", request.Referer);
        req.AddHeader("X-Requested-With", "com.valvesoftware.android.steam.community");
        req.AddHeader("Content-Type", request.ContentType);
        req.AddStringBody(request.GetContent(), DataFormat.None);
        request.AddQuery(req);

        if (request.Timeout > 0)
            req.Timeout = request.Timeout;
        var res = await client.ExecuteAsync(req);
        client.Dispose();
        return new(res, resCookie);
    }

    private static StringResponse GetMobileApp(GetRequest request)
    {
        var (client, req, resCookie) = GetRestClient(request.Url, Method.Post, request.Proxy, request.UserAgent, AddCookieSession(request.Url, request.Session));
        request.AddHeaders(req);
        req.AddHeader("accept", "text/javascript, text/html, application/xml, text/xml, *");
        req.AddHeader("Accept-Encoding", "gzip");
        req.AddHeader("Cookie", "mobileClient=android; mobileClientVersion=777777 3.0.0; Steam_Language=english");
        req.AddHeader("X-Requested-With", "com.valvesoftware.android.steam.community");
        if (!request.Referer.IsEmpty())
            req.AddHeader("Referer", request.Referer);
        if (request.Timeout > 0)
            req.Timeout = request.Timeout;
        var res = client.Execute(req);
        client.Dispose();
        return new(res, resCookie);
    }
    private static async Task<StringResponse> GetMobileAppAsync(GetRequest request)
    {
        var (client, req, resCookie) = GetRestClient(request.Url, Method.Post, request.Proxy, request.UserAgent, AddCookieSession(request.Url, request.Session));
        request.AddHeaders(req);
        req.AddHeader("accept", "text/javascript, text/html, application/xml, text/xml, *");
        req.AddHeader("Accept-Encoding", "gzip");
        req.AddHeader("Cookie", "mobileClient=android; mobileClientVersion=777777 3.0.0; Steam_Language=english");
        req.AddHeader("X-Requested-With", "com.valvesoftware.android.steam.community");
        if (!request.Referer.IsEmpty())
            req.AddHeader("Referer", request.Referer);
        if (request.Timeout > 0)
            req.Timeout = request.Timeout;
        var res = await client.ExecuteAsync(req);
        client.Dispose();
        return new(res, resCookie);
    }
}
public class ProtobufRequest
{
    public string Url { get; init; }
    public string AccessToken { get; init; }
    public string ProtoData { get; init; }
    public IWebProxy Proxy { get; init; }
    public ISessionProvider Session { get; init; }
    public string UserAgent { get; init; }
    public string Cookie { get; init; }
    public bool IsMobile { get; init; } = true;
    public int Timeout { get; set; } = 30000;

    public ProtobufRequest(string url, string protoData)
    {
        Url = url;
        //ProtoData = System.Web.HttpUtility.UrlEncode(protoData);
        ProtoData = protoData;
    }
}


public class GetRequest
{
    public int CurrentRedirect { get; set; } = 0;
    public CookieContainer? CookieContainer { get; set; } = null;
    public int MaxRedirects { get; set; } = 10;
    public string Url { get; set; }
    public List<KeyValuePair<string, string>> QueryParametrs { get; private set; } = new(50);
    public List<KeyValuePair<string, string>> Headers { get; private set; } = new(50);
    public IWebProxy Proxy { get; set; }
    public ISessionProvider Session { get; set; }
    public string UserAgent { get; set; }
    public string Referer { get; set; }
    public string Cookie { get; set; }
    public bool UseVersion2 { get; set; } = false;
    public bool IsAjax { get; set; } = false;
    public bool IsMobile { get; set; } = false;
    public int Timeout { get; set; } = 30000;

    public GetRequest(string url) => Url = url;
    public GetRequest(string url, IWebProxy proxy) : this(url) => Proxy = proxy;
    public GetRequest(string url, ISessionProvider session) : this(url) => Session = session;
    public GetRequest(string url, IWebProxy proxy, ISessionProvider session) : this(url, proxy) => Session = session;
    public GetRequest(string url, IWebProxy proxy, ISessionProvider session, string referer) : this(url, proxy, session) => Referer = referer;
    public GetRequest(string url, IWebProxy proxy, ISessionProvider session, string referer, string userAgent) : this(url, proxy, session, referer) => UserAgent = userAgent;

    public GetRequest AddHeader(string name, string value)
    {
        Headers.Add(new(name, value));
        return this;
    }
    public GetRequest AddHeaders(RestRequest request)
    {
        foreach (var header in Headers)
            request.AddHeader(header.Key, header.Value);
        return this;
    }

    public GetRequest AddQuery(string name, string value)
    {
        QueryParametrs.Add(new (name, HttpUtility.UrlEncode(value)));
        return this;
    }
    public GetRequest AddQuery(string name, int value)
    {
        QueryParametrs.Add(new(name, value.ToString()));
        return this;
    }
    public GetRequest AddQuery(string name, uint value)
    {
        QueryParametrs.Add(new(name, value.ToString()));
        return this;
    }
    public GetRequest AddQuery(string name, long value)
    {
        QueryParametrs.Add(new(name, value.ToString()));
        return this;
    }
    public GetRequest AddQuery(string name, ulong value)
    {
        QueryParametrs.Add(new(name, value.ToString()));
        return this;
    }
    public void AddQuery(RestRequest request)
    {
        foreach (var query in QueryParametrs)
            request.AddQueryParameter(query.Key, query.Value, false);
    }
    public string GetUserAgent()
    {
        if (UserAgent != null)
            return UserAgent;

        if (UseVersion2)
            return Downloader.UserAgentOkHttp;
        else
            return Downloader.UserAgentChrome;
    }
    public string GetAccept()
    {
        if (IsMobile && IsAjax)
            return "text/javascript, text/html, application/xml, text/xml, *";
        if (UseVersion2)
            return "application/json, text/plain, */*";
        return "*/*";
    }
    public string GetXRequestedWidth()
    {
        if (IsMobile && IsAjax)
            return "com.valvesoftware.android.steam.community";
        else if (IsAjax)
            return "XMLHttpRequest";
        return null;
    }
}
public class PostRequest: GetRequest
{
    internal const char Ampersand = '&';

    public List<KeyValuePair<string, string>> PostData { get; private set; } = new(50);
    public string Content { get; set; } = "";
    public string ContentType { get; set; }
    public string SecOpenIDNonce { get; set; }

    public PostRequest(string url, string contentType) : base(url) => ContentType = contentType;
    public PostRequest(string url, string content, string contentType): this(url, contentType) => Content = content;
    public PostRequest(string url, string content, string contentType, IWebProxy proxy) : this(url, content, contentType) => Proxy = proxy;
    public PostRequest(string url, string content, string contentType, ISessionProvider session) : this(url, content, contentType) => Session = session;
    public PostRequest(string url, string content, string contentType, IWebProxy proxy, ISessionProvider session)
        :this(url, content, contentType, proxy) => Session = session;
    public PostRequest(string url, string content, string contentType, IWebProxy proxy, ISessionProvider session, string referer)
        : this(url, content, contentType, proxy, session) => Referer = referer;
    public PostRequest(string url, string content, string contentType, IWebProxy proxy, ISessionProvider session, string referer, string userAgent)
        : this(url, content, contentType, proxy, session, referer) => UserAgent = userAgent;

    public string GetContent()
    {
        if (PostData.Count == 0)
            return Content;

        var sb = new StringBuilder();
        if (!Content.IsEmpty())
            sb.Append(Content).Append(Ampersand);
        foreach (var item in PostData)
            sb.Append(item.Key).Append('=').Append(item.Value).Append(Ampersand);
        if (sb[^1] == Ampersand)
            sb = sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }
    public PostRequest AddPostData(string name, string value, bool encode = true)
    {
        PostData.Add(new(name, encode ? Regex.Escape(value) : value));
        return this;
    }
    public PostRequest AddPostData(string name, int value)
    {
        PostData.Add(new(name, value.ToString()));
        return this;
    }
    public PostRequest AddPostData(string name, long value)
    {
        PostData.Add(new(name, value.ToString()));
        return this;
    }
    public PostRequest AddPostData(string name, ulong value)
    {
        PostData.Add(new(name, value.ToString()));
        return this;
    }
    public PostRequest AddPostData(string name, float value)
    {
        PostData.Add(new(name, value.ToString()));
        return this;
    }
    public PostRequest AddPostData(string name, double value)
    {
        PostData.Add(new(name, value.ToString()));
        return this;
    }
}

public class StringResponse : Response
{
    public string Data { get; init; } = null;
    public StringResponse(RestResponse res): base(res) => Data = res.Content;
    public StringResponse(RestResponse res, CookieContainer cookies) : base(res, cookies) => Data = res.Content;
    public StringResponse(HttpWebResponse res) : base(res)
    {
        if (res != null && res.ContentLength > 0)
        {
            using var sr = new StreamReader(res.GetResponseStream());
            Data = sr.ReadToEnd();
        }
    }
}
public class MemoryResponse : Response, IDisposable
{
    // Track whether Dispose has been called.
    private bool disposed = false;

    public MemoryStream Stream { get; init; } = null;

    public MemoryResponse(RestResponse res) : base(res) => Stream = res.RawBytes == null || res.RawBytes.Length == 0 || !res.IsSuccessful ? null : new MemoryStream(res.RawBytes);

    // Implement IDisposable.
    // Do not make this method virtual.
    // A derived class should not be able to override this method.
    public void Dispose()
    {
        Dispose(disposing: true);
        // This object will be cleaned up by the Dispose method.
        // Therefore, you should call GC.SuppressFinalize to
        // take this object off the finalization queue
        // and prevent finalization code for this object
        // from executing a second time.
        GC.SuppressFinalize(this);
    }
    // Dispose(bool disposing) executes in two distinct scenarios.
    // If disposing equals true, the method has been called directly
    // or indirectly by a user's code. Managed and unmanaged resources
    // can be disposed.
    // If disposing equals false, the method has been called by the
    // runtime from inside the finalizer and you should not reference
    // other objects. Only unmanaged resources can be disposed.
    protected virtual void Dispose(bool disposing)
    {
        // Check to see if Dispose has already been called.
        if (!disposed)
        {
            // If disposing equals true, dispose all managed
            // and unmanaged resources.
            if (disposing)
            {
                // Dispose managed resources.
                if (Stream != null)
                {
                    Stream.Dispose();
                }
            }

            // Note disposing has been done.
            disposed = true;
        }
    }
}
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
            foreach (var header in res.Headers)
            {
                switch (header.Name)
                {
                    case HeaderNameLocation:
                        if (header.Value.ToString() == HeaderValueLocation)
                            LostAuth = true;
                        break;
                    case HeaderNameXEResult:
                        if (header.Value != null)
                            EResult = (EResult)header.Value.ToString().ParseInt32();
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
                    EResult = (EResult)header.Value.ToString().ParseInt32();
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
                        EResult = (EResult)res.Headers[HeaderNameLocation].ParseInt32();
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
                    EResult = (EResult)res.Headers[HeaderNameXEResult].ParseInt32();
                    break;
                }
            }
        }
    }
}
