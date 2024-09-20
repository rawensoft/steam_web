using SteamWeb.API.Models;
using SteamWeb.API.Models.ITwoFactorService;
using SteamWeb.Web;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SteamWeb.API;
public static class ITwoFactorService
{
    public static async Task<ResponseData<QueryStatus>> QueryStatus(ApiRequest apiRequest, ulong steamid)
    {
        var request = new PostRequest(SteamApiUrls.ITwoFactorService_QueryStatus_v1, Downloader.AppFormUrlEncoded)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
        };
        apiRequest.AddAuthToken(request).AddPostData("steamid", steamid);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<QueryStatus>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
}