using System.Text.Json;
using SteamWeb.API.Models;
using SteamWeb.Web;
using SteamWeb.API.Models.IFriendsListService;

namespace SteamWeb.API;
public static class IFriendsListService
{
    public static ResponseData<FavoritesModel> GetFavorites(ApiRequest apiRequest)
    {
        var request = new GetRequest(SteamPoweredUrls.IFriendsListService_GetFavorites_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
        };
        apiRequest.AddAuthToken(request);
        var response = Downloader.Get(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<FavoritesModel>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    public static async Task<ResponseData<FavoritesModel>> GetFavoritesAsync(ApiRequest apiRequest)
    {
        var request = new GetRequest(SteamPoweredUrls.IFriendsListService_GetFavorites_v1)
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
            var obj = JsonSerializer.Deserialize<ResponseData<FavoritesModel>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    public static ResponseData<FriendsList> GetFriendsList(ApiRequest apiRequest)
    {
        var request = new GetRequest(SteamPoweredUrls.IFriendsListService_GetFriendsList_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
        };
        apiRequest.AddAuthToken(request);
        var response = Downloader.Get(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<FriendsList>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    public static async Task<ResponseData<FriendsList>> GetFriendsListAsync(ApiRequest apiRequest)
    {
        var request = new GetRequest(SteamPoweredUrls.IFriendsListService_GetFriendsList_v1)
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
            var obj = JsonSerializer.Deserialize<ResponseData<FriendsList>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
}