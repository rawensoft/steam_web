using SteamWeb.API.Models;
using SteamWeb.Web;
using System.Text.Json;
using SteamWeb.API.Models.IMobileNotificationService;

namespace SteamWeb.API;
public static class IMobileNotificationService
{
    public static async Task<ResponseData<UserNotificationCounts>> GetUserNotificationCounts(ApiRequest apiRequest)
    {
        var request = new GetRequest(SteamPoweredUrls.IMobileNotificationService_GetUserNotificationCounts_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp
        };
        apiRequest.AddAuthToken(request);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<UserNotificationCounts>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
}