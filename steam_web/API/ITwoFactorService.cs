﻿using SteamWeb.API.Models;
using SteamWeb.API.Models.ITwoFactorService;
using SteamWeb.Web;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SteamWeb.API;
public static class ITwoFactorService
{
    public static async Task<Response<QueryStatus>> QueryStatus(Proxy? proxy, string access_token, ulong steamid)
    {
        var request = new PostRequest(SteamPoweredUrls.ITwoFactorService_QueryStatus_v1, Downloader.AppFormUrlEncoded)
        {
            Proxy = proxy,
            UserAgent = KnownUserAgents.OkHttp
		}
        .AddPostData("access_token", access_token).AddPostData("steamid", steamid);
        var response = await Downloader.PostAsync(request);
        if (!response.Success) return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Response<QueryStatus>>(response.Data!);
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
}
