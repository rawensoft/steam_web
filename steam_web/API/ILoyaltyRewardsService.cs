using SteamWeb.API.Models;
using SteamWeb.Web;
using System.Text.Json;
using SteamWeb.API.Models.ILoyaltyRewardsService;

namespace SteamWeb.API;
public static class ILoyaltyRewardsService
{
    public static ResponseData<RewardSummary> GetSummary(ApiRequest apiRequest, ulong steamid)
    {
        var request = new GetRequest(SteamPoweredUrls.ILoyaltyRewardsService_GetSummary_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp
        };
        apiRequest.AddAuthToken(request).AddQuery("steamid", steamid);
        var response = Downloader.Get(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<RewardSummary>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    public static async Task<ResponseData<RewardSummary>> GetSummaryAsync(ApiRequest apiRequest, ulong steamid)
    {
        var request = new GetRequest(SteamPoweredUrls.ILoyaltyRewardsService_GetSummary_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp
		};
        apiRequest.AddAuthToken(request).AddQuery("steamid", steamid);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<RewardSummary>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
}