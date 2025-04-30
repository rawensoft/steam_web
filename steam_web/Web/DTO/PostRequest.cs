using System.Net;
using System.Text;
using SteamWeb.Extensions;
using System.Text.RegularExpressions;
using SteamWeb.Auth.Interfaces;
using RestSharp;

namespace SteamWeb.Web.DTO;
public class PostRequest : GetRequest
{
    internal const char Ampersand = '&';

    public List<KeyValuePair<string, string>> PostData { get; private set; } = new(50);
    public string Content { get; set; } = string.Empty;
    public string ContentType { get; set; }
    /// <summary>
    /// Указывать куку при входе через steam oauth
    /// </summary>
    public string? SecOpenIDNonce { get; set; }
    /// <summary>
    /// Boundary для multipart/form-data. Обычно для steam запроса нужно указать:
    /// <code>
    /// string boudary = "----WebKitFormBoundary" + 17.GetRandomString();
    /// ...
    /// request.MultipartFormDataBoudary = boudary;
    /// </code>
    /// <para/>
    /// Если указано принудительно отправляет multipart/form-data, в других случаях отправить валидный multipart невозможно
    /// </summary>
    public string? MultipartFormDataBoudary { get; set; }

	public PostRequest(string url, string contentType) : base(url) => ContentType = contentType;
    public PostRequest(string url, string content, string contentType) : this(url, contentType) => Content = content;
    public PostRequest(string url, string content, string contentType, IWebProxy? proxy) : this(url, content, contentType) => Proxy = proxy;
    public PostRequest(string url, string content, string contentType, ISessionProvider? session) : this(url, content, contentType) => Session = session;
    public PostRequest(string url, string content, string contentType, IWebProxy? proxy, ISessionProvider? session)
        : this(url, content, contentType, proxy) => Session = session;
    public PostRequest(string url, string content, string contentType, IWebProxy? proxy, ISessionProvider? session, string? referer)
        : this(url, content, contentType, proxy, session) => Referer = referer;
    public PostRequest(string url, string content, string contentType, IWebProxy? proxy, ISessionProvider? session, string? referer, string userAgent)
        : this(url, content, contentType, proxy, session, referer) => UserAgent = userAgent;

    public string GetContent()
    {
        if (PostData.Count == 0)
            return Content;

        var sb = new StringBuilder(PostData.Count * 4 + 3);
        if (!Content.IsEmpty())
            sb.Append(Content).Append(Ampersand);
        foreach (var item in PostData)
            sb.Append(item.Key).Append('=').Append(item.Value).Append(Ampersand);
        if (sb[^1] == Ampersand)
            sb = sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }
    /// <summary>
    /// Добавляет Key-Value пары для контента 'application/x-www-form-urlencoded'
    /// </summary>
    /// <param name="name">Имя параметра</param>
    /// <param name="value">Значение параметра</param>
    /// <param name="encode">True применить кодирование:<code>System.Text.RegularExpressions.Regex.Escape(value)</code></param>
    /// <returns></returns>
    public PostRequest AddPostData(string name, string value, bool encode = true)
    {
        PostData.Add(new(name, encode ? Regex.Escape(value) : value));
        return this;
    }
    public PostRequest AddPostData(string name, byte value)
    {
        PostData.Add(new(name, value.ToString()));
        return this;
    }
    public PostRequest AddPostData(string name, short value)
    {
        PostData.Add(new(name, value.ToString()));
        return this;
    }
    public PostRequest AddPostData(string name, ushort value)
    {
        PostData.Add(new(name, value.ToString()));
        return this;
    }
    public PostRequest AddPostData(string name, int value)
    {
        PostData.Add(new(name, value.ToString()));
        return this;
    }
    public PostRequest AddPostData(string name, uint value)
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
    public PostRequest AddPostData(string name, decimal value)
    {
        PostData.Add(new(name, value.ToString()));
        return this;
    }
    public PostRequest AddPostData(string name, bool value)
    {
        PostData.Add(new(name, value.ToString()));
        return this;
    }
    internal RestRequest AddPostData(RestRequest restRequest)
    {
        foreach (var (key, value) in PostData)
            restRequest.AddParameter(key, value, false);
        return restRequest;
    }
}