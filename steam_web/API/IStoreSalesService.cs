using ProtoBuf;
using SteamWeb.API.Models;
using SteamWeb.Script.DTO;
using SteamWeb.Web;

namespace SteamWeb.API;
public static class IStoreSalesService
{
    public static CStoreSalesService_SetVote_Response? SetVote(ApiRequest apiRequest, CStoreSalesService_SetVote_Request proto)
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, proto);
        var base64 = Convert.ToBase64String(ms.ToArray());

        var request = new ProtobufRequest(SteamApiUrls.IStoreSalesService_SetVote_v1, base64)
        {
            AccessToken = apiRequest.AuthToken,
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
            SpoofSteamId = 0,
            Referer = "https://store.steampowered.com/",
        };
        using var response = Downloader.PostProtobuf(request);
        if (response.EResult != EResult.OK)
            return null;
        if (!response.Success)
            return null;

        try
        {
            var obj = Serializer.Deserialize<CStoreSalesService_SetVote_Response>(response.Stream!);
            return obj;
        }
        catch (Exception)
        {
            return null;
        }
    }
    public static async Task<CStoreSalesService_SetVote_Response?> SetVoteAsync(ApiRequest apiRequest, CStoreSalesService_SetVote_Request proto)
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, proto);
        var base64 = Convert.ToBase64String(ms.ToArray());

        var request = new ProtobufRequest(SteamApiUrls.IStoreSalesService_SetVote_v1, base64)
        {
            AccessToken = apiRequest.AuthToken,
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
            SpoofSteamId = 0,
            Referer = "https://store.steampowered.com/",
        };
        using var response = await Downloader.PostProtobufAsync(request);
        if (response.EResult != EResult.OK)
            return null;
        if (!response.Success)
            return null;

        try
        {
            var obj = Serializer.Deserialize<CStoreSalesService_SetVote_Response>(response.Stream!);
            return obj;
        }
        catch (Exception)
        {
            return null;
        }
    }
}