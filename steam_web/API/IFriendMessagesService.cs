using SteamWeb.API.Models;
using SteamWeb.Web;
using System.Text.Json;
using SteamWeb.API.Models.IFriendMessagesService;
using ResponseData = SteamWeb.API.Models.ResponseData;

namespace SteamWeb.API;
public static class IFriendMessagesService
{
    public static ResponseData<ActiveMessageSessions> GetActiveMessageSessions(ApiRequest apiRequest, int? lastmessage_since = null, bool? only_sessions_with_messages = null)
    {
        var request = new GetRequest(SteamPoweredUrls.IFriendMessagesService_GetActiveMessageSessions_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
        };
        apiRequest.AddAuthToken(request);
        if (lastmessage_since.HasValue)
            request.AddQuery("lastmessage_since", lastmessage_since.Value);
        if (only_sessions_with_messages.HasValue)
            request.AddQuery("only_sessions_with_messages", only_sessions_with_messages.Value);
        var response = Downloader.Get(request);
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
    public static async Task<ResponseData<ActiveMessageSessions>> GetActiveMessageSessionsAsync(ApiRequest apiRequest, int? lastmessage_since = null, bool? only_sessions_with_messages = null)
    {
        var request = new GetRequest(SteamPoweredUrls.IFriendMessagesService_GetActiveMessageSessions_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
        };
        apiRequest.AddAuthToken(request);
        if (lastmessage_since.HasValue)
            request.AddQuery("lastmessage_since", lastmessage_since.Value);
        if (only_sessions_with_messages.HasValue)
            request.AddQuery("only_sessions_with_messages", only_sessions_with_messages.Value);
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

    public static ResponseData<RecentMessages> GetRecentMessages(ApiRequest apiRequest, RecentMessageRequest recentMessageRequest)
    {
        var request = new GetRequest(SteamPoweredUrls.IFriendMessagesService_GetRecentMessages_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
        };
        apiRequest.AddAuthToken(request).AddQuery("steamid1", recentMessageRequest.SteamId1)
            .AddQuery("steamid2", recentMessageRequest.SteamId2);

        if (recentMessageRequest.RecentTime32StartTime.HasValue)
            request.AddQuery("rtime32_start_time", recentMessageRequest.RecentTime32StartTime.Value);

        if (recentMessageRequest.Count.HasValue)
            request.AddQuery("count", recentMessageRequest.Count.Value);

        if (recentMessageRequest.MostRecentConversation)
            request.AddQuery("most_recent_conversation", recentMessageRequest.MostRecentConversation);

        if (recentMessageRequest.BbCodeFormat)
            request.AddQuery("bbcode_format", recentMessageRequest.BbCodeFormat);

        if (recentMessageRequest.StartOrdinal.HasValue)
            request.AddQuery("start_ordinal", recentMessageRequest.StartOrdinal.Value);

        if (recentMessageRequest.TimeLast.HasValue)
            request.AddQuery("time_last", recentMessageRequest.TimeLast.Value);

        if (recentMessageRequest.OrdinalLast.HasValue)
            request.AddQuery("ordinal_last", recentMessageRequest.OrdinalLast.Value);

        var response = Downloader.Get(request);
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
    public static async Task<ResponseData<RecentMessages>> GetRecentMessagesAsync(ApiRequest apiRequest, RecentMessageRequest recentMessageRequest)
    {
        var request = new GetRequest(SteamPoweredUrls.IFriendMessagesService_GetRecentMessages_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
        };
        apiRequest.AddAuthToken(request).AddQuery("steamid1", recentMessageRequest.SteamId1)
            .AddQuery("steamid2", recentMessageRequest.SteamId2);

        if (recentMessageRequest.RecentTime32StartTime.HasValue)
            request.AddQuery("rtime32_start_time", recentMessageRequest.RecentTime32StartTime.Value);

        if (recentMessageRequest.Count.HasValue)
            request.AddQuery("count", recentMessageRequest.Count.Value);

        if (recentMessageRequest.MostRecentConversation)
            request.AddQuery("most_recent_conversation", recentMessageRequest.MostRecentConversation);

        if (recentMessageRequest.BbCodeFormat)
            request.AddQuery("bbcode_format", recentMessageRequest.BbCodeFormat);

        if (recentMessageRequest.StartOrdinal.HasValue)
            request.AddQuery("start_ordinal", recentMessageRequest.StartOrdinal.Value);

        if (recentMessageRequest.TimeLast.HasValue)
            request.AddQuery("time_last", recentMessageRequest.TimeLast.Value);

        if (recentMessageRequest.OrdinalLast.HasValue)
            request.AddQuery("ordinal_last", recentMessageRequest.OrdinalLast.Value);

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

    public static ResponseData MarkOfflineMessagesRead(ApiRequest apiRequest, ulong steamid_friend)
    {
        var request = new PostRequest(SteamPoweredUrls.IFriendMessagesService_MarkOfflineMessagesRead_v1, Downloader.AppFormUrlEncoded)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
        };
        apiRequest.AddAuthToken(request).AddPostData("steamid_friend", steamid_friend);
        var response = Downloader.Post(request);
        return new() { Success = response.Success };
    }
    public static async Task<ResponseData> MarkOfflineMessagesReadAsync(ApiRequest apiRequest, ulong steamid_friend)
    {
        var request = new PostRequest(SteamPoweredUrls.IFriendMessagesService_MarkOfflineMessagesRead_v1, Downloader.AppFormUrlEncoded)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
        };
        apiRequest.AddAuthToken(request).AddPostData("steamid_friend", steamid_friend);
        var response = await Downloader.PostAsync(request);
        return new() { Success = response.Success };
    }
}