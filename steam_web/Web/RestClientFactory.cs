using System.Net;
using RestSharp;
using System.Collections.Concurrent;
using SteamWeb.Web.Models;

namespace SteamWeb.Web;

static class RestClientFactory
{
	private static readonly ConcurrentDictionary<ClientIndexData, RestClient> _clients = new();

	/// <summary>
	/// Находит RestClient по proxy, url (domain) и user agent
	/// </summary>
	/// <returns>Возвращает RestClient</returns>
	public static RestClient GetClient(string url, IWebProxy? proxy, string ua) => GetClient(new Uri(url), proxy, ua);
	public static RestClient GetClient(Uri uri, IWebProxy? proxy, string ua)
	{
        var indexData = new ClientIndexData(uri, proxy, ua);
		RestClient? client = null;
		_clients!.AddOrUpdate(indexData, (key) =>
		{
			client = new RestClient(new RestClientOptions()
			{
				AutomaticDecompression = DecompressionMethods.All,
				FollowRedirects = false,
				UserAgent = ua,
				BaseUrl = new Uri(uri.Scheme + "://" + uri.Host),
				AllowMultipleDefaultParametersWithSameName = true,
				Proxy = proxy,
				// если скрыть, то при Redirect куки сбрасываются
				CookieContainer = null, // из-за этого происходит дублирование кук и если какие-то куки изменились, то будут изменённые и не изменённые
				ConfigureMessageHandler = h =>
				{
					((HttpClientHandler)h).UseCookies = true; // если это не установить, то Set-Cookie не работает
															  // если скрыть, то куки записываются в этот контейнер, в который мы не можем попасть
					((HttpClientHandler)h).CookieContainer = new(); // если это не установить, то куки из Set-Cookie не добавляются в наш CookieContainer
					((HttpClientHandler)h).UseProxy = proxy != null;
					((HttpClientHandler)h).Proxy = proxy;
					return h;
				}
			});
			return client!;
		},
		(key, oldValue) =>
		{
            client = oldValue;
			return oldValue;
		});
        return client!;
	}
}
