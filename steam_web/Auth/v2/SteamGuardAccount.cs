using System.Text.Json;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using SteamWeb.Web;
using SteamWeb.Extensions;
using ProtoBuf;
using SteamWeb.Auth.v2.Models;
using SteamWeb.Auth.v2.DTO;
using TimeAligner = SteamWeb.Auth.v1.TimeAligner;
using APIEndpoints = SteamWeb.Auth.v1.APIEndpoints;

namespace SteamWeb.Auth.v2;
public class SteamGuardAccount
{
    [JsonPropertyName("shared_secret")] public string SharedSecret { get; init; }
    [JsonPropertyName("serial_number")] public ulong SerialNumber { get; init; }
    [JsonPropertyName("revocation_code")] public string RevocationCode { get; init; }
    [JsonPropertyName("uri")] public string URI { get; init; }
    [JsonPropertyName("server_time")] public int ServerTime { get; init; }
    [JsonPropertyName("account_name")] public string AccountName { get; init; }
    [JsonPropertyName("token_gid")] public string TokenGID { get; init; }
    [JsonPropertyName("identity_secret")] public string IdentitySecret { get; init; }
    [JsonPropertyName("secret_1")] public string Secret1 { get; init; }
    [JsonPropertyName("status")] public int Status { get; init; }
    [JsonPropertyName("device_id")] public string DeviceID { get; set; }
    /// <summary>
    /// Не сериализируется и не десерилизируется. Нужно устанавливать каждый раз при создании экземпляра класса.
    /// </summary>
    [JsonIgnore] public IWebProxy? Proxy { get; set; } = null;
    /// <summary>
    /// Set to true if the authenticator has actually been applied to the account.
    /// </summary>
    [JsonPropertyName("fully_enrolled")] public bool FullyEnrolled { get; set; } = false;
    public SessionData? Session { get; set; }
    private static byte[] _steamGuardCodeTranslations = new byte[] { 50, 51, 52, 53, 54, 55, 56, 57, 66, 67, 68, 70, 71, 72, 74, 75, 77, 78, 80, 81, 82, 84, 86, 87, 88, 89 };

    public bool DeactivateAuthenticator()
    {
        if (Session == null)
            return false;
        using var memStream1 = new MemoryStream();
        Serializer.Serialize(memStream1, new CTwoFactor_RemoveAuthenticator_Request()
        {
            //remove_all_steamguard_cookies = true,
            revocation_code = RevocationCode,
            revocation_reason = 1,
            steamguard_scheme = 1,
        });
        var request = new ProtobufRequest(SteamPoweredUrls.ITwoFactorService_RemoveAuthenticator_v1, Convert.ToBase64String(memStream1.ToArray()))
        {
            AccessToken = Session.AccessToken,
            Proxy = Proxy,
            UserAgent = SessionData.UserAgentMobile
        };
        using var response = Downloader.PostProtobuf(request);
        if (!response.Success || response.EResult != EResult.OK)
            return false;
        var obj = Serializer.Deserialize<CTwoFactor_RemoveAuthenticator_Response>(response.Stream);
        return obj.success;
	}
	public async Task<bool> DeactivateAuthenticatorAsync()
	{
		if (Session == null)
			return false;
		using var memStream1 = new MemoryStream();
		Serializer.Serialize(memStream1, new CTwoFactor_RemoveAuthenticator_Request()
		{
			//remove_all_steamguard_cookies = true,
			revocation_code = RevocationCode,
			revocation_reason = 1,
			steamguard_scheme = 1,
		});
		var request = new ProtobufRequest(SteamPoweredUrls.ITwoFactorService_RemoveAuthenticator_v1, Convert.ToBase64String(memStream1.ToArray()))
		{
			AccessToken = Session.AccessToken,
			Proxy = Proxy,
			UserAgent = SessionData.UserAgentMobile
		};
		using var response = await Downloader.PostProtobufAsync(request);
		if (!response.Success || response.EResult != EResult.OK)
			return false;
		var obj = Serializer.Deserialize<CTwoFactor_RemoveAuthenticator_Response>(response.Stream);
		return obj.success;
	}

	/// <summary>
	/// Generate Steam Guard Code from Steam Time Request
	/// </summary>
	/// <returns>Null Or Code</returns>
	public string GenerateSteamGuardCode() => GenerateSteamGuardCodeForTime(TimeAligner.GetSteamTime())!;
    /// <summary>
    /// Generate Steam Guard Code from your time
    /// </summary>
    /// <returns>Null Or Code</returns>
    public string? GenerateSteamGuardCodeForTime(long time)
    {
        if (SharedSecret == null || SharedSecret.Length == 0)
            return null;

        byte[] timeArray = new byte[8];
        time /= 30L;
        for (int i = 8; i > 0; i--)
        {
            timeArray[i - 1] = (byte)time;
            time >>= 8;
        }

        HMACSHA1 hmacGenerator = new HMACSHA1();
        hmacGenerator.Key = Convert.FromBase64String(Regex.Unescape(SharedSecret));
        byte[] hashedData = hmacGenerator.ComputeHash(timeArray);
        byte[] codeArray = new byte[5];
        try
        {
            byte b = (byte)(hashedData[19] & 0xF);
            int codePoint = (hashedData[b] & 0x7F) << 24 | (hashedData[b + 1] & 0xFF) << 16 | (hashedData[b + 2] & 0xFF) << 8 | (hashedData[b + 3] & 0xFF);
            int length = 5;
            for (int i = 0; i < length; ++i)
            {
                codeArray[i] = _steamGuardCodeTranslations[codePoint % _steamGuardCodeTranslations.Length];
                codePoint /= _steamGuardCodeTranslations.Length;
            }
        }
        catch (Exception)
        {
            return null; //Change later, catch-alls are bad!
        }
        return Encoding.UTF8.GetString(codeArray);
    }

    /// <summary>
    /// Refreshes the Steam session. Necessary to perform confirmations if your session has expired or changed.
    /// </summary>
    /// <returns>Доступные статусы:
    /// <code>Success</code>
    /// <code>WrongPlatform</code>
    /// <code>WGTokenExpired</code>
    /// <code>Error</code>
    /// </returns>
    public bool RefreshSession()
    {
        if (Session == null)
            return false;
        using var memStream1 = new MemoryStream();
        Serializer.Serialize(memStream1, new UpdateTokenRequest()
        {
            refresh_token = Session.RefreshToken,
            steamid = Session.SteamID
        }); 
        var request = new ProtobufRequest(SteamPoweredUrls.IAuthenticationService_GenerateAccessTokenForApp_v1, Convert.ToBase64String(memStream1.ToArray()))
        {
            AccessToken = Session.AccessToken,
            Proxy = Proxy,
            UserAgent = SessionData.UserAgentMobile
        };
        using var response = Downloader.PostProtobuf(request);
        if (!response.Success || response.EResult != EResult.OK)
            return false;
        var token = Serializer.Deserialize<UpdateTokenResponse>(response.Stream);
        if (token.access_token == null)
            return false;
        Session.AccessToken = token.access_token;
        return true;
    }

    /// <summary>
    /// Refreshes the Steam session. Necessary to perform confirmations if your session has expired or changed.
    /// </summary>
    /// <returns>Доступные статусы:
    /// <code>Success</code>
    /// <code>WrongPlatform</code>
    /// <code>WGTokenExpired</code>
    /// <code>Error</code>
    /// </returns>
    public async Task<bool> RefreshSessionAsync()
    {
        if (Session == null)
            return false;
        using var memStream1 = new MemoryStream();
        Serializer.Serialize(memStream1, new UpdateTokenRequest()
        {
            refresh_token = Session.RefreshToken,
            steamid = Session.SteamID
        });
        var request = new ProtobufRequest(SteamPoweredUrls.IAuthenticationService_GenerateAccessTokenForApp_v1, Convert.ToBase64String(memStream1.ToArray()))
        {
            AccessToken = Session.AccessToken,
            Proxy = Proxy,
            UserAgent = SessionData.UserAgentMobile
        };
        using var response = await Downloader.PostProtobufAsync(request);
        if (!response.Success || response.EResult != EResult.OK)
            return false;
        var token = Serializer.Deserialize<UpdateTokenResponse>(response.Stream);
        if (token.access_token == null)
            return false;
        Session.AccessToken = token.access_token;
        return true;
    }
    public bool CheckSession()
    {
        if (Session == null)
            return false;
        var request = new GetRequest(SteamPoweredUrls.IAuthenticationService_GetAuthSessionsForAccount_v1, Proxy, Session)
        {
            UserAgent = SessionData.UserAgentMobile,
            UseVersion2 = true
        }.AddQuery("access_token", Session.AccessToken).AddQuery("origin", "SteamMobile").AddQuery("input_protobuf_encoded", string.Empty);
        var response = Downloader.Get(request);
        if (!response.Success || response.Data.IsEmpty())
            return false;
        return response.EResult == EResult.OK && response.Data.IsEmpty();
        var sessionInfo = JsonSerializer.Deserialize<API.Models.Response<AuthSessionForAccount>>(response.Data);
        sessionInfo!.success = true;
        if (sessionInfo.response?.client_ids == null)
            return false;
        return true;
    }
    public async Task<bool> CheckSessionAsync()
    {
        if (Session == null)
            return false;
        var request = new GetRequest(SteamPoweredUrls.IAuthenticationService_GetAuthSessionsForAccount_v1, Proxy, Session)
        {
            UserAgent = SessionData.UserAgentMobile,
            UseVersion2 = true
        }.AddQuery("access_token", Session.AccessToken).AddQuery("origin", "SteamMobile").AddQuery("input_protobuf_encoded", string.Empty);
        var response = await Downloader.GetAsync(request);
        if (!response.Success || response.Data.IsEmpty())
            return false;
        return response.EResult == EResult.OK && response.Data.IsEmpty();
        var sessionInfo = JsonSerializer.Deserialize<API.Models.Response<AuthSessionForAccount>>(response.Data);
        sessionInfo!.success = true;
        if (sessionInfo.response?.client_ids == null)
            return false;
        return true;
    }
    /// <summary>
    /// Получает информацию о кошельке аккаунта
    /// </summary>
    /// <param name="request">Какую информацию получить</param>
    /// <returns>Null при ошибке</returns>
    public CUserAccount_GetWalletDetails_Response? WalletDetails(CUserAccount_GetClientWalletDetails_Request requestDetails)
    {
        if (Session == null)
            return null;
        using var memStream1 = new MemoryStream();
        Serializer.Serialize(memStream1, requestDetails);
        var request = new ProtobufRequest(SteamPoweredUrls.IUserAccountService_GetClientWalletDetails_v1, Convert.ToBase64String(memStream1.ToArray()))
        {
            UserAgent = SessionData.UserAgentMobile,
            Proxy = Proxy,
            Session = Session,
            AccessToken = Session.AccessToken
        };
        using var response = Downloader.PostProtobuf(request);
        if (response.EResult != EResult.OK)
            return null;
        var wallet = Serializer.Deserialize<CUserAccount_GetWalletDetails_Response>(response.Stream);
        return wallet;
    }
    /// <summary>
    /// Получает информацию о кошельке аккаунта
    /// </summary>
    /// <param name="request">Какую информацию получить</param>
    /// <returns>Null при ошибке</returns>
    public async Task<CUserAccount_GetWalletDetails_Response?> WalletDetailsAsync(CUserAccount_GetClientWalletDetails_Request requestDetails)
    {
        if (Session == null)
            return null;
        using var memStream1 = new MemoryStream();
        Serializer.Serialize(memStream1, requestDetails);
        var request = new ProtobufRequest(SteamPoweredUrls.IUserAccountService_GetClientWalletDetails_v1, Convert.ToBase64String(memStream1.ToArray()))
        {
            UserAgent = SessionData.UserAgentMobile,
            Proxy = Proxy,
            Session = Session,
            AccessToken = Session.AccessToken
        };
        using var response = await Downloader.PostProtobufAsync(request);
        if (response.EResult != EResult.OK)
            return null;
        var wallet = Serializer.Deserialize<CUserAccount_GetWalletDetails_Response>(response.Stream);
        return wallet;
    }
    public Confirmation[] FetchConfirmations()
    {
        if (Session == null)
            return new Confirmation[0];
        long time = TimeAligner.GetSteamTime();
        string tag = "list";
        var request = new GetRequest(SteamCommunityUrls.MobileConf_GetList, Proxy, Session)
        {
            UseVersion2 = true,
            UserAgent = SessionData.UserAgentMobile
        }.AddQuery("p", DeviceID).AddQuery("a", Session.SteamID)
        .AddQuery("k", _generateConfirmationHashForTime(time, tag)).AddQuery("t", time).AddQuery("m", "react").AddQuery("tag", tag);

        var response = Downloader.Get(request);
        if (!response.Success)
            return new Confirmation[0];
        var options = new JsonSerializerOptions
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };
        var confs = JsonSerializer.Deserialize<ConfirmationsResponse>(response.Data!, options);
        return confs!.conf;
    }
    public async Task<Confirmation[]> FetchConfirmationsAsync()
    {
        if (Session == null)
            return new Confirmation[0];
        long time = TimeAligner.GetSteamTime();
        string tag = "list";
        var request = new GetRequest(SteamCommunityUrls.MobileConf_GetList, Proxy, Session)
        {
            UseVersion2 = true,
            UserAgent = SessionData.UserAgentMobile
        }.AddQuery("p", DeviceID).AddQuery("a", Session.SteamID)
        .AddQuery("k", _generateConfirmationHashForTime(time, tag)).AddQuery("t", time).AddQuery("m", "react").AddQuery("tag", tag);

        var response = await Downloader.GetAsync(request);
        if (!response.Success)
            return new Confirmation[0];
		var options = new JsonSerializerOptions
		{
			NumberHandling = JsonNumberHandling.AllowReadingFromString
		};
		var confs = JsonSerializer.Deserialize<ConfirmationsResponse>(response.Data!, options);
        return confs!.conf;
    }

    public bool AcceptConfirmation(Confirmation conf, bool withCredentials)
    {
        if (Session == null)
            return false;
        string tag = "accept";
        string url = $"{APIEndpoints.COMMUNITY_BASE}/mobileconf/ajaxop?op=allow&{GenerateConfirmationQueryParams(tag)}&cid={conf.id}&ck={conf.nonce}";
        string content = withCredentials ? "{\"withCredentials\":true}" : "{\"withCredentials\":false}";
        var request = new PostRequest(url, content, Downloader.AppJson, Proxy, Session, null, Downloader.UserAgentOkHttp)
        {
            UseVersion2 = true,
            Timeout = 90000
        };
        var response = Downloader.Post(request);
        if (response.LostAuth)
            return false;
        if (!response.Success || response.Data.IsEmpty()) return false;
        var confResponse = JsonSerializer.Deserialize<SendConfirmationResponse>(response.Data);
        return confResponse!.Success;
    }
    public bool CancelConfirmation(Confirmation conf, bool withCredentials)
    {
        if (Session == null)
            return false;
        string tag = "reject";
        string url = $"{APIEndpoints.COMMUNITY_BASE}/mobileconf/ajaxop?op=allow&{GenerateConfirmationQueryParams(tag)}&cid={conf.id}&ck={conf.nonce}";
        string content = withCredentials ? "{\"withCredentials\":true}" : "{\"withCredentials\":false}";
        var request = new PostRequest(url, content, Downloader.AppJson, Proxy, Session, null, Downloader.UserAgentOkHttp) { UseVersion2 = true };
        var response = Downloader.Post(request);
        if (response.LostAuth)
            return false;
        if (!response.Success || response.Data.IsEmpty()) return false;
        var confResponse = JsonSerializer.Deserialize<SendConfirmationResponse>(response.Data);
        return confResponse!.Success;
    }
    public bool AcceptMultiConfirmations(Confirmation[] confs)
    {
        if (Session == null)
            return false;
        if (confs.Length == 0)
            return true;
        string tag = "accept";
        string url = $"{APIEndpoints.COMMUNITY_BASE}/mobileconf/multiajaxop?op=allow&{GenerateConfirmationQueryParams(tag)}";
        var sb = new StringBuilder().Append("cid[]=").Append(confs[0].id).Append("&ck[]=").Append(confs[0].nonce);
        for (int i = 1; i < confs.Length; i++)
        {
            var conf = confs[i];
            sb.Append("&cid[]=");
            sb.Append(conf.id);
            sb.Append("&ck[]=");
            sb.Append(conf.nonce);
        }
        var request = new PostRequest(url, sb.ToString(), Downloader.AppFormUrlEncoded, Proxy, Session, null, Downloader.UserAgentOkHttp)
        {
            UseVersion2 = true,
            Timeout = 90000
        };
        var response = Downloader.Post(request);
        if (response.LostAuth)
            return false;
        if (!response.Success || response.Data.IsEmpty())
            return false;
        var confResponse = JsonSerializer.Deserialize<SendConfirmationResponse>(response.Data);
        return confResponse!.Success;
    }
    public bool CancelMultiConfirmations(Confirmation[] confs)
    {
        if (Session == null)
            return false;
        if (confs.Length == 0)
            return true;
        string tag = "reject";
        string url = $"{APIEndpoints.COMMUNITY_BASE}/mobileconf/multiajaxop?op=allow&{GenerateConfirmationQueryParams(tag)}";
        var sb = new StringBuilder().Append("cid[]=").Append(confs[0].id).Append("&ck[]=").Append(confs[0].nonce);
        for (int i = 1; i < confs.Length; i++)
        {
            var conf = confs[i];
            sb.Append("&cid[]=");
            sb.Append(conf.id);
            sb.Append("&ck[]=");
            sb.Append(conf.nonce);
        }
        var request = new PostRequest(url, sb.ToString(), Downloader.AppFormUrlEncoded, Proxy, Session, null, Downloader.UserAgentOkHttp) { UseVersion2 = true };
        var response = Downloader.Post(request);
        if (response.LostAuth)
            return false;
        if (!response.Success || response.Data.IsEmpty())
            return false;
        var confResponse = JsonSerializer.Deserialize<SendConfirmationResponse>(response.Data);
        return confResponse!.Success;
    }


    public async Task<bool> AcceptConfirmationAsync(Confirmation conf, bool withCredentials)
    {
        if (Session == null)
            return false;
        string tag = "accept";
        string url = $"{APIEndpoints.COMMUNITY_BASE}/mobileconf/ajaxop?op=allow&{GenerateConfirmationQueryParams(tag)}&cid={conf.id}&ck={conf.nonce}";
        string content = withCredentials ? "{\"withCredentials\":true}" : "{\"withCredentials\":false}";
        var request = new PostRequest(url, content, Downloader.AppJson, Proxy, Session, null, Downloader.UserAgentOkHttp) { UseVersion2 = true };
        var response = await Downloader.PostAsync(request);
        if (response.LostAuth)
            return false;
        if (!response.Success || response.Data.IsEmpty()) return false;
        var confResponse = JsonSerializer.Deserialize<SendConfirmationResponse>(response.Data);
        return confResponse!.Success;
    }
    public async Task<bool> CancelConfirmationAsync(Confirmation conf, bool withCredentials)
    {
        if (Session == null)
            return false;
        string tag = "reject";
        string url = $"{APIEndpoints.COMMUNITY_BASE}/mobileconf/ajaxop?op=allow&{GenerateConfirmationQueryParams(tag)}&cid={conf.id}&ck={conf.nonce}";
        string content = withCredentials ? "{\"withCredentials\":true}" : "{\"withCredentials\":false}";
        var request = new PostRequest(url, content, Downloader.AppJson, Proxy, Session, null, Downloader.UserAgentOkHttp) { UseVersion2 = true };
        var response = await Downloader.PostAsync(request);
        if (response.LostAuth)
            return false;
        if (!response.Success || response.Data.IsEmpty()) return false;
        var confResponse = JsonSerializer.Deserialize<SendConfirmationResponse>(response.Data);
        return confResponse!.Success;
    }
    public async Task<bool> AcceptMultiConfirmationsAsync(Confirmation[] confs)
    {
        if (Session == null)
            return false;
        if (confs.Length == 0)
            return true;
        string tag = "accept";
        string url = $"{APIEndpoints.COMMUNITY_BASE}/mobileconf/multiajaxop?op=allow&{GenerateConfirmationQueryParams(tag)}";
        var sb = new StringBuilder().Append("cid[]=").Append(confs[0].id).Append("&ck[]=").Append(confs[0].nonce);
        for (int i = 1; i < confs.Length; i++)
        {
            var conf = confs[i];
            sb.Append("&cid[]=");
            sb.Append(conf.id);
            sb.Append("&ck[]=");
            sb.Append(conf.nonce);
        }
        var request = new PostRequest(url, sb.ToString(), Downloader.AppFormUrlEncoded, Proxy, Session, null, Downloader.UserAgentOkHttp) { UseVersion2 = true };
        var response = await Downloader.PostAsync(request);
        if (response.LostAuth)
            return false;
        if (!response.Success || response.Data.IsEmpty())
            return false;
        var confResponse = JsonSerializer.Deserialize<SendConfirmationResponse>(response.Data);
        return confResponse!.Success;
    }
    public async Task<bool> CancelMultiConfirmationsAsync(Confirmation[] confs)
    {
        if (Session == null)
            return false;
        if (confs.Length == 0)
            return true;
        string tag = "reject";
        string url = $"{APIEndpoints.COMMUNITY_BASE}/mobileconf/multiajaxop?op=allow&{GenerateConfirmationQueryParams(tag)}";
        var sb = new StringBuilder().Append("cid[]=").Append(confs[0].id).Append("&ck[]=").Append(confs[0].nonce);
        for (int i = 1; i < confs.Length; i++)
        {
            var conf = confs[i];
            sb.Append("&cid[]=");
            sb.Append(conf.id);
            sb.Append("&ck[]=");
            sb.Append(conf.nonce);
        }
        var request = new PostRequest(url, sb.ToString(), Downloader.AppFormUrlEncoded, Proxy, Session, null, Downloader.UserAgentOkHttp) { UseVersion2 = true };
        var response = await Downloader.PostAsync(request);
        if (response.LostAuth)
            return false;
        if (!response.Success || response.Data.IsEmpty())
            return false;
        var confResponse = JsonSerializer.Deserialize<SendConfirmationResponse>(response.Data);
        return confResponse!.Success;
    }

    public CAuthentication_GetAuthSessionsForAccount_Response? GetAuthSessionsForAccount()
    {
        if (Session == null)
            return null;
        var request = new ProtobufRequest(SteamPoweredUrls.IAuthenticationService_GetAuthSessionsForAccount_v1, string.Empty)
        {
            UserAgent = SessionData.UserAgentMobile,
            Proxy = Proxy,
            AccessToken = Session.AccessToken,
            IsMobile = true
        };
        using var response = Downloader.GetProtobuf(request);
        if (response.EResult != EResult.OK)
            return null;
        if (response.Stream == null)
            return new();
        var obj = Serializer.Deserialize<CAuthentication_GetAuthSessionsForAccount_Response>(response.Stream);
        return obj;
    }
    public CAuthentication_GetAuthSessionInfo_Response? GetAuthSessionInfo(CAuthentication_GetAuthSessionInfo_Request requestDetails)
    {
        if (Session == null)
            return null;
        using var memStream1 = new MemoryStream();
        Serializer.Serialize(memStream1, requestDetails);
        var request = new ProtobufRequest(SteamPoweredUrls.IAuthenticationService_GetAuthSessionInfo_v1, Convert.ToBase64String(memStream1.ToArray()))
        {
            UserAgent = SessionData.UserAgentMobile,
            Proxy = Proxy,
            AccessToken = Session.AccessToken
        };
        using var response = Downloader.PostProtobuf(request);
        if (response.EResult != EResult.OK)
            return null;
        var obj = Serializer.Deserialize<CAuthentication_GetAuthSessionInfo_Response>(response.Stream);
        return obj;
    }

    public string GenerateConfirmationURL(string tag = "conf")
    {
        var sb = new StringBuilder(4);
		sb.Append(APIEndpoints.COMMUNITY_BASE);
		sb.Append("/mobileconf/conf?");
		sb.Append(GenerateConfirmationQueryParams(tag));
		return sb.ToString();
    }
    public string GenerateConfirmationQueryParams(string tag)
    {
        if (string.IsNullOrEmpty(DeviceID))
            throw new ArgumentException("Device ID is not present");

        long time = TimeAligner.GetSteamTime();
        var sb = new StringBuilder(10);
        sb.Append("p=");
		sb.Append(DeviceID);
		sb.Append("&a=");
		sb.Append(Session?.SteamID);
		sb.Append("&k=");
		sb.Append(_generateConfirmationHashForTime(time, tag));
		sb.Append("&t=");
		sb.Append(time);
		sb.Append("&m=react&tag=");
		sb.Append(tag);
		return sb.ToString();
	}
    private string? _generateConfirmationHashForTime(long time, string tag)
    {
        byte[] decode = Convert.FromBase64String(IdentitySecret);
        int n2 = 8;
        if (tag != null)
        {
            if (tag.Length > 32) n2 = 8 + 32;
            else n2 = 8 + tag.Length;
        }
        byte[] array = new byte[n2];
        int n3 = 8;
        while (true)
        {
            int n4 = n3 - 1;
            if (n3 <= 0) break;
            array[n4] = (byte)time;
            time >>= 8;
            n3 = n4;
        }
        if (tag != null) Array.Copy(Encoding.UTF8.GetBytes(tag), 0, array, 8, n2 - 8);
        try
        {
            HMACSHA1 hmacGenerator = new HMACSHA1();
            hmacGenerator.Key = decode;
            byte[] hashedData = hmacGenerator.ComputeHash(array);
            string encodedData = Convert.ToBase64String(hashedData, Base64FormattingOptions.None);
            return encodedData;
        }
        catch
        {
            return null;
        }
    }
}