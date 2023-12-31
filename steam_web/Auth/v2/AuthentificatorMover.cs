using System.Net;
using ProtoBuf;
using SteamWeb.Auth.v2.DTO;
using SteamWeb.Auth.v2.Enums;
using SteamWeb.Auth.v2.Models;
using SteamWeb.Extensions;
using SteamWeb.Web;

namespace SteamWeb.Auth.v2;
public class AuthentificatorMover
{
	private readonly IWebProxy? _proxy;
	private readonly UserLogin _userLogin;

	public EResult LastEResult { get; private set; } = EResult.OK;

	/// <summary>
	/// Создаёт новый экземпляр класса для переноса аутентификатора.
	/// <para/>
	/// Для правильного переноса нужно начать мобильную авторизацию и на этапе <see cref="UserLogin.PollAuthSessionStatus"/> начать перенос.
	/// </summary>
	/// 
	/// <param name="userLogin">
	/// Свойство <see cref="UserLogin.NextStep"/> должно быть <see cref="NEXT_STEP.Poll"/>
	/// <para/>
	/// Свойство <see cref="UserLogin.LastEResult"/> должно быть <see cref="EResult.OK"/>
	/// <para/>
	/// Свойство <see cref="UserLogin.Platform"/> должно быть <see cref="EAuthTokenPlatformType.MobileApp"/>
	/// <para/>
	/// Свойство <see cref="UserLogin.WeakToken"/> не должно быть пустым
	/// </param>
	/// 
	/// <exception cref="ArgumentException"/>
	public AuthentificatorMover(UserLogin userLogin)
	{
		if (userLogin.LastEResult != EResult.OK)
			throw new ArgumentException("Последний EResult не OK", nameof(userLogin));

		if (userLogin.NextStep != NEXT_STEP.Poll)
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
			UserAgent = SessionData.UserAgentMobileApp
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
	public CTwoFactor_RemoveAuthenticatorViaChallengeContinue_Response RemoveAuthenticatorViaChallengeContinue(CTwoFactor_RemoveAuthenticatorViaChallengeContinue_Request request)
	{
		using var memStream1 = new MemoryStream();
		Serializer.Serialize(memStream1, request);
		var req = new ProtobufRequest(SteamApiUrls.ITwoFactorService_RemoveAuthenticatorViaChallengeContinue_v1, Convert.ToBase64String(memStream1.ToArray()))
		{
			AccessToken = _userLogin.WeakToken,
			Proxy = _proxy,
			UserAgent = SessionData.UserAgentMobileApp
		};
		using var response = Downloader.PostProtobuf(req);
		LastEResult = response.EResult;
		if (!response.Success || response.EResult != EResult.OK)
			return new();
		var obj = Serializer.Deserialize<CTwoFactor_RemoveAuthenticatorViaChallengeContinue_Response>(response.Stream);
		return obj;
	}
}