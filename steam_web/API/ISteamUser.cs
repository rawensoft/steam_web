using SteamWeb.Web;
using System.Text.Json;
using SteamWeb.API.Models.ISteamUser;
using SteamWeb.API.Enums;
using SteamWeb.API.Models;
using System.Text;

namespace SteamWeb.API;
public static class ISteamUser
{
    public static async Task<Response<Players<PlayerSummary>>> GetPlayerSummariesAsync(ApiRequest apiRequest, ulong[] steamids)
    {
        if (steamids.Length == 0)
            throw new ArgumentException(nameof(steamids));
        if (steamids.Length > 100)
            throw new ArgumentOutOfRangeException(nameof(steamids));

        var sb = new StringBuilder();
        foreach (var steamid in steamids)
            sb.Append(steamid.ToString())
                .Append(',');
        if (sb[^1] == ',')
            sb.Remove(sb.Length - 1, 1);

        var request = new GetRequest(SteamPoweredUrls.ISteamUser_GetPlayerSummaries_v2)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        }
            .AddQuery("key", apiRequest.AuthToken!).AddQuery("steamids", sb.ToString());
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Response<Players<PlayerSummary>>>(response.Data!)!;
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    public static Response<Players<PlayerSummary>> GetPlayerSummaries(ApiRequest apiRequest, ulong[] steamids)
    {
        if (steamids.Length == 0)
            throw new ArgumentException(nameof(steamids));
        if (steamids.Length > 100)
            throw new ArgumentOutOfRangeException(nameof(steamids));

        var sb = new StringBuilder();
        foreach (var steamid in steamids)
            sb.Append(steamid.ToString()).Append(',');
        if (sb[^1] == ',')
            sb.Remove(sb.Length - 1, 1);

        var request = new GetRequest(SteamPoweredUrls.ISteamUser_GetPlayerSummaries_v2)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        }
            .AddQuery("key", apiRequest.AuthToken!).AddQuery("steamids", sb.ToString());
        var response = Downloader.Get(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Response<Players<PlayerSummary>>>(response.Data!)!;
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    public static async Task<PlayerBans> GetPlayerBansAsync(ApiRequest apiRequest, ulong[] steamids)
    {
        if (steamids.Length == 0)
            throw new ArgumentException(nameof(steamids));
        if (steamids.Length > 100)
            throw new ArgumentOutOfRangeException(nameof(steamids));
        var sb = new StringBuilder();

        foreach (var steamid in steamids)
            sb.Append(steamid.ToString()).Append(',');
        if (sb[^1] == ',')
            sb.Remove(sb.Length - 1, 1);

        var request = new GetRequest(SteamPoweredUrls.ISteamUser_GetPlayerBans_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        }
            .AddQuery("key", apiRequest.AuthToken!).AddQuery("steamids", sb.ToString());
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<PlayerBans>(response.Data!)!;
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    public static PlayerBans GetPlayerBans(ApiRequest apiRequest, params ulong[] steamids)
    {
        if (steamids.Length == 0)
            throw new ArgumentException(nameof(steamids));
        if (steamids.Length > 100)
            throw new ArgumentOutOfRangeException(nameof(steamids));

        var sb = new StringBuilder();
        foreach (var steamid in steamids)
            sb.Append(steamid.ToString()).Append(',');
        if (sb[^1] == ',')
            sb.Remove(sb.Length - 1, 1);

        var request = new GetRequest(SteamPoweredUrls.ISteamUser_GetPlayerBans_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        }
            .AddQuery("key", apiRequest.AuthToken!).AddQuery("steamids", sb.ToString());
        var response = Downloader.Get(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<PlayerBans>(response.Data!)!;
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }

    public static async Task<ResponseFriends<PlayerFriend>> GetFriendList(ApiRequest apiRequest, ulong steamid)
    {
        var request = new GetRequest(SteamPoweredUrls.ISteamUser_GetFriendList_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        }.AddQuery("key", apiRequest.AuthToken!).AddQuery("steamid", steamid);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResponseFriends<PlayerFriend>>(response.Data!)!;
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
    /// <param name="vanityurl">The vanity URL to get a SteamID for</param>
    /// <param name="url_type">The type of vanity URL. 1 (default): Individual profile, 2: Group, 3: Official game group</param>
    /// <returns></returns>
    public static async Task<Response<VanityUrl>> ResolveVanityURL(ApiRequest apiRequest, string vanityurl, VANITY_TYPE url_type)
    {
        var request = new GetRequest(SteamPoweredUrls.ISteamUser_ResolveVanityURL_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        }.AddQuery("key", apiRequest.AuthToken!).AddQuery("vanityurl", vanityurl).AddQuery("url_type", (int)url_type);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Response<VanityUrl>>(response.Data!)!;
            obj.success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
}