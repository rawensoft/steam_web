using ProtoBuf;
using SteamWeb.API.Models;
using SteamWeb.API.Protobufs;
using SteamWeb.Web;

namespace SteamWeb.API;
public static class ISteamAwardsService
{
    public static EResult Nominate(ApiRequest apiRequest, CSteamAwards_Nominate_Request proto)
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, proto);
        var base64 = Convert.ToBase64String(ms.ToArray());

        var request = new ProtobufRequest(SteamApiUrls.ISteamAwardsService_Nominate_v1, base64)
        {
            AccessToken = apiRequest.AuthToken,
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
            SpoofSteamId = 0,
        };
        using var response = Downloader.GetProtobuf(request);
        if (response.EResult != EResult.OK)
            return response.EResult;
        if (!response.Success)
            return EResult.Invalid;
        return EResult.OK;
    }
    public static async Task<EResult> NominateAsync(ApiRequest apiRequest, CSteamAwards_Nominate_Request proto)
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, proto);
        var base64 = Convert.ToBase64String(ms.ToArray());

        var request = new ProtobufRequest(SteamApiUrls.ISteamAwardsService_Nominate_v1, base64)
        {
            AccessToken = apiRequest.AuthToken,
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
            SpoofSteamId = 0,
        };
        using var response = await Downloader.GetProtobufAsync(request);
        if (response.EResult != EResult.OK)
            return response.EResult;
        if (!response.Success)
            return EResult.Invalid;
        return EResult.OK;
    }

    public static CSteamAwards_GetNominationRecommendations_Response? GetNominationRecommendations(ApiRequest apiRequest, CSteamAwards_GetNominationRecommendations_Request proto)
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, proto);
        var base64 = Convert.ToBase64String(ms.ToArray());

        var request = new ProtobufRequest(SteamApiUrls.ISteamAwardsService_GetNominationRecommendations_v1, base64)
        {
            AccessToken = apiRequest.AuthToken,
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
            SpoofSteamId = 0,
        };
        using var response = Downloader.GetProtobuf(request);
        if (!response.Success)
            return null;
        if (response.EResult != EResult.OK)
            return null;

        try
        {
            var obj = Serializer.Deserialize<CSteamAwards_GetNominationRecommendations_Response>(response.Stream!);
            return obj;
        }
        catch (Exception)
        {
            return null;
        }
    }
    public static async Task<CSteamAwards_GetNominationRecommendations_Response?> GetNominationRecommendationsAsync(ApiRequest apiRequest, CSteamAwards_GetNominationRecommendations_Request proto)
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, proto);
        var base64 = Convert.ToBase64String(ms.ToArray());

        var request = new ProtobufRequest(SteamApiUrls.ISteamAwardsService_GetNominationRecommendations_v1, base64)
        {
            AccessToken = apiRequest.AuthToken,
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
            SpoofSteamId = 0,
        };
        using var response = await Downloader.GetProtobufAsync(request);
        if (!response.Success)
            return null;
        if (response.EResult != EResult.OK)
            return null;

        try
        {
            var obj = Serializer.Deserialize<CSteamAwards_GetNominationRecommendations_Response>(response.Stream!);
            return obj;
        }
        catch (Exception)
        {
            return null;
        }
    }
}