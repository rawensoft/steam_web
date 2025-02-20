using ProtoBuf;
using SteamWeb.API.Models;
using SteamWeb.API.Protobufs;
using SteamWeb.Web;

namespace SteamWeb.API;
public static class IMobileAppService
{
    public static CMobileAppService_GetMobileSummary_Response? GetMobileSummary(ApiRequest apiRequest)
    {
        var request = new ProtobufRequest(SteamApiUrls.IMobileAppService_GetMobileSummary_v1, string.Empty)
        {
            AccessToken = apiRequest.AuthToken,
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
            SpoofSteamId = 0,
        };
        using var response = Downloader.PostProtobuf(request);
        if (!response.Success)
            return null;
        if (response.EResult != EResult.OK)
            return null;

        try
        {
            var obj = Serializer.Deserialize<CMobileAppService_GetMobileSummary_Response>(response.Stream!);
            return obj;
        }
        catch (Exception)
        {
            return null;
        }
    }
    public static CMobileAppService_GetMobileSummary_Response? GetMobileSummary(ApiRequest apiRequest, CMobileAppService_GetMobileSummary_Request proto)
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, proto);
        var base64 = Convert.ToBase64String(ms.ToArray());

        var request = new ProtobufRequest(SteamApiUrls.IMobileAppService_GetMobileSummary_v1, base64)
        {
            AccessToken = apiRequest.AuthToken,
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
            SpoofSteamId = 0,
        };
        using var response = Downloader.PostProtobuf(request);
        if (!response.Success)
            return null;
        if (response.EResult != EResult.OK)
            return null;

        try
        {
            var obj = Serializer.Deserialize<CMobileAppService_GetMobileSummary_Response>(response.Stream!);
            return obj;
        }
        catch (Exception)
        {
            return null;
        }
    }
    public static async Task<CMobileAppService_GetMobileSummary_Response?> GetMobileSummaryAsync(ApiRequest apiRequest)
    {
        var request = new ProtobufRequest(SteamApiUrls.IMobileAppService_GetMobileSummary_v1, string.Empty)
        {
            AccessToken = apiRequest.AuthToken,
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
            SpoofSteamId = 0,
        };
        using var response = await Downloader.PostProtobufAsync(request);
        if (!response.Success)
            return null;
        if (response.EResult != EResult.OK)
            return null;

        try
        {
            var obj = Serializer.Deserialize<CMobileAppService_GetMobileSummary_Response>(response.Stream!);
            return obj;
        }
        catch (Exception)
        {
            return null;
        }
    }
    public static async Task<CMobileAppService_GetMobileSummary_Response?> GetMobileSummaryAsync(ApiRequest apiRequest, CMobileAppService_GetMobileSummary_Request proto)
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, proto);
        var base64 = Convert.ToBase64String(ms.ToArray());

        var request = new ProtobufRequest(SteamApiUrls.IMobileAppService_GetMobileSummary_v1, base64)
        {
            AccessToken = apiRequest.AuthToken,
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
            SpoofSteamId = 0,
        };
        using var response = await Downloader.PostProtobufAsync(request);
        if (!response.Success)
            return null;
        if (response.EResult != EResult.OK)
            return null;

        try
        {
            var obj = Serializer.Deserialize<CMobileAppService_GetMobileSummary_Response>(response.Stream!);
            return obj;
        }
        catch (Exception)
        {
            return null;
        }
    }
}