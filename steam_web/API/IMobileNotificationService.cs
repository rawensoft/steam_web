using SteamWeb.API.Models;
using SteamWeb.Web;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using SteamWeb.API.Models.IMobileNotificationService;

namespace SteamWeb.API;
public static class IMobileNotificationService
{
    public static async Task<Response<UserNotificationCounts>> GetUserNotificationCounts(Proxy proxy, string access_token)
    {
        var request = new GetRequest(SteamPoweredUrls.IMobileNotificationService_GetUserNotificationCounts_v1, proxy)
        {
            UserAgent = Downloader.UserAgentOkHttp
        }
        .AddQuery("access_token", access_token);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Response<UserNotificationCounts>>(response.Data);
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
}
