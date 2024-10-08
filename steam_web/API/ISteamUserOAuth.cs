﻿using SteamWeb.API.Models;
using SteamWeb.API.Models.ISteamUser;
using SteamWeb.API.Models.ISteamUserOAuth;
using SteamWeb.Web;
using System.Text.Json;

namespace SteamWeb.API;
public static class ISteamUserOAuth
{
    public static async Task<FriendsList<PlayerFriend>> GetFriendListAsync(ApiRequest apiRequest)
        => await GetFriendListAsync(apiRequest, string.Empty);
    public static async Task<FriendsList<PlayerFriend>> GetFriendListAsync(ApiRequest apiRequest, string relationship)
    {
        var request = new GetRequest(SteamApiUrls.ISteamUserOAuth_GetFriendList_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request);
        if (!string.IsNullOrEmpty(relationship))
            request.AddQuery("relationship", relationship);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<FriendsList<PlayerFriend>>(response.Data!, Steam.JsonOptions)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    public static async Task<FriendsList<PlayerFriend>> GetFriendListAsync(ApiRequest apiRequest, ulong steamid)
        => await GetFriendListAsync(apiRequest, steamid, string.Empty);
    public static async Task<FriendsList<PlayerFriend>> GetFriendListAsync(ApiRequest apiRequest, ulong steamid, string relationship)
    {
        var request = new GetRequest(SteamApiUrls.ISteamUserOAuth_GetFriendList_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request).AddQuery("steamid", steamid);
        if (!string.IsNullOrEmpty(relationship))
            request.AddQuery("relationship", relationship);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<FriendsList<PlayerFriend>>(response.Data!, Steam.JsonOptions)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    public static async Task<ResponseGroups> GetGroupListAsync(ApiRequest apiRequest, ulong steamid)
    {
        var request = new GetRequest(SteamApiUrls.ISteamUserOAuth_GetGroupList_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request).AddQuery("steamid", steamid);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseGroups>(response.Data!, Steam.JsonOptions)!;
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    public static async Task<ResponseGroups> GetGroupListAsync(ApiRequest apiRequest)
    {
        var request = new GetRequest(SteamApiUrls.ISteamUserOAuth_GetGroupList_v1)
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
            var obj = JsonSerializer.Deserialize<ResponseGroups>(response.Data!, Steam.JsonOptions)!;
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    public static async Task<TokenDetail> GetTokenDetailsAsync(ApiRequest apiRequest)
    {
        var request = new GetRequest(SteamApiUrls.ISteamUserOAuth_GetTokenDetails)
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
            var obj = JsonSerializer.Deserialize<TokenDetail>(response.Data!, Steam.JsonOptions)!;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
}