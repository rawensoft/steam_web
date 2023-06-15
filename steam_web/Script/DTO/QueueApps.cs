using System.Text.Json.Serialization;
using SteamWeb.Web;
using SteamWeb.Auth.Interfaces;

namespace SteamWeb.Script.DTO;
public class QueueApps
{
    [JsonIgnore] public bool Success { get; internal set; } = false;
    [JsonPropertyName("queue")] public uint[] Queue { get; init; } = new uint[0];

    public bool QueueApp(ISessionProvider session, Proxy proxy, uint appid_to_clear_from_queue)
    {
        var request = new PostRequest(SteamPoweredUrls.App10, Downloader.AppFormUrlEncoded)
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
        var request = new PostRequest(SteamPoweredUrls.App10, Downloader.AppFormUrlEncoded)
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
