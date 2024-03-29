﻿using SteamWeb.API.Models;
using SteamWeb.Web;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using SteamWeb.API.Models.IFriendMessagesService;
using Response = SteamWeb.API.Models.Response;

namespace SteamWeb.API;
public static class IFriendMessagesService
{
    public static async Task<Response<ActiveMessageSessions>> GetActiveMessageSessions(Proxy proxy, string access_token)
    {
        var request = new GetRequest(SteamPoweredUrls.IFriendMessagesService_GetActiveMessageSessions_v1, proxy)
        { UserAgent = KnownUserAgents.OkHttp }.AddQuery("access_token", access_token);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Response<ActiveMessageSessions>>(response.Data);
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    public static async Task<Response<RecentMessages>> GetRecentMessages(Proxy proxy, string access_token, ulong steamid1, ulong steamid2, int rtime32_start_time = 0)
    {
        var request = new GetRequest(SteamPoweredUrls.IFriendMessagesService_GetRecentMessages_v1, proxy)
        { UserAgent = KnownUserAgents.OkHttp }.AddQuery("access_token", access_token).AddQuery("steamid1", steamid1)
        .AddQuery("steamid2", steamid2).AddQuery("rtime32_start_time", rtime32_start_time);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Response<RecentMessages>>(response.Data);
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    public static async Task<Response> MarkOfflineMessagesRead(Proxy proxy, string access_token, ulong steamid_friend)
    {
        var request = new PostRequest(SteamPoweredUrls.IFriendMessagesService_MarkOfflineMessagesRead_v1, Downloader.AppFormUrlEncoded)
        {
            UserAgent = KnownUserAgents.OkHttp,
            Proxy = proxy
        };
        request.AddQuery("access_token", access_token).AddQuery("steamid_friend", steamid_friend);
        var response = await Downloader.PostAsync(request);
        return new() { success = response.Success };
    }
}
