using System.Net;
using ProtoBuf;
using SteamWeb.Auth.v2.DTO;
using SteamWeb.Auth.v2.Enums;
using SteamWeb.Extensions;
using SteamWeb.Web;
using SDAv1 = SteamWeb.Auth.v1.SteamGuardAccount;
using SDAv2 = SteamWeb.Auth.v2.SteamGuardAccount;

namespace SteamWeb.Auth.v2;
public class AuthenticatorMover
{
	private readonly IWebProxy? _proxy;
	private readonly UserLogin _userLogin;
	private CRemoveAuthenticatorViaChallengeContinue_Replacement_Token? _token = null;
	private readonly string _deviceId = AuthenticatorLinker.GenerateDeviceID();

	public EResult LastEResult { get; private set; } = EResult.OK;

	/// <summary>
	/// Создаёт новый экземпляр класса для переноса аутентификатора.
	/// <para/>
	/// Для правильного переноса нужно начать мобильную авторизацию и на этапе <see cref="UserLogin.PollAuthSessionStatus"/> начать перенос.
	/// </summary>
	/// 
	/// <param name="userLogin">
	/// Свойство <see cref="UserLogin.NextStep"/> должно быть <see cref="NEXT_STEP.Update"/>
	/// <para/>
	/// Свойство <see cref="UserLogin.LastEResult"/> должно быть <see cref="EResult.OK"/>
	/// <para/>
	/// Свойство <see cref="UserLogin.Platform"/> должно быть <see cref="EAuthTokenPlatformType.MobileApp"/>
	/// <para/>
	/// Свойство <see cref="UserLogin.WeakToken"/> не должно быть пустым
	/// </param>
	/// 
	/// <exception cref="ArgumentException"/>
	public AuthenticatorMover(UserLogin userLogin)
	{
		if (userLogin.LastEResult != EResult.OK)
			throw new ArgumentException("Последний EResult не OK", nameof(userLogin));

		if (userLogin.NextStep != NEXT_STEP.Update)
			throw new ArgumentException("Следующий шаг должен быть Poll", nameof(userLogin));

		if (userLogin.WeakToken.IsEmpty())
			throw new ArgumentException("Не указан weak_token", nameof(userLogin));

		_userLogin = userLogin;
		_proxy = userLogin.Proxy;
	}

	public CTwoFactor_RemoveAuthenticatorViaChallengeStart_Response RemoveAuthenticatorViaChallengeStart()
	{
		var request = new ProtobufRequest(SteamApiUrls.ITwoFactorService_RemoveAuthenticatorViaChallengeStart_v1, string.Empty)
		{
			AccessToken = _userLogin.WeakToken,
			Proxy = _proxy,
			UserAgent = KnownUserAgents.OkHttp
		};
		using var response = Downloader.PostProtobuf(request);
		LastEResult = response.EResult;
		if (!response.Success || response.EResult != EResult.OK)
			return new();
		if (response.EResult == EResult.OK)
			return new() { Success = true };
		var obj = Serializer.Deserialize<CTwoFactor_RemoveAuthenticatorViaChallengeStart_Response>(response.Stream);
		return obj;
	}
	public async Task<CTwoFactor_RemoveAuthenticatorViaChallengeStart_Response> RemoveAuthenticatorViaChallengeStartAsync()
	{
		var request = new ProtobufRequest(SteamApiUrls.ITwoFactorService_RemoveAuthenticatorViaChallengeStart_v1, string.Empty)
		{
			AccessToken = _userLogin.WeakToken,
			Proxy = _proxy,
			UserAgent = KnownUserAgents.OkHttp
		};
		using var response = await Downloader.PostProtobufAsync(request);
		LastEResult = response.EResult;
		if (!response.Success || response.EResult != EResult.OK)
			return new();
		if (response.EResult == EResult.OK)
			return new() { Success = true };
		var obj = Serializer.Deserialize<CTwoFactor_RemoveAuthenticatorViaChallengeStart_Response>(response.Stream);
		return obj;
	}

	public CTwoFactor_RemoveAuthenticatorViaChallengeContinue_Response RemoveAuthenticatorViaChallengeContinue(CTwoFactor_RemoveAuthenticatorViaChallengeContinue_Request request)
	{
		using var memStream1 = new MemoryStream();
		Serializer.Serialize(memStream1, request);
		var req = new ProtobufRequest(SteamApiUrls.ITwoFactorService_RemoveAuthenticatorViaChallengeContinue_v1, Convert.ToBase64String(memStream1.ToArray()))
		{
			AccessToken = _userLogin.WeakToken,
			Proxy = _proxy,
			UserAgent = KnownUserAgents.OkHttp
		};
		using var response = Downloader.PostProtobuf(req);
		LastEResult = response.EResult;
		if (!response.Success || response.EResult != EResult.OK)
			return new();
		var obj = Serializer.Deserialize<CTwoFactor_RemoveAuthenticatorViaChallengeContinue_Response>(response.Stream);
		if (obj.Success)
			_token = obj.replacement_token;
		return obj;
	}
	public async Task<CTwoFactor_RemoveAuthenticatorViaChallengeContinue_Response> RemoveAuthenticatorViaChallengeContinueAsync(CTwoFactor_RemoveAuthenticatorViaChallengeContinue_Request request)
	{
		using var memStream1 = new MemoryStream();
		Serializer.Serialize(memStream1, request);
		var req = new ProtobufRequest(SteamApiUrls.ITwoFactorService_RemoveAuthenticatorViaChallengeContinue_v1, Convert.ToBase64String(memStream1.ToArray()))
		{
			AccessToken = _userLogin.WeakToken,
			Proxy = _proxy,
			UserAgent = KnownUserAgents.OkHttp
		};
		using var response = await Downloader.PostProtobufAsync(req);
		LastEResult = response.EResult;
		if (!response.Success || response.EResult != EResult.OK)
			return new();
		var obj = Serializer.Deserialize<CTwoFactor_RemoveAuthenticatorViaChallengeContinue_Response>(response.Stream);
		if (obj.Success)
			_token = obj.replacement_token;
		return obj;
	}

	public SDAv1? GetSteamGuardv1()
	{
		if (_token == null)
			return null;
		var sda = new SDAv1
		{
			Session = null,
			AccountName = _token.account_name,
			DeviceID = _deviceId,
			FullyEnrolled = true,
			IdentitySecret = Convert.ToBase64String(_token.identity_secret),
			RevocationCode = _token.revocation_code,
			Secret1 = Convert.ToBase64String(_token.secret_1),
			SerialNumber = _token.serial_number.ToString(),
			ServerTime = _token.server_time,
			SharedSecret = Convert.ToBase64String(_token.shared_secret),
			Status = _token.status,
			TokenGID = _token.token_gid,
			URI = _token.uri,
		};
		return sda;
	}
	public SDAv2? GetSteamGuardv2()
	{
		if (_token == null)
			return null;
		var sda = new SDAv2
		{
			Session = null,
			AccountName = _token.account_name,
			DeviceID = _deviceId,
			FullyEnrolled = true,
			IdentitySecret = Convert.ToBase64String(_token.identity_secret),
			RevocationCode = _token.revocation_code,
			Secret1 = Convert.ToBase64String(_token.secret_1),
			SerialNumber = _token.serial_number,
			ServerTime = _token.server_time,
			SharedSecret = Convert.ToBase64String(_token.shared_secret),
			Status = _token.status,
			TokenGID = _token.token_gid,
			URI = _token.uri,
		};
		return sda;
	}
}