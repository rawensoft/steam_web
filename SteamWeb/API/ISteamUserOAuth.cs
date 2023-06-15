using SteamWeb.API.Models.ISteamUser;
using SteamWeb.API.Models.ISteamUserOAuth;
using SteamWeb.Web;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SteamWeb.API;
public static class ISteamUserOAuth
{
    public static async Task<FriendsList<PlayerFriend>> GetFriendListAsync(Proxy proxy, string access_token)
        => await GetFriendListAsync(proxy, access_token, "");
    public static async Task<FriendsList<PlayerFriend>> GetFriendListAsync(Proxy proxy, string access_token, string relationship)
    {
        var request = new GetRequest(SteamPoweredUrls.ISteamUserOAuth_GetFriendList_v1, proxy).AddQuery("access_token", access_token);
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
    public static async Task<FriendsList<PlayerFriend>> GetFriendListAsync(Proxy proxy, string access_token, ulong steamid)
        => await GetFriendListAsync(proxy, access_token, steamid, "");
    public static async Task<FriendsList<PlayerFriend>> GetFriendListAsync(Proxy proxy, string access_token, ulong steamid, string relationship)
    {
        var request = new GetRequest(SteamPoweredUrls.ISteamUserOAuth_GetFriendList_v1, proxy).AddQuery("access_token", access_token).AddQuery("steamid", steamid);
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

    public static async Task<ResponseGroups> GetGroupListAsync(Proxy proxy, string access_token, ulong steamid)
    {
        var request = new GetRequest(SteamPoweredUrls.ISteamUserOAuth_GetGroupList_v1, proxy).AddQuery("access_token", access_token).AddQuery("steamid", steamid);
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
    public static async Task<ResponseGroups> GetGroupListAsync(Proxy proxy, string access_token)
    {
        var request = new GetRequest(SteamPoweredUrls.ISteamUserOAuth_GetGroupList_v1, proxy).AddQuery("access_token", access_token);
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

    public static async Task<TokenDetail> GetTokenDetailsAsync(Proxy proxy, string access_token)
    {
        var request = new GetRequest(SteamPoweredUrls.ISteamUserOAuth_GetTokenDetails, proxy).AddQuery("access_token", access_token);
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
