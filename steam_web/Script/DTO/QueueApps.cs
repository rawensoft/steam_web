using System.Text.Json.Serialization;
using SteamWeb.Web;
using SteamWeb.Auth.Interfaces;
using System.Net;

namespace SteamWeb.Script.DTO;
public class QueueApps
{
    [JsonIgnore] public bool Success { get; internal set; } = false;
    [JsonPropertyName("queue")] public uint[] Queue { get; init; } = new uint[0];

    public bool NextApp(ISessionProvider session, Proxy proxy)
    {
        var container = new CookieContainer();
        container.Add(new("https://store.steampowered.com/explore/"), new Cookie("queue_type", "0"));
        session.AddCookieToContainer(new(), new("https://store.steampowered.com/explore/"));
        var request = new PostRequest(SteamPoweredUrls.Explore_Next, Downloader.AppFormUrlEncoded)
        {
            Proxy = proxy,
            Session = session,
            Referer = SteamPoweredUrls.App10,
            IsAjax = true
        };
        var response = Downloader.Post(request);
        return response.Success;
    }
    public bool QueueApp(ISessionProvider session, Proxy proxy, uint appid_to_clear_from_queue)
    {
        var request = new PostRequest(SteamPoweredUrls.Explore_Next_Zero, Downloader.AppFormUrlEncoded)
        {
            Proxy = proxy,
            Session = session,
            Referer = SteamPoweredUrls.App10,
            IsAjax = true
        };
        request.AddPostData("appid_to_clear_from_queue", appid_to_clear_from_queue).AddPostData("sessionid", session.SessionID);
        var response = Downloader.Post(request);
        return response.Success;
    }

    public bool QueueApp(ISessionProvider session, Proxy proxy, uint appid, uint appid_to_clear_from_queue)
    {
        var request = new PostRequest(SteamPoweredUrls.App + $"/{appid}", Downloader.AppFormUrlEncoded)
        {
            Proxy = proxy,
            Session = session,
            Referer = SteamPoweredUrls.Explore,
            IsAjax = true
        };
        request.AddPostData("appid_to_clear_from_queue", appid_to_clear_from_queue).AddPostData("sessionid", session.SessionID);
        var response = Downloader.Post(request);
        return response.Success;
    }
    public async Task<bool> QueueAppAsync(ISessionProvider session, Proxy proxy, uint appid_to_clear_from_queue)
    {
        var request = new PostRequest(SteamPoweredUrls.App + $"/{appid_to_clear_from_queue}", Downloader.AppFormUrlEncoded)
        {
            Proxy = proxy,
            Session = session,
            Referer = SteamPoweredUrls.Explore,
            IsAjax = true
        };
        request.AddPostData("appid_to_clear_from_queue", appid_to_clear_from_queue).AddPostData("sessionid", session.SessionID);
        var response = await Downloader.PostAsync(request);
        return response.Success;
    }
}
