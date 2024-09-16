using System.Text.Json;
using SteamWeb.API.Models;
using SteamWeb.Web;
using SteamWeb.API.Models.ISteamEconomy;

namespace SteamWeb.API;
public static class ISteamEconomy
{
    /// <summary>
    /// Находит информацию по classid и\без instanceid
    /// </summary>
    /// <param name="apiRequest"></param>
    /// <param name="appid">Must be a steam economy app.</param>
    /// <param name="classid0">Class ID of the nth class.</param>
    /// <param name="class_count">Number of classes requested. Must be at least one.</param>
    /// <param name="instanceid0">Instance ID of the nth class.</param>
    /// <returns>Информация о предмете</returns>
    public static ResultData<Dictionary<string, AssetClassInfo>> GetAssetClassInfo(ApiRequest apiRequest, uint appid, uint classid0, byte class_count = 1, uint? instanceid0 = null)
    {
        var request = new PostRequest(SteamPoweredUrls.ISteamEconomy_GetAssetClassInfo_v1, Downloader.AppFormUrlEncoded)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request).AddPostData("appid", appid).AddPostData("classid0", classid0).AddPostData("class_count", class_count);
        if (instanceid0.HasValue)
            request.AddPostData("instanceid0", instanceid0.Value);
        var response = Downloader.Post(request);
        if (!response.Success)
            return new();
        string data = response.Data!;
        var trueIndex = data.IndexOf("true");
        if (trueIndex != -1)
        {
            data = data.Remove(trueIndex, 4);
            data = data.Insert(trueIndex, "{}");
        }
        var falseIndex = data.IndexOf("false");
        if (falseIndex != -1)
        {
            data = data.Remove(falseIndex, 5);
            data = data.Insert(falseIndex, "{}");

            /// TODO:
            /// проверить удаление ошибки
            /// скорее всего будет проблема с IndexOf и Remove
            var startErrorIndex = data.IndexOf("error: \"");
            if (startErrorIndex != -1)
            {
                startErrorIndex += 7;
                var endErrorIndex = data.IndexOf("\",", startErrorIndex);
                if (endErrorIndex != -1)
                {
                    data = data.Remove(startErrorIndex, endErrorIndex - startErrorIndex + 1);
                    data = data.Insert(startErrorIndex, "{}");
                }
            }
        }
        try
        {
            var obj = JsonSerializer.Deserialize<ResultData<Dictionary<string, AssetClassInfo>>>(data, Steam.JsonOptions)!;
            obj.Success = trueIndex != -1;
            obj.Result?.Remove("success");
            if (!obj.Success)
                obj.Result?.Remove("error");
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    /// <summary>
    /// Находит информацию по classid и\без instanceid
    /// </summary>
    /// <param name="apiRequest"></param>
    /// <param name="appid">Must be a steam economy app.</param>
    /// <param name="classid0">Class ID of the nth class.</param>
    /// <param name="class_count">Number of classes requested. Must be at least one.</param>
    /// <param name="instanceid0">Instance ID of the nth class.</param>
    /// <returns>Информация о предмете</returns>
    public static async Task<ResultData<Dictionary<string, AssetClassInfo>>> GetAssetClassInfoAsync(ApiRequest apiRequest, uint appid, uint classid0, byte class_count = 1, uint? instanceid0 = null)
    {
        var request = new PostRequest(SteamPoweredUrls.ISteamEconomy_GetAssetClassInfo_v1, Downloader.AppFormUrlEncoded)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request).AddPostData("appid", appid).AddPostData("classid0", classid0).AddPostData("class_count", class_count);
        if (instanceid0.HasValue)
            request.AddPostData("instanceid0", instanceid0.Value);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        string data = response.Data!;
        var trueIndex = data.IndexOf("true");
        if (trueIndex != -1)
        {
            data = data.Remove(trueIndex, 4);
            data = data.Insert(trueIndex, "{}");
        }
        var falseIndex = data.IndexOf("false");
        if (falseIndex != -1)
        {
            data = data.Remove(falseIndex, 5);
            data = data.Insert(falseIndex, "{}");

            /// TODO:
            /// проверить удаление ошибки
            /// скорее всего будет проблема с IndexOf и Remove
            var startErrorIndex = data.IndexOf("error: \"");
            if (startErrorIndex != -1)
            {
                startErrorIndex += 7;
                var endErrorIndex = data.IndexOf("\",", startErrorIndex);
                if (endErrorIndex != -1)
                {
                    data = data.Remove(startErrorIndex, endErrorIndex - startErrorIndex + 1);
                    data = data.Insert(startErrorIndex, "{}");
                }
            }
        }
        try
        {
            var obj = JsonSerializer.Deserialize<ResultData<Dictionary<string, AssetClassInfo>>>(data, Steam.JsonOptions)!;
            obj.Success = trueIndex != -1;
            obj.Result?.Remove("success");
            if (!obj.Success)
                obj.Result?.Remove("error");
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    public static ResultData<AssetPrices> GetAssetPrices(ApiRequest apiRequest, uint appid, string currency = "")
    {
        var request = new PostRequest(SteamPoweredUrls.ISteamEconomy_GetAssetPrices_v1, Downloader.AppFormUrlEncoded)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request).AddPostData("appid", appid);
        if (currency != string.Empty)
            request.AddPostData("currency", currency);
        var response = Downloader.Post(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResultData<AssetPrices>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    public static async Task<ResultData<AssetPrices>> GetAssetPricesAsync(ApiRequest apiRequest, uint appid, string currency = "")
    {
        var request = new PostRequest(SteamPoweredUrls.ISteamEconomy_GetAssetPrices_v1, Downloader.AppFormUrlEncoded)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request).AddPostData("appid", appid);
        if (currency != string.Empty)
            request.AddPostData("currency", currency);
        var response = await Downloader.PostAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResultData<AssetPrices>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
}