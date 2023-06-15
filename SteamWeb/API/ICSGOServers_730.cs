using System;
using System.Text.Json;
using System.Threading.Tasks;
using SteamWeb.API.Models.ICSGOServers_730;
using SteamWeb.Web;
using SteamWeb.API.Models;

namespace SteamWeb.API;
public static class ICSGOServers_730
{
    /// <summary>
    /// offline, idle, low, normal, medium
    /// </summary>
    /// <param name="Proxy"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static async Task<Result<GameServerStatus>> GetGameServersStatusAsync(Proxy proxy, string key)
    {
        var response = await Downloader.GetAsync((new GetRequest(SteamPoweredUrls.ICSGOServers_730_GetGameServersStatus_v1, proxy)).AddQuery("key", key));
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Result<GameServerStatus>>(response.Data);
            obj.success = true;
            return obj;
        }
        catch (Exception ex)
        { }
        return new();
    }
    /// <summary>
    /// offline, idle, low, normal, medium
    /// </summary>
    /// <param name="Proxy"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static Result<GameServerStatus> GetGameServersStatus(Proxy proxy, string key)
    {
        var response = Downloader.Get((new GetRequest(SteamPoweredUrls.ICSGOServers_730_GetGameServersStatus_v1, proxy)).AddQuery("key", key));
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<Result<GameServerStatus>>(response.Data);
            obj.success = true;
            return obj;
        }
        catch (Exception ex)
        { }
        return new();
    }
}
