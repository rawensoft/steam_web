using ProtoBuf;
using SteamWeb.API.Models;
using SteamWeb.API.Protobufs;
using SteamWeb.Auth.v2.DTO;
using SteamWeb.Auth.v2.Models;
using SteamWeb.Models;
using SteamWeb.Web;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
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
            RefreshToken = session.RefreshToken!,
            SteamId = session.SteamID
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
            RefreshToken = session.RefreshToken!,
            SteamId = session.SteamID
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
        if (token.AccessToken == null)
            return (response.EResult, token);

        response.Stream?.Close();
        response.Stream?.Dispose();
        return (response.EResult, token);
    }
    public static (EResult, UpdateTokenResponse?) GenerateAccessTokenForApp(string refresh_token, ulong steamid, Proxy? proxy, CancellationToken? ct = null)
    {
        var jwt = JwtData.Deserialize(refresh_token);
        using var memStream1 = new MemoryStream();
        Serializer.Serialize(memStream1, new UpdateTokenRequest()
        {
            RefreshToken = refresh_token!,
            SteamId = steamid,
        });
        var request = new ProtobufRequest(SteamApiUrls.IAuthenticationService_GenerateAccessTokenForApp_v1, Convert.ToBase64String(memStream1.ToArray()))
        {
            Proxy = proxy,
            UserAgent = "Valve/Steam HTTP Client 1.0",
            CancellationToken = ct,
            AccessToken = jwt.Subject + "||" + refresh_token,
        };

        using var response = Downloader.PostProtobuf(request);
        if (response.EResult != EResult.OK)
            return (response.EResult, default);

        var token = Serializer.Deserialize<UpdateTokenResponse>(response.Stream);
        if (token.AccessToken == null)
            return (response.EResult, token);

        response.Stream?.Close();
        response.Stream?.Dispose();
        return (response.EResult, token);
    }

    public static (EResult, CAuthentication_RefreshToken_Enumerate_Response?) EnumerateTokens(DefaultRequest request, CAuthentication_RefreshToken_Enumerate_Request proto)
    {
        using var memStream1 = new MemoryStream();
        Serializer.Serialize(memStream1, proto);
        var protoRequest = new ProtobufRequest(SteamApiUrls.IAuthenticationService_EnumerateTokens_v1, Convert.ToBase64String(memStream1.ToArray()))
        {
            Proxy = request.Proxy,
            CancellationToken = request.CancellationToken,
            AccessToken = request.Session?.AccessToken,
        };

        using var protoResponse = Downloader.PostProtobuf(protoRequest);
        if (protoResponse.EResult != EResult.OK)
            return (protoResponse.EResult, default);

        var token = Serializer.Deserialize<CAuthentication_RefreshToken_Enumerate_Response>(protoResponse.Stream);
        return (protoResponse.EResult, token);
    }
    public static async Task<(EResult, CAuthentication_RefreshToken_Enumerate_Response?)> EnumerateTokensAsync(DefaultRequest request, CAuthentication_RefreshToken_Enumerate_Request proto)
    {
        using var memStream1 = new MemoryStream();
        Serializer.Serialize(memStream1, proto);
        var protoRequest = new ProtobufRequest(SteamApiUrls.IAuthenticationService_EnumerateTokens_v1, Convert.ToBase64String(memStream1.ToArray()))
        {
            Proxy = request.Proxy,
            CancellationToken = request.CancellationToken,
            AccessToken = request.Session?.AccessToken,
        };

        using var protoResponse = await Downloader.PostProtobufAsync(protoRequest);
        if (protoResponse.EResult != EResult.OK)
            return (protoResponse.EResult, default);

        var token = Serializer.Deserialize<CAuthentication_RefreshToken_Enumerate_Response>(protoResponse.Stream);
        return (protoResponse.EResult, token);
    }
}