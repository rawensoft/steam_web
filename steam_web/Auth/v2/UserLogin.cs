using System.Net;
using System.Text;
using ProtoBuf;
using SteamWeb.Auth.v2.Enums;
using SteamWeb.Auth.v2.Models;
using SteamWeb.Extensions;
using SteamWeb.Web;

namespace SteamWeb.Auth.v2;
public class UserLogin
{
	private const string SocketError = "SOCKS server failed to connect to the destination.";
	private const string UnauthorizedError = "You don't have permission to access \"http&#58;&#47;&#47;store&#46;steampowered&#46;com&#47;\" on this server.";
	private const string CookieSteamCountry = "steamCountry";
	private const string CookieBrowserId = "browserid";
	private const string CookieSessionId = "sessionid";


	public string Login { get; init; }
    public string Password { get; init; }
    /// <summary>
    /// Появится только после BeginAuthSessionViaCredentials
    /// </summary>
    public ulong SteamID64 { get; private set; }
    /// <summary>
    /// Как нужно подтвердить вход в аккаунт
    /// </summary>
    public EAuthSessionGuardType[] Approve { get; private set; } = new EAuthSessionGuardType[0];
    public SessionData? Session { get; private set; }
    /// <summary>
    /// Полностью ли пройдена авторизация
    /// </summary>
    public bool FullyEnrolled { get; private set; } = false;
    /// <summary>
    /// Код с мобильного приложения или с почты
    /// </summary>
    public string? Data { get; set; }
    /// <summary>
    /// Последний код EResult от Steam
    /// </summary>
    public EResult LastEResult { get; private set; } = EResult.OK;
    /// <summary>
    /// След шаг, нужный для успешной авторизации
    /// </summary>
    public NEXT_STEP NextStep => _nextStep;
    /// <summary>
    /// Текущий статус авторизации
    /// </summary>
    public LoginResult Result
    {
        get
        {
            if (_result == LoginResult.GeneralFailure || _result == LoginResult.NeedAprove) SetEnumResponse();
            return _result;
        }
    }
    public bool IsNeedTwoFactorCode => _isNeedTwoFactorCode;
    public bool IsNeedEmailCode => _isNeedEmailCode;
    public bool IsNeedConfirm => _isNeedConfirm;
	internal string? WeakToken { get; private set; } = null;
	internal IWebProxy? Proxy { get; init; } = null;
	internal EAuthTokenPlatformType Platform { get; init; }

	private bool _isNeedTwoFactorCode = false;
    private bool _isNeedEmailCode = false;
    private bool _isNeedConfirm = false;
    private NEXT_STEP _nextStep = NEXT_STEP.Begin;
    private byte[]? _request_id = null;
    private ulong _client_id = 0;
    private bool? _isCookieNotGet = null;
    private bool? _isRSANotGet = null;
    private LoginResult _result = LoginResult.GeneralFailure;
	private CancellationToken? _cts = null;

	public UserLogin(string login, string passwd, EAuthTokenPlatformType platform)
    {
        Login = login;
        Password = passwd;
        Platform = platform;
    }
    public UserLogin(string login, string passwd, EAuthTokenPlatformType platform, IWebProxy? proxy) : this(login, passwd, platform) => Proxy = proxy;
	public UserLogin(string login, string passwd, EAuthTokenPlatformType platform, IWebProxy? proxy, CancellationToken? cts) :
        this(login, passwd, platform, proxy) => _cts = cts;
	public UserLogin(string login, string passwd, EAuthTokenPlatformType platform, CancellationToken? cts) : this(login, passwd, platform) => _cts = cts;

	private void SetEnumResponse()
    {
        if (FullyEnrolled)
            _result = LoginResult.LoginOkay;
        else if (_nextStep == NEXT_STEP.Update)
            _result = LoginResult.NeedAprove;
        else if (LastEResult == EResult.InvalidPassword)
            _result = LoginResult.BadCredentials;
        else if (LastEResult == EResult.AccountLimitExceeded ||
            LastEResult == EResult.RateLimitExceeded ||
            LastEResult == EResult.AccountActivityLimitExceeded ||
            LastEResult == EResult.PhoneActivityLimitExceeded ||
			LastEResult == EResult.AccountLoginDeniedThrottle)
            _result = LoginResult.RateExceeded;
        else if (_isCookieNotGet == true)
            _result = LoginResult.BadCookie;
        else if (_isRSANotGet == true)
            _result = LoginResult.BadRSA;
        else _result = LoginResult.GeneralFailure;
    }

    public bool BeginAuthSessionViaCredentials()
    {
        if (_nextStep != NEXT_STEP.Begin)
            return false;
        var usetAgent = Platform == EAuthTokenPlatformType.MobileApp ? KnownUserAgents.OkHttp : KnownUserAgents.WindowsBrowser;
        var getRequest = new GetRequest(KnownUri.BASE_POWERED)
        {
			Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application",
            UserAgent = usetAgent,
            Proxy = Proxy,
            IsMobile = Platform == EAuthTokenPlatformType.MobileApp,
			CancellationToken = _cts
		};
		var response = Downloader.Get(getRequest);
		if (response.ErrorMessage == SocketError)
		{
			_result = LoginResult.ProxyError;
			return false;
		}
		if (response.SocketErrorCode == System.Net.Sockets.SocketError.TimedOut)
		{
			_result = LoginResult.Timeout;
			return false;
		}
		if (response.SocketErrorCode != System.Net.Sockets.SocketError.Success)
		{
			_result = LoginResult.ConnectionError;
			return false;
		}
		if (response.CookieContainer == null)
        {
            if (response.Data?.Contains(UnauthorizedError) == true)
                LastEResult = EResult.RateLimitExceeded;
            _isCookieNotGet = true;
            return false;
        }
		
		string? steamCountry = null, browserID = null, sessionID = null;
		var cookies = response.CookieContainer.GetAllCookies();
		foreach (Cookie cookie in cookies)
        {
            if (cookie.Name == CookieSteamCountry)
                steamCountry = cookie.Value;
            else if (cookie.Name == CookieBrowserId)
                browserID = cookie.Value;
            else if (cookie.Name == CookieSessionId)
                sessionID = cookie.Value;
        }
		if (steamCountry.IsEmpty() || browserID.IsEmpty() || sessionID.IsEmpty())
        {
            _isCookieNotGet = true;
            return false;
        }
        _isCookieNotGet = false;

        using var memStream = new MemoryStream();
        Serializer.Serialize(memStream, new PasswordRSARequest() { account_name = Login });
        var tmp = Convert.ToBase64String(memStream.ToArray());
        var getRequestProto = new ProtobufRequest(SteamApiUrls.IAuthenticationService_GetPasswordRSAPublicKey_v1, tmp)
        {
            Proxy = Proxy,
            UserAgent = usetAgent,
            Cookie = KnownCookies.DefaultMobileCookie,
			CancellationToken = _cts
		};
        using var responseProto1 = Downloader.GetProtobuf(getRequestProto);
		LastEResult = responseProto1.EResult;
		if (responseProto1.ErrorMessage == SocketError)
		{
			_result = LoginResult.ProxyError;
			return false;
		}
		if (responseProto1.SocketErrorCode == System.Net.Sockets.SocketError.TimedOut)
		{
			_result = LoginResult.Timeout;
			return false;
		}
		if (responseProto1.SocketErrorCode != System.Net.Sockets.SocketError.Success)
		{
			_result = LoginResult.ConnectionError;
			return false;
		}
		if (responseProto1.EResult != EResult.OK)
		{
			return false;
		}
        var rsaResponse = Serializer.Deserialize<PasswordRSAResponse>(responseProto1.Stream);
        _isRSANotGet = false;

		string encryptedPassword = Helpers.Encrypt(Password, rsaResponse.publickey_mod, rsaResponse.publickey_exp);
		using var memStream1 = new MemoryStream();
		if (Platform == EAuthTokenPlatformType.MobileApp)
		{
			var request = new AuthSessionMobileRequest()
			{
				account_name = Login,
				encrypted_password = encryptedPassword,
				encryption_timestamp = rsaResponse.timestamp,
				device_details = new()
			};
			Serializer.Serialize(memStream1, request);
		}
		else
		{
			var request = new AuthSessionDesktopRequest()
			{
				account_name = Login,
				encrypted_password = encryptedPassword,
				encryption_timestamp = rsaResponse.timestamp,
				device_details = new()
				{
					device_friendly_name = usetAgent,
					platform_type = Platform
				}
			};
			Serializer.Serialize(memStream1, request);
		}
        string content = Convert.ToBase64String(memStream1.ToArray());
		var stringCookies = new StringBuilder(9);
		stringCookies.Append(KnownCookies.DefaultMobileCookie);
		stringCookies.Append("steamCountry=");
		stringCookies.Append(steamCountry);
		stringCookies.Append("; browserid=");
		stringCookies.Append(browserID);
		stringCookies.Append("; sessionid=");
		stringCookies.Append(sessionID);
		stringCookies.Append("; ");
		var postRequestProto = new ProtobufRequest(SteamApiUrls.IAuthenticationService_BeginAuthSessionViaCredentials_v1, content)
        {
            Proxy = Proxy,
            UserAgent = usetAgent,
            Cookie = stringCookies.ToString(),
			CancellationToken = _cts
		};
        using var responseProto2 = Downloader.PostProtobuf(postRequestProto);
        LastEResult = responseProto2.EResult;
		if (responseProto2.ErrorMessage == SocketError)
		{
			_result = LoginResult.ProxyError;
			return false;
		}
		if (responseProto2.SocketErrorCode == System.Net.Sockets.SocketError.TimedOut)
		{
			_result = LoginResult.Timeout;
			return false;
		}
		if (responseProto2.SocketErrorCode != System.Net.Sockets.SocketError.Success)
		{
			_result = LoginResult.ConnectionError;
			return false;
		}
		if (responseProto2.EResult != EResult.OK)
		{
			return false;
		}
		var authSession = Serializer.Deserialize<AuthSessionResponse>(responseProto2.Stream);

        SteamID64 = authSession.steamid;
        Session = new()
        {
            SteamID = authSession.steamid,
            BrowserId = browserID!,
            SessionID = sessionID!,
            SteamCountry = steamCountry!,
            PlatformType = Platform,
        };
        _client_id = authSession.client_id;
        _request_id = authSession.request_id;
		WeakToken = authSession.weak_token;

		var length = authSession.allowed_confirmations.Length;
        Approve = new EAuthSessionGuardType[length];
        for (int i = 0; i < length; i++)
        {
            var item = authSession.allowed_confirmations[i].message;
            Approve[i] = item;
            if (item == EAuthSessionGuardType.None)
                continue;
            if (item == EAuthSessionGuardType.Unknown)
                continue;
            if (item == EAuthSessionGuardType.EmailCode)
                _isNeedEmailCode = true;
            else if (item == EAuthSessionGuardType.DeviceCode)
                _isNeedTwoFactorCode = true;
            else _isNeedConfirm = true;
        }
        if (_isNeedConfirm || _isNeedTwoFactorCode || _isNeedEmailCode)
            _nextStep = NEXT_STEP.Update;
        else _nextStep = NEXT_STEP.Poll;
        return true;
    }
    public bool UpdateAuthSessionWithSteamGuardCode(string? fa2Code)
    {
        if (fa2Code.IsEmpty())
        {
            if (Data.IsEmpty())
                return false;
            fa2Code = Data!;
        }
        if (!IsNeedEmailCode && !IsNeedTwoFactorCode)
            return false;
        using var memStream2 = new MemoryStream();
        Serializer.Serialize(memStream2, new AuthGuardRequest()
        {
            client_id = _client_id,
            code = fa2Code,
            code_type = IsNeedEmailCode ? EAuthSessionGuardType.EmailCode : EAuthSessionGuardType.DeviceCode,
            steamid = Session!.SteamID
        });
        string content = Convert.ToBase64String(memStream2.ToArray());
        var protoRequest = new ProtobufRequest(SteamApiUrls.IAuthenticationService_UpdateAuthSessionWithSteamGuardCode_v1, content)
        {
            UserAgent = Platform == EAuthTokenPlatformType.MobileApp ? KnownUserAgents.OkHttp : KnownUserAgents.WindowsBrowser,
            Proxy = Proxy,
			CancellationToken = _cts
		};
        using var response = Downloader.PostProtobuf(protoRequest);
        LastEResult = response.EResult;
		if (response.ErrorMessage == SocketError)
		{
			_result = LoginResult.ProxyError;
			return false;
		}
		if (response.SocketErrorCode == System.Net.Sockets.SocketError.TimedOut)
		{
			_result = LoginResult.Timeout;
			return false;
		}
		if (response.SocketErrorCode != System.Net.Sockets.SocketError.Success)
		{
			_result = LoginResult.ConnectionError;
			return false;
		}
        if (response.EResult != EResult.OK)
            return false;
        _nextStep = NEXT_STEP.Poll;
        return true;
    }
    public bool PollAuthSessionStatus()
    {
        if (_nextStep != NEXT_STEP.Poll || FullyEnrolled)
            return false;
        using var memStream3 = new MemoryStream();
        Serializer.Serialize(memStream3, new AuthPollRequest()
        {
            client_id = _client_id,
            request_id = _request_id!,
        });
        string content = Convert.ToBase64String(memStream3.ToArray());
        var protoRequest = new ProtobufRequest(SteamApiUrls.IAuthenticationService_PollAuthSessionStatus_v1, content)
        {
            UserAgent = Platform == EAuthTokenPlatformType.MobileApp ? KnownUserAgents.OkHttp : KnownUserAgents.WindowsBrowser,
            Proxy = Proxy,
            CancellationToken = _cts
		};
        using var response = Downloader.PostProtobuf(protoRequest);
        LastEResult = response.EResult;
		if (response.ErrorMessage == SocketError)
		{
			_result = LoginResult.ProxyError;
			return false;
		}
		if (response.SocketErrorCode == System.Net.Sockets.SocketError.TimedOut)
		{
			_result = LoginResult.Timeout;
			return false;
		}
		if (response.SocketErrorCode != System.Net.Sockets.SocketError.Success)
		{
			_result = LoginResult.ConnectionError;
			return false;
		}
		if (response.EResult != EResult.OK)
            return false;
        var authPoll0 = Serializer.Deserialize<AuthPollResponse>(response.Stream);
        if (authPoll0.access_token.IsEmpty() || authPoll0.refresh_token.IsEmpty())
            return false;
        Session!.AccessToken = authPoll0.access_token;
        Session.RefreshToken = authPoll0.refresh_token;
        FullyEnrolled = true;
        return true;
    }

	public async Task<bool> BeginAuthSessionViaCredentialsAsync()
	{
		if (_nextStep != NEXT_STEP.Begin)
			return false;
		var usetAgent = Platform == EAuthTokenPlatformType.MobileApp ? KnownUserAgents.OkHttp : KnownUserAgents.WindowsBrowser;
		var getRequest = new GetRequest(KnownUri.BASE_POWERED)
		{
			Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application",
			UserAgent = usetAgent,
			Proxy = Proxy,
			IsMobile = Platform == EAuthTokenPlatformType.MobileApp,
			CancellationToken = _cts
		};
		var response = await Downloader.GetAsync(getRequest);
		if (response.ErrorMessage == SocketError)
		{
			_result = LoginResult.ProxyError;
			return false;
		}
		if (response.SocketErrorCode == System.Net.Sockets.SocketError.TimedOut)
		{
			_result = LoginResult.Timeout;
			return false;
		}
		if (response.SocketErrorCode != System.Net.Sockets.SocketError.Success)
		{
			_result = LoginResult.ConnectionError;
			return false;
		}
		if (response.CookieContainer == null)
		{
			if (response.Data?.Contains(UnauthorizedError) == true)
				LastEResult = EResult.RateLimitExceeded;
			_isCookieNotGet = true;
			return false;
		}

		string? steamCountry = null, browserID = null, sessionID = null;
		var cookies = response.CookieContainer.GetAllCookies();
		foreach (Cookie cookie in cookies)
		{
			if (cookie.Name == CookieSteamCountry)
				steamCountry = cookie.Value;
			else if (cookie.Name == CookieBrowserId)
				browserID = cookie.Value;
			else if (cookie.Name == CookieSessionId)
				sessionID = cookie.Value;
		}
		if (steamCountry.IsEmpty() || browserID.IsEmpty() || sessionID.IsEmpty())
		{
			_isCookieNotGet = true;
			return false;
		}
		_isCookieNotGet = false;

		using var memStream = new MemoryStream();
		Serializer.Serialize(memStream, new PasswordRSARequest() { account_name = Login });
		var tmp = Convert.ToBase64String(memStream.ToArray());
		var getRequestProto = new ProtobufRequest(SteamApiUrls.IAuthenticationService_GetPasswordRSAPublicKey_v1, tmp)
		{
			Proxy = Proxy,
			UserAgent = usetAgent,
			Cookie = KnownCookies.DefaultMobileCookie,
			CancellationToken = _cts
		};
		using var responseProto1 = await Downloader.GetProtobufAsync(getRequestProto);
		LastEResult = responseProto1.EResult;
		if (responseProto1.ErrorMessage == SocketError)
		{
			_result = LoginResult.ProxyError;
			return false;
		}
		if (responseProto1.SocketErrorCode == System.Net.Sockets.SocketError.TimedOut)
		{
			_result = LoginResult.Timeout;
			return false;
		}
		if (responseProto1.SocketErrorCode != System.Net.Sockets.SocketError.Success)
		{
			_result = LoginResult.ConnectionError;
			return false;
		}
		if (responseProto1.EResult != EResult.OK)
		{
			return false;
		}
		var rsaResponse = Serializer.Deserialize<PasswordRSAResponse>(responseProto1.Stream);
		_isRSANotGet = false;

		string encryptedPassword = Helpers.Encrypt(Password, rsaResponse.publickey_mod, rsaResponse.publickey_exp);
		using var memStream1 = new MemoryStream();
		if (Platform == EAuthTokenPlatformType.MobileApp)
		{
			var request = new AuthSessionMobileRequest()
			{
				account_name = Login,
				encrypted_password = encryptedPassword,
				encryption_timestamp = rsaResponse.timestamp,
				device_details = new()
			};
			Serializer.Serialize(memStream1, request);
		}
		else
		{
			var request = new AuthSessionDesktopRequest()
			{
				account_name = Login,
				encrypted_password = encryptedPassword,
				encryption_timestamp = rsaResponse.timestamp,
				device_details = new()
				{
					device_friendly_name = usetAgent,
					platform_type = Platform
				}
			};
			Serializer.Serialize(memStream1, request);
		}
		string content = Convert.ToBase64String(memStream1.ToArray());
		var stringCookies = new StringBuilder(9);
		stringCookies.Append(KnownCookies.DefaultMobileCookie);
		stringCookies.Append("steamCountry=");
		stringCookies.Append(steamCountry);
		stringCookies.Append("; browserid=");
		stringCookies.Append(browserID);
		stringCookies.Append("; sessionid=");
		stringCookies.Append(sessionID);
		stringCookies.Append("; ");
		var postRequestProto = new ProtobufRequest(SteamApiUrls.IAuthenticationService_BeginAuthSessionViaCredentials_v1, content)
		{
			Proxy = Proxy,
			UserAgent = usetAgent,
			Cookie = stringCookies.ToString(),
			CancellationToken = _cts
		};
		using var responseProto2 = await Downloader.PostProtobufAsync(postRequestProto);
		LastEResult = responseProto2.EResult;
		if (responseProto2.ErrorMessage == SocketError)
		{
			_result = LoginResult.ProxyError;
			return false;
		}
		if (responseProto2.SocketErrorCode == System.Net.Sockets.SocketError.TimedOut)
		{
			_result = LoginResult.Timeout;
			return false;
		}
		if (responseProto2.SocketErrorCode != System.Net.Sockets.SocketError.Success)
		{
			_result = LoginResult.ConnectionError;
			return false;
		}
		if (responseProto2.EResult != EResult.OK)
		{
			return false;
		}
		var authSession = Serializer.Deserialize<AuthSessionResponse>(responseProto2.Stream);

		SteamID64 = authSession.steamid;
		Session = new()
		{
			SteamID = authSession.steamid,
			BrowserId = browserID!,
			SessionID = sessionID!,
			SteamCountry = steamCountry!,
			PlatformType = Platform,
		};
		_client_id = authSession.client_id;
		_request_id = authSession.request_id;
		WeakToken = authSession.weak_token;

		var length = authSession.allowed_confirmations.Length;
		Approve = new EAuthSessionGuardType[length];
		for (int i = 0; i < length; i++)
		{
			var item = authSession.allowed_confirmations[i].message;
			Approve[i] = item;
			if (item == EAuthSessionGuardType.None)
				continue;
			if (item == EAuthSessionGuardType.Unknown)
				continue;
			if (item == EAuthSessionGuardType.EmailCode)
				_isNeedEmailCode = true;
			else if (item == EAuthSessionGuardType.DeviceCode)
				_isNeedTwoFactorCode = true;
			else
				_isNeedConfirm = true;
		}
		if (_isNeedConfirm || _isNeedTwoFactorCode || _isNeedEmailCode)
			_nextStep = NEXT_STEP.Update;
		else
			_nextStep = NEXT_STEP.Poll;
		return true;
	}
	public async Task<bool> UpdateAuthSessionWithSteamGuardCodeAsync(string? fa2Code)
	{
		if (fa2Code.IsEmpty())
		{
			if (Data.IsEmpty())
				return false;
			fa2Code = Data!;
		}
		if (!IsNeedEmailCode && !IsNeedTwoFactorCode)
			return false;
		using var memStream2 = new MemoryStream();
		Serializer.Serialize(memStream2, new AuthGuardRequest()
		{
			client_id = _client_id,
			code = fa2Code,
			code_type = IsNeedEmailCode ? EAuthSessionGuardType.EmailCode : EAuthSessionGuardType.DeviceCode,
			steamid = Session!.SteamID
		});
		string content = Convert.ToBase64String(memStream2.ToArray());
		var protoRequest = new ProtobufRequest(SteamApiUrls.IAuthenticationService_UpdateAuthSessionWithSteamGuardCode_v1, content)
		{
			UserAgent = Platform == EAuthTokenPlatformType.MobileApp ? KnownUserAgents.OkHttp : KnownUserAgents.WindowsBrowser,
			Proxy = Proxy,
			CancellationToken = _cts
		};
		using var response = await Downloader.PostProtobufAsync(protoRequest);
		LastEResult = response.EResult;
		if (response.ErrorMessage == SocketError)
		{
			_result = LoginResult.ProxyError;
			return false;
		}
		if (response.SocketErrorCode == System.Net.Sockets.SocketError.TimedOut)
		{
			_result = LoginResult.Timeout;
			return false;
		}
		if (response.SocketErrorCode != System.Net.Sockets.SocketError.Success)
		{
			_result = LoginResult.ConnectionError;
			return false;
		}
		if (response.EResult != EResult.OK)
			return false;
		_nextStep = NEXT_STEP.Poll;
		return true;
	}
	public async Task<bool> PollAuthSessionStatusAsync()
	{
		if (_nextStep != NEXT_STEP.Poll || FullyEnrolled)
			return false;
		using var memStream3 = new MemoryStream();
		Serializer.Serialize(memStream3, new AuthPollRequest()
		{
			client_id = _client_id,
			request_id = _request_id!,
		});
		string content = Convert.ToBase64String(memStream3.ToArray());
		var protoRequest = new ProtobufRequest(SteamApiUrls.IAuthenticationService_PollAuthSessionStatus_v1, content)
		{
			UserAgent = Platform == EAuthTokenPlatformType.MobileApp ? KnownUserAgents.OkHttp : KnownUserAgents.WindowsBrowser,
			Proxy = Proxy,
			CancellationToken = _cts
		};
		using var response = await Downloader.PostProtobufAsync(protoRequest);
		LastEResult = response.EResult;
		if (response.ErrorMessage == SocketError)
		{
			_result = LoginResult.ProxyError;
			return false;
		}
		if (response.SocketErrorCode == System.Net.Sockets.SocketError.TimedOut)
		{
			_result = LoginResult.Timeout;
			return false;
		}
		if (response.SocketErrorCode != System.Net.Sockets.SocketError.Success)
		{
			_result = LoginResult.ConnectionError;
			return false;
		}
		if (response.EResult != EResult.OK)
			return false;
		var authPoll0 = Serializer.Deserialize<AuthPollResponse>(response.Stream);
		if (authPoll0.access_token.IsEmpty() || authPoll0.refresh_token.IsEmpty())
			return false;
		Session!.AccessToken = authPoll0.access_token;
		Session.RefreshToken = authPoll0.refresh_token;
		FullyEnrolled = true;
		return true;
	}
}
