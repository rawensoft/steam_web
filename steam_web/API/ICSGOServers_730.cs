﻿using System.Text.Json;
using SteamWeb.API.Models.ICSGOServers_730;
using SteamWeb.Web;
using SteamWeb.API.Models;

namespace SteamWeb.API;
public static class ICSGOServers_730
{
    /// <summary>
    /// offline, idle, low, normal, medium
    /// </summary>
    /// <returns></returns>
    public static async Task<Result<GameServerStatus>> GetGameServersStatusAsync(ApiRequest apiRequest)
    {
        var request = new GetRequest(SteamPoweredUrls.ICSGOServers_730_GetGameServersStatus_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResultData<GameServerStatus>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    /// <summary>
    /// offline, idle, low, normal, medium
    /// </summary>
    /// <returns></returns>
    public static Result<GameServerStatus> GetGameServersStatus(ApiRequest apiRequest)
    {
        var request = new GetRequest(SteamPoweredUrls.ICSGOServers_730_GetGameServersStatus_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request);
        var response = Downloader.Get(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResultData<GameServerStatus>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
}