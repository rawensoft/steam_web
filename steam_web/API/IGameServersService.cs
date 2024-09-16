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
        };
        apiRequest.AddAuthToken(request);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<AccountList>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
}