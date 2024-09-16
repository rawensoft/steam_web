using System.Text.Json;
using SteamWeb.API.Models.ISteamEconomy;
using SteamWeb.API.Models;
using SteamWeb.Web;
using SteamWeb.API.Models.ICSGOPlayers_730;

namespace SteamWeb.API;
public static class ICSGOPlayers_730
{
    /// <summary>
    /// Находим следующий код матча
    /// </summary>
    /// <param name="apiRequest"></param>
    /// <param name="steamid">steamid64 игрока</param>
    /// <param name="steamidkey">Ключ для доступа к истории матчей.<para/>Отображается на странице <see href="https://steamcommunity.com/my/gcpd/730/?tab=authcodes"/><para/>Можно получить на странице <see href="https://help.steampowered.com/ru/wizard/HelpWithGameIssue/?appid=730&amp;issueid=128"/></param>
    /// <param name="knowncode">Код должен быть в формате 'CSGO-*-*-*-*-*', либо 'steam://rungame/730/steamid/+csgo_download_match%20CSGO-*-*-*-*-*'</param>
    /// <returns>Информация о следующием матче</returns>
    public static ResultData<MatchSharingCode> GetNextMatchSharingCode(ApiRequest apiRequest, ulong steamid, string steamidkey, string knowncode)
    {
        if (knowncode.StartsWith("steam://"))
            knowncode = knowncode.Split("%20")[1];
        var request = new GetRequest(SteamPoweredUrls.ICSGOPlayers_730_GetNextMatchSharingCode_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request).AddQuery("steamid", steamid).AddQuery("steamidkey", steamidkey, false).AddQuery("knowncode", knowncode, false);
        var response = Downloader.Get(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResultData<MatchSharingCode>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
    /// <summary>
    /// Находим следующий код матча
    /// </summary>
    /// <param name="apiRequest"></param>
    /// <param name="steamid">steamid64 игрока</param>
    /// <param name="steamidkey">Ключ для доступа к истории матчей.<para/>Отображается на странице <see href="https://steamcommunity.com/my/gcpd/730/?tab=authcodes"/><para/>Можно получить на странице <see href="https://help.steampowered.com/ru/wizard/HelpWithGameIssue/?appid=730&amp;issueid=128"/></param>
    /// <param name="knowncode">Код должен быть в формате 'CSGO-*-*-*-*-*', либо 'steam://rungame/730/steamid/+csgo_download_match%20CSGO-*-*-*-*-*'</param>
    /// <returns>Информация о следующием матче</returns>
    public static async Task<ResultData<MatchSharingCode>> GetNextMatchSharingCodeAsync(ApiRequest apiRequest, ulong steamid, string steamidkey, string knowncode)
    {
        if (knowncode.StartsWith("steam://"))
            knowncode = knowncode.Split("%20")[1];
        var request = new GetRequest(SteamPoweredUrls.ICSGOPlayers_730_GetNextMatchSharingCode_v1)
        {
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
        };
        apiRequest.AddAuthToken(request).AddQuery("steamid", steamid).AddQuery("steamidkey", steamidkey, false).AddQuery("knowncode", knowncode, false);
        var response = Downloader.Get(request);
        if (!response.Success)
            return new();
        try
        {
            var obj = JsonSerializer.Deserialize<ResultData<MatchSharingCode>>(response.Data!, Steam.JsonOptions)!;
            obj.Success = true;
            return obj;
        }
        catch (Exception)
        {
            return new();
        }
    }
}