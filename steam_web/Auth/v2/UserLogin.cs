using System.Security.Cryptography;
using System.Text;
using ProtoBuf;
using SteamWeb.Auth.v2.Enums;
using SteamWeb.Auth.v2.Models;
using SteamWeb.Extensions;
using SteamWeb.Web;
using Util = SteamWeb.Auth.v1.Util;

namespace SteamWeb.Auth.v2;
public class UserLogin
{
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

    private bool _isNeedTwoFactorCode = false;
    private bool _isNeedEmailCode = false;
    private bool _isNeedConfirm = false;
    private NEXT_STEP _nextStep = NEXT_STEP.Begin;
    private readonly System.Net.IWebProxy? _proxy = null;
    private byte[]? _request_id = null;
    private ulong _client_id = 0;
    private readonly EAuthTokenPlatformType _platform;
    private bool? _isCookieNotGet = null;
    private bool? _isRSANotGet = null;
    private LoginResult _result = LoginResult.GeneralFailure;
	private CancellationToken? _cts = null;

	public UserLogin(string login, string passwd, EAuthTokenPlatformType platform)
    {
        Login = login;
        Password = passwd;
        _platform = platform;
    }
    public UserLogin(string login, string passwd, EAuthTokenPlatformType platform, System.Net.IWebProxy proxy) : this(login, passwd, platform) => _proxy = proxy;
	public UserLogin(string login, string passwd, EAuthTokenPlatformType platform, System.Net.IWebProxy proxy, CancellationToken? cts) :
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
            LastEResult == EResult.PhoneActivityLimitExceeded)
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
        var usetAgent = _platform == EAuthTokenPlatformType.MobileApp ? Downloader.UserAgentOkHttp : SessionData.UserAgentBrowser;
        var getRequest = new GetRequest("https://store.steampowered.com/")
        {
            UserAgent = usetAgent,
            Proxy = _proxy,
            IsMobile = _platform == EAuthTokenPlatformType.MobileApp
        };
        var response = Downloader.Get(getRequest);
        if (response.Cookie == null)
        {
            if (response.Data?.Contains("You don't have permission to access \"http&#58;&#47;&#47;store&#46;steampowered&#46;com&#47;\" on this server.") == true)
                LastEResult = EResult.RateLimitExceeded;
            _isCookieNotGet = true;
            return false;
        }
        string? steamCountry = null, browserID = null, sessionID = null;
        foreach (var item in response.Cookie.Split("; "))
        {
            var splitted = item.Split('=');
            if (splitted.Length != 2)
                continue;
            if (splitted[0] == "steamCountry")
                steamCountry = splitted[1];
            else if (splitted[0] == "browserid")
                browserID = splitted[1];
            else if (splitted[0] == "sessionid")
                sessionID = splitted[1];
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
        memStream.Close();
        memStream.Dispose();
        var getRequestProto = new ProtobufRequest("https://api.steampowered.com/IAuthenticationService/GetPasswordRSAPublicKey/v1", tmp)
        {
            Proxy = _proxy,
            UserAgent = usetAgent,
            Cookie = SessionData.DefaultMobileCookie
        };
        using var responseProto1 = Downloader.GetProtobuf(getRequestProto);
        LastEResult = responseProto1.EResult;
        if (responseProto1.EResult != EResult.OK)
            return false;
        var rsaResponse = Serializer.Deserialize<PasswordRSAResponse>(responseProto1.Stream);
        _isRSANotGet = false;
        responseProto1.Stream.Close();
        responseProto1.Stream.Dispose();

        byte[] encryptedPasswordBytes;
        using var rsaEncryptor = new RSACryptoServiceProvider();
        var passwordBytes = Encoding.ASCII.GetBytes(Password);
        var rsaParameters = rsaEncryptor.ExportParameters(false);
        rsaParameters.Exponent = Util.HexStringToByteArray(rsaResponse.publickey_exp);
        rsaParameters.Modulus = Util.HexStringToByteArray(rsaResponse.publickey_mod);
        rsaEncryptor.ImportParameters(rsaParameters);
        encryptedPasswordBytes = rsaEncryptor.Encrypt(passwordBytes, false);
        string encryptedPassword = Convert.ToBase64String(encryptedPasswordBytes, Base64FormattingOptions.None);

        using var memStream1 = new MemoryStream();
        dynamic request = _platform == EAuthTokenPlatformType.MobileApp ? new AuthSessionMobileRequest()
        {
            account_name = Login,
            encrypted_password = Helpers.Encrypt(Password, rsaResponse.publickey_mod, rsaResponse.publickey_exp),
            encryption_timestamp = rsaResponse.timestamp,
            device_details = new()
        } : new AuthSessionDesktopRequest()
        {
            account_name = Login,
            encrypted_password = encryptedPassword,
            encryption_timestamp = rsaResponse.timestamp,
            device_details = new()
            {
                device_friendly_name = SessionData.UserAgentBrowser,
                platform_type = _platform
            }
        };
        Serializer.Serialize(memStream1, request);
        string content = Convert.ToBase64String(memStream1.ToArray());
        memStream1.Close();
        memStream1.Dispose();
        var postRequestProto = new ProtobufRequest("https://api.steampowered.com/IAuthenticationService/BeginAuthSessionViaCredentials/v1", content)
        {
            Proxy = _proxy,
            UserAgent = usetAgent,
            Cookie = $"{SessionData.DefaultMobileCookie}steamCountry={steamCountry}; browserid={browserID}; sessionid={sessionID}; "
        };
        using var responseProto2 = Downloader.PostProtobuf(postRequestProto);
        LastEResult = responseProto2.EResult;
        if (responseProto2.EResult != EResult.OK)
            return false;
        var authSession = Serializer.Deserialize<AuthSessionResponse>(responseProto2.Stream);
        responseProto2.Stream.Close();
        responseProto2.Stream.Dispose();

        SteamID64 = authSession.steamid;
        Session = new()
        {
            SteamID = authSession.steamid,
            BrowserId = browserID!,
            SessionID = sessionID!,
            SteamCountry = steamCountry!,
            PlatformType = _platform,
        };
        _client_id = authSession.client_id;
        _request_id = authSession.request_id;

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
    public bool UpdateAuthSessionWithSteamGuardCode(string fa2Code)
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
            steamid = Session.SteamID
        });
        string content = Convert.ToBase64String(memStream2.ToArray());
        memStream2.Close();
        memStream2!.Dispose();
        var protoRequest = new ProtobufRequest("https://api.steampowered.com/IAuthenticationService/UpdateAuthSessionWithSteamGuardCode/v1", content)
        {
            UserAgent = _platform == EAuthTokenPlatformType.MobileApp ? Downloader.UserAgentSteamMobileApp : SessionData.UserAgentBrowser,
            Proxy = _proxy
        };
        using var response = Downloader.PostProtobuf(protoRequest);
        LastEResult = response.EResult;
        if (response.Stream != null)
        {
            response.Stream.Close();
            response.Stream.Dispose();
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
        memStream3.Close();
        memStream3.Dispose();
        var protoRequest = new ProtobufRequest("https://api.steampowered.com/IAuthenticationService/PollAuthSessionStatus/v1", content)
        {
            UserAgent = _platform == EAuthTokenPlatformType.MobileApp ? Downloader.UserAgentSteamMobileApp : SessionData.UserAgentBrowser,
            Proxy = _proxy
        };
        using var response = Downloader.PostProtobuf(protoRequest);
        LastEResult = response.EResult;
        if (response.EResult != EResult.OK)
            return false;
        var authPoll0 = Serializer.Deserialize<AuthPollResponse>(response.Stream);
        response.Stream.Close();
        response.Stream.Dispose();
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
        var usetAgent = _platform == EAuthTokenPlatformType.MobileApp ? Downloader.UserAgentSteamMobileApp : SessionData.UserAgentBrowser;
        var getRequest = new GetRequest("https://store.steampowered.com/")
        {
            UserAgent = usetAgent,
            Proxy = _proxy,
            IsMobile = _platform == EAuthTokenPlatformType.MobileApp
        };
        var response = await Downloader.GetAsync(getRequest);
        if (response.Cookie == null)
        {
            if (response.Data?.Contains("You don't have permission to access \"http&#58;&#47;&#47;store&#46;steampowered&#46;com&#47;\" on this server.") == true)
                LastEResult = EResult.RateLimitExceeded;
            _isCookieNotGet = true;
            return false;
        }
        string? steamCountry = null, browserID = null, sessionID = null;
        foreach (var item in response.Cookie.Split("; "))
        {
            var splitted = item.Split('=');
            if (splitted.Length != 2)
                continue;
            if (splitted[0] == "steamCountry")
                steamCountry = splitted[1];
            else if (splitted[0] == "browserid")
                browserID = splitted[1];
            else if (splitted[0] == "sessionid")
                sessionID = splitted[1];
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
        memStream.Close();
        memStream.Dispose();
        var getRequestProto = new ProtobufRequest("https://api.steampowered.com/IAuthenticationService/GetPasswordRSAPublicKey/v1", tmp)
        {
            Proxy = _proxy,
            UserAgent = usetAgent
        };
        var responseProto = await Downloader.GetProtobufAsync(getRequestProto);
        LastEResult = responseProto.EResult;
        if (responseProto.EResult != EResult.OK)
            return false;
        var rsaResponse = Serializer.Deserialize<PasswordRSAResponse>(responseProto.Stream);
        _isRSANotGet = false;
        responseProto.Stream.Close();
        responseProto.Stream.Dispose();

        byte[] encryptedPasswordBytes;
        using var rsaEncryptor = new RSACryptoServiceProvider(2048);
        var passwordBytes = Encoding.ASCII.GetBytes(Password);
        var rsaParameters = rsaEncryptor.ExportParameters(false);
        rsaParameters.Exponent = Util.HexStringToByteArray(rsaResponse.publickey_exp);
        rsaParameters.Modulus = Util.HexStringToByteArray(rsaResponse.publickey_mod);
        rsaEncryptor.ImportParameters(rsaParameters);
        encryptedPasswordBytes = rsaEncryptor.Encrypt(passwordBytes, false);
        string encryptedPassword = Convert.ToBase64String(encryptedPasswordBytes, Base64FormattingOptions.None);

        using var memStream1 = new MemoryStream();
        dynamic request = _platform == EAuthTokenPlatformType.MobileApp ? new AuthSessionMobileRequest()
        {
            account_name = Login,
            encrypted_password = encryptedPassword,
            encryption_timestamp = rsaResponse.timestamp,
            device_details = new()
        } : new AuthSessionDesktopRequest()
        {
            account_name = Login,
            encrypted_password = encryptedPassword,
            encryption_timestamp = rsaResponse.timestamp,
            device_details = new()
            {
                device_friendly_name = SessionData.UserAgentBrowser,
                platform_type = _platform
            }
        };
        Serializer.Serialize(memStream1, request);
        string content = Convert.ToBase64String(memStream1.ToArray());
        memStream1.Close();
        memStream1.Dispose();
        var postRequestProto = new ProtobufRequest("https://api.steampowered.com/IAuthenticationService/BeginAuthSessionViaCredentials/v1", content)
        {
            Proxy = _proxy,
            UserAgent = usetAgent,
            Cookie = $"steamCountry={steamCountry}; browserid={browserID}; sessionid={sessionID}"
        };
        responseProto = await Downloader.PostProtobufAsync(postRequestProto);
        LastEResult = responseProto.EResult;
        var authSession = responseProto.Stream != null ? Serializer.Deserialize<AuthSessionResponse>(responseProto.Stream) : null;
        if (responseProto.EResult != EResult.OK)
            return false;
        responseProto.Stream!.Close();
        responseProto.Stream.Dispose();

        SteamID64 = authSession!.steamid;
        Session = new()
        {
            SteamID = authSession.steamid,
            BrowserId = browserID!,
            SessionID = sessionID!,
            SteamCountry = steamCountry!,
            PlatformType = _platform
        };
        _client_id = authSession.client_id;
        _request_id = authSession.request_id;

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
        if (_isNeedConfirm || _isNeedTwoFactorCode || _isNeedEmailCode) _nextStep = NEXT_STEP.Update;
        else _nextStep = NEXT_STEP.Poll;
        return true;
    }
    public async Task<bool> UpdateAuthSessionWithSteamGuardCodeAsync(string fa2Code)
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
            steamid = Session.SteamID
        });
        string content = Convert.ToBase64String(memStream2.ToArray());
        memStream2.Close();
        memStream2!.Dispose();
        var protoRequest = new ProtobufRequest("https://api.steampowered.com/IAuthenticationService/UpdateAuthSessionWithSteamGuardCode/v1", content)
        {
            UserAgent = _platform == EAuthTokenPlatformType.MobileApp ? Downloader.UserAgentSteamMobileApp : SessionData.UserAgentBrowser,
            Proxy = _proxy
        };
        var response = await Downloader.PostProtobufAsync(protoRequest);
        LastEResult = response.EResult;
        if (response.Stream != null)
        {
            response.Stream.Close();
            response.Stream.Dispose();
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
        memStream3.Close();
        memStream3!.Dispose();
        var protoRequest = new ProtobufRequest("https://api.steampowered.com/IAuthenticationService/PollAuthSessionStatus/v1", content)
        {
            UserAgent = _platform == EAuthTokenPlatformType.MobileApp ? Downloader.UserAgentSteamMobileApp : SessionData.UserAgentBrowser,
            Proxy = _proxy
        };
        var response = await Downloader.PostProtobufAsync(protoRequest);
        LastEResult = response.EResult;
        if (response.EResult != EResult.OK)
            return false;
        var authPoll0 = Serializer.Deserialize<AuthPollResponse>(response.Stream);
        response.Stream.Close();
        response.Stream.Dispose();
        if (authPoll0.access_token.IsEmpty() || authPoll0.refresh_token.IsEmpty())
            return false;
        Session!.AccessToken = authPoll0.access_token;
        Session.RefreshToken = authPoll0.refresh_token;
        FullyEnrolled = true;
        return true;
    }
}
