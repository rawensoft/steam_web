using System.Text.Json;
using SteamWeb.API.Models;
using SteamWeb.API.Models.ISteamApps;
using SteamWeb.Web;

namespace SteamWeb.API;
public static class ISteamApps
{
    public static ResponseData<UpToDateModel> UpToDateCheck(ApiRequest apiRequest, uint appid, uint version)
    {
        var request = new PostRequest(SteamApiUrls.ISteamApps_UpToDateCheck_v1, Downloader.AppFormUrlEncoded)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request).AddPostData("appid", appid).AddPostData("version", version);
        var response = Downloader.Post(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<UpToDateModel>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    public static async Task<ResponseData<UpToDateModel>> UpToDateCheckAsync(ApiRequest apiRequest, uint appid, uint version)
    {
        var request = new PostRequest(SteamApiUrls.ISteamApps_UpToDateCheck_v1, Downloader.AppFormUrlEncoded)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request).AddPostData("appid", appid).AddPostData("version", version);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<UpToDateModel>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
}