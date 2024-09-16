using SteamWeb.API.Models;
using SteamWeb.Web;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using SteamWeb.API.Models.IFriendMessagesService;
using Response = SteamWeb.API.Models.Response;

namespace SteamWeb.API;
public static class IFriendMessagesService
{
    public static async Task<Response<ActiveMessageSessions>> GetActiveMessageSessions(ApiRequest apiRequest)
    {
        var request = new GetRequest(SteamPoweredUrls.IFriendMessagesService_GetActiveMessageSessions_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
        };
        apiRequest.AddAuthToken(request);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<ActiveMessageSessions>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    public static async Task<Response<RecentMessages>> GetRecentMessages(ApiRequest apiRequest, ulong steamid1, ulong steamid2, int rtime32_start_time = 0)
    {
        var request = new GetRequest(SteamPoweredUrls.IFriendMessagesService_GetRecentMessages_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
        };
        apiRequest.AddAuthToken(request).AddQuery("steamid1", steamid1)
            .AddQuery("steamid2", steamid2).AddQuery("rtime32_start_time", rtime32_start_time);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<RecentMessages>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    public static async Task<Response> MarkOfflineMessagesRead(ApiRequest apiRequest, ulong steamid_friend)
    {
        var request = new PostRequest(SteamPoweredUrls.IFriendMessagesService_MarkOfflineMessagesRead_v1, Downloader.AppFormUrlEncoded)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
        };
        apiRequest.AddAuthToken(request).AddQuery("steamid_friend", steamid_friend);
        var response = await Downloader.PostAsync(request);
        return new() { success = response.Success };
    }
}