using SteamWeb.API.Models;
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
        var request = new GetRequest(SteamPoweredUrls.ISteamUserOAuth_GetFriendList_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        }
            .AddQuery("access_token", apiRequest.AuthToken!);
        if (!string.IsNullOrEmpty(relationship))
            request.AddQuery("relationship", relationship);
        var response = await Downloader.GetAsync(request);
        if (!response.Success) return new();
        try
        {
            var obj = JsonSerializer.Deserialize<FriendsList<PlayerFriend>>(response.Data);
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
        var request = new GetRequest(SteamPoweredUrls.ISteamUserOAuth_GetFriendList_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        }
            .AddQuery("access_token", apiRequest.AuthToken!).AddQuery("steamid", steamid);
        if (!string.IsNullOrEmpty(relationship))
            request.AddQuery("relationship", relationship);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<FriendsList<PlayerFriend>>(response.Data);
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    public static async Task<ResponseGroups> GetGroupListAsync(ApiRequest apiRequest, ulong steamid)
    {
        var request = new GetRequest(SteamPoweredUrls.ISteamUserOAuth_GetGroupList_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        }
            .AddQuery("access_token", apiRequest.AuthToken!).AddQuery("steamid", steamid);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseGroups>(response.Data);
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
        var request = new GetRequest(SteamPoweredUrls.ISteamUserOAuth_GetGroupList_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        }
            .AddQuery("access_token", apiRequest.AuthToken!);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseGroups>(response.Data);
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
        var request = new GetRequest(SteamPoweredUrls.ISteamUserOAuth_GetTokenDetails)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        }
            .AddQuery("access_token", apiRequest.AuthToken!);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<TokenDetail>(response.Data);
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
}
