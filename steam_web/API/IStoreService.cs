using ProtoBuf;
using SteamWeb.API.Models;
using SteamWeb.API.Protobufs;
using SteamWeb.Web;

namespace SteamWeb.API;
public static class IStoreService
{
    public static EResult SkipDiscoveryQueueItem(ApiRequest apiRequest, CStore_SkipDiscoveryQueueItem_Request proto)
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, proto);
        var base64 = Convert.ToBase64String(ms.ToArray());

        var request = new ProtobufRequest(SteamApiUrls.IStoreService_SkipDiscoveryQueueItem_v1, base64)
        {
            AccessToken = apiRequest.AuthToken,
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
            SpoofSteamId = 0,
        };
        using var response = Downloader.PostProtobuf(request);
        if (response.EResult != EResult.OK)
            return response.EResult;
        if (!response.Success)
            return EResult.Invalid;
        return EResult.OK;
    }
    public static async Task<EResult> SkipDiscoveryQueueItemAsync(ApiRequest apiRequest, CStore_SkipDiscoveryQueueItem_Request proto)
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, proto);
        var base64 = Convert.ToBase64String(ms.ToArray());

        var request = new ProtobufRequest(SteamApiUrls.IStoreService_SkipDiscoveryQueueItem_v1, base64)
        {
            AccessToken = apiRequest.AuthToken,
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
            SpoofSteamId = 0,
        };
        using var response = await Downloader.PostProtobufAsync(request);
        if (response.EResult != EResult.OK)
            return response.EResult;
        if (!response.Success)
            return EResult.Invalid;
        return EResult.OK;
    }

    public static CStore_GetDiscoveryQueue_Response? GetDiscoveryQueue(ApiRequest apiRequest, CStore_GetDiscoveryQueue_Request proto)
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, proto);
        var base64 = Convert.ToBase64String(ms.ToArray());

        var request = new ProtobufRequest(SteamApiUrls.IStoreService_GetDiscoveryQueue_v1, base64)
        {
            AccessToken = apiRequest.AuthToken,
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
            SpoofSteamId = 0,
        };
        using var response = Downloader.GetProtobuf(request);
        if (response.EResult != EResult.OK)
            return null;
        if (!response.Success)
            return null;

        try
        {
            var obj = Serializer.Deserialize<CStore_GetDiscoveryQueue_Response>(response.Stream!);
            return obj;
        }
        catch (Exception)
        {
            return null;
        }
    }
    public static async Task<CStore_GetDiscoveryQueue_Response?> GetDiscoveryQueueAsync(ApiRequest apiRequest, CStore_GetDiscoveryQueue_Request proto)
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, proto);
        var base64 = Convert.ToBase64String(ms.ToArray());

        var request = new ProtobufRequest(SteamApiUrls.IStoreService_GetDiscoveryQueue_v1, base64)
        {
            AccessToken = apiRequest.AuthToken,
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
            SpoofSteamId = 0,
        };
        using var response = await Downloader.GetProtobufAsync(request);
        if (response.EResult != EResult.OK)
            return null;
        if (!response.Success)
            return null;

        try
        {
            var obj = Serializer.Deserialize<CStore_GetDiscoveryQueue_Response>(response.Stream!);
            return obj;
        }
        catch (Exception)
        {
            return null;
        }
    }
}