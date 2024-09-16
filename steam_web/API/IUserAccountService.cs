using System.Text.Json;
using SteamWeb.API.Models;
using SteamWeb.API.Models.IUserAccountService;
using SteamWeb.Web;

namespace SteamWeb.API;
public static class IUserAccountService
{
    public static ResponseData<ClientWalletDetails> GetClientWalletDetails(ApiRequest apiRequest,
        bool? include_balance_in_usd = null, int? wallet_region = null, bool? include_formatted_balance = null)
    {
        var request = new PostRequest(SteamPoweredUrls.IUserAccountService_GetClientWalletDetails_v1, Downloader.AppFormUrlEncoded)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.SteamWindowsClient,
        };
        apiRequest.AddAuthToken(request);
        if (include_balance_in_usd.HasValue)
            request.AddPostData("include_balance_in_usd", include_balance_in_usd.Value);
        if (wallet_region.HasValue)
            request.AddPostData("wallet_region", wallet_region.Value);
        if (include_formatted_balance.HasValue)
            request.AddPostData("include_formatted_balance", include_formatted_balance.Value);

        var response = Downloader.Post(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<ClientWalletDetails>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    public static async Task<ResponseData<ClientWalletDetails>> GetClientWalletDetailsAsync(ApiRequest apiRequest,
        bool? include_balance_in_usd = null, int? wallet_region = null, bool? include_formatted_balance = null)
    {
        var request = new PostRequest(SteamPoweredUrls.IUserAccountService_GetClientWalletDetails_v1, Downloader.AppFormUrlEncoded)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.SteamWindowsClient,
        };
        apiRequest.AddAuthToken(request);
        if (include_balance_in_usd.HasValue)
            request.AddPostData("include_balance_in_usd", include_balance_in_usd.Value);
        if (wallet_region.HasValue)
            request.AddPostData("wallet_region", wallet_region.Value);
        if (include_formatted_balance.HasValue)
            request.AddPostData("include_formatted_balance", include_formatted_balance.Value);

        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseData<ClientWalletDetails>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
}