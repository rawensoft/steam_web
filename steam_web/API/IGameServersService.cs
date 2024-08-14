using System.Text.Json;
using SteamWeb.API.Models;
using SteamWeb.API.Models.IGameServersService;
using SteamWeb.Web;

namespace SteamWeb.API;
public static class IGameServersService
{
    public static async Task<Response<AccountList>> GetAccountList(ApiRequest apiRequest)
    {
        var request = new GetRequest(SteamPoweredUrls.IGameServersService_GetAccountList_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
        }.AddQuery("key", apiRequest.AuthToken!);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Response<AccountList>>(response.Data!);
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
}
