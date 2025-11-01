using System.Text.Json;
using ProtoBuf;
using SteamWeb.API.Models;
using SteamWeb.Auth.v2.DTO;
using SteamWeb.Auth.v2.Models;
using SteamWeb.Web;
using SessionData = SteamWeb.Auth.v2.Models.SessionData;

namespace SteamWeb.API;
public static class IAuthenticationService
{
    /// <summary>
    /// Выдаёт информацию о текущей сессии, если она активная
    /// <code>
    /// if (sessionInfo.response == null || sessionInfo.response.client_ids.Length == 0) TODO: RefreshSession;
    /// </code>
    /// </summary>
    /// <param name="proxy"></param>
    /// <param name="session"></param>
    /// <returns>default если ошибка в запросе или нет сессии</returns>
    public static async Task<ResponseData<AuthSessionForAccount>?> GetAuthSessionsForAccountAsync(SessionData session, Proxy? proxy, CancellationToken? ct = null)
    {
        if (session == null)
            return default;
        var request = new GetRequest(SteamApiUrls.IAuthenticationService_GetAuthSessionsForAccount_v1)
        {
            Session = session,
            Proxy = proxy,
            UserAgent = KnownUserAgents.SteamMobileBrowser,
            UseVersion2 = true,
			CancellationToken = ct
		}.AddQuery("access_token", session.AccessToken!);
        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return default;

        var sessionInfo = JsonSerializer.Deserialize<ResponseData<AuthSessionForAccount>>(response.Data!, Steam.JsonOptions)!;
        sessionInfo.Success = true;
        return sessionInfo;
    }
    /// <summary>
    /// Выдаёт информацию о текущей сессии, если она активная
    /// <code>
    /// if (sessionInfo.response == null || sessionInfo.response.client_ids.Length == 0) TODO: RefreshSession;
    /// </code>
    /// </summary>
    /// <param name="proxy"></param>
    /// <param name="session"></param>
    /// <returns>default если ошибка в запросе или нет сессии</returns>
    public static ResponseData<AuthSessionForAccount>? GetAuthSessionsForAccount(SessionData session, Proxy? proxy, CancellationToken? ct = null)
    {
        if (session == null)
            return default;

        var request = new GetRequest(SteamApiUrls.IAuthenticationService_GetAuthSessionsForAccount_v1)
        {
            Session = session,
            Proxy = proxy,
            UserAgent = KnownUserAgents.SteamMobileBrowser,
            UseVersion2 = true,
			CancellationToken = ct
		}.AddQuery("access_token", session.AccessToken!);
        var response = Downloader.Get(request);
        if (!response.Success)
            return default;

        var sessionInfo = JsonSerializer.Deserialize<ResponseData<AuthSessionForAccount>>(response.Data!, Steam.JsonOptions)!;
        sessionInfo.Success = true;
        return sessionInfo;
    }

    /// <summary>
    /// Обновляет access token по refresh token из session
    /// <code>
    /// if (response.Item1 == EResult.OK) {
    ///     if (!response.Item2.AccessToken.IsEmpty())
    ///         session.AccessToken = response.Item2.AccessToken;
    /// }
    /// </code>
    /// </summary>
    /// <param name="proxy"></param>
    /// <param name="session"></param>
    /// <returns></returns>
    public static async Task<(EResult, UpdateTokenResponse?)> GenerateAccessTokenForAppAsync(SessionData session, Proxy? proxy, CancellationToken? ct = null)
    {
        if (session == null)
            return (EResult.Invalid, default);

        using var memStream1 = new MemoryStream();
        Serializer.Serialize(memStream1, new UpdateTokenRequest()
        {
            refresh_token = session.RefreshToken!,
            steamid = session.SteamID
        });
        var request = new ProtobufRequest(SteamApiUrls.IAuthenticationService_GenerateAccessTokenForApp_v1, Convert.ToBase64String(memStream1.ToArray()))
        {
            Session = session,
            Proxy = proxy,
            UserAgent = KnownUserAgents.SteamMobileBrowser,
            AccessToken = session.AccessToken,
			CancellationToken = ct
		};
        using var response = await Downloader.PostProtobufAsync(request);

        if (response.EResult != EResult.OK)
            return (response.EResult, default);

        var token = Serializer.Deserialize<UpdateTokenResponse>(response.Stream);
        if (token.AccessToken == null)
            return (response.EResult, token);

        response.Stream?.Close();
        response.Stream?.Dispose();
        return (response.EResult, token);
    }
    /// <summary>
    /// Обновляет access token по refresh token из session
    /// <code>
    /// if (response.Item1 == EResult.OK) {
    ///     if (!response.Item2.AccessToken.IsEmpty())
    ///         session.AccessToken = response.Item2.AccessToken;
    /// }
    /// </code>
    /// </summary>
    /// <param name="proxy"></param>
    /// <param name="session"></param>
    /// <returns></returns>
    public static (EResult, UpdateTokenResponse?) GenerateAccessTokenForApp(SessionData session, Proxy? proxy, CancellationToken? ct = null)
    {
        if (session == null)
            return (EResult.Invalid, default);
        using var memStream1 = new MemoryStream();
        Serializer.Serialize(memStream1, new UpdateTokenRequest()
        {
            refresh_token = session.RefreshToken!,
            steamid = session.SteamID
        });
        var request = new ProtobufRequest(SteamApiUrls.IAuthenticationService_GenerateAccessTokenForApp_v1, Convert.ToBase64String(memStream1.ToArray()))
        {
            Session = session,
            Proxy = proxy,
            UserAgent = KnownUserAgents.SteamMobileBrowser,
            AccessToken = session.AccessToken,
            CancellationToken = ct
        };

        using var response = Downloader.PostProtobuf(request);
        if (response.EResult != EResult.OK)
            return (response.EResult, default);

        var token = Serializer.Deserialize<UpdateTokenResponse>(response.Stream);
        if (token.access_token == null)
            return (response.EResult, token);

        response.Stream?.Close();
        response.Stream?.Dispose();
        return (response.EResult, token);
    }
}