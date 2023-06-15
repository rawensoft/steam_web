using SteamWeb.API.Models;
using SteamWeb.Web;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using SteamWeb.API.Models.ILoyaltyRewardsService;

namespace SteamWeb.API;
public static class ILoyaltyRewardsService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Proxy"></param>
    /// <param name="access_token">Можно получить через <code>(await Ajax.pointssummary_ajaxgetasyncconfig(Session, Proxy)).Item2.WebAPI_Token;</code></param>
    /// <param name="steamid">Обязательно тот, к которому относится access_token</param>
    /// <returns></returns>
    public static async Task<Response<RewardSummary>> GetSummaryAsync(Proxy proxy, string access_token, ulong steamid)
    {
        var request = new GetRequest(SteamPoweredUrls.ILoyaltyRewardsService_GetSummary_v1, proxy)
        {
            UserAgent = Downloader.UserAgentOkHttp
        }
        .AddQuery("access_token", access_token).AddQuery("steamid", steamid);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Response<RewardSummary>>(response.Data);
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Proxy"></param>
    /// <param name="access_token">Можно получить через <code>(await Ajax.pointssummary_ajaxgetasyncconfig(Session, Proxy)).Item2.WebAPI_Token;</code></param>
    /// <param name="steamid">Обязательно тот, к которому относится access_token</param>
    /// <returns></returns>
    public static Response<RewardSummary> GetSummary(Proxy proxy, string access_token, ulong steamid)
    {
        var request = new GetRequest(SteamPoweredUrls.ILoyaltyRewardsService_GetSummary_v1, proxy)
        {
            UserAgent = Downloader.UserAgentOkHttp
        }
        .AddQuery("access_token", access_token).AddQuery("steamid", steamid);
        var response = Downloader.Get(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Response<RewardSummary>>(response.Data);
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
}
