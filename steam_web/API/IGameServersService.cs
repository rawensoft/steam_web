using System;
using System.Text.Json;
using System.Threading.Tasks;
using SteamWeb.API.Models;
using SteamWeb.API.Models.IGameServersService;
using SteamWeb.Web;

namespace SteamWeb.API;
public static class IGameServersService
{
    public static async Task<Response<AccountList>> GetAccountList(Proxy proxy, string key)
    {
        var request = new GetRequest(SteamPoweredUrls.IGameServersService_GetAccountList_v1, proxy)
        {
            UserAgent = KnownUserAgents.OkHttp
		}.AddQuery("key", key);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Response<AccountList>>(response.Data);
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
}
