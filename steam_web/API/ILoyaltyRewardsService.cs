using SteamWeb.API.Models;
using SteamWeb.Web;
using ProtoBuf;
using SteamWeb.API.Protobufs;

namespace SteamWeb.API;
public static class ILoyaltyRewardsService
{
    public static CLoyaltyRewards_GetSummary_Response? GetSummary(ApiRequest apiRequest, CLoyaltyRewards_GetSummary_Request proto)
	{
		using var ms = new MemoryStream();
		Serializer.Serialize(ms, proto);
		var base64 = Convert.ToBase64String(ms.ToArray());

		var request = new ProtobufRequest(SteamApiUrls.ILoyaltyRewardsService_GetSummary_v1, base64)
        {
			AccessToken = apiRequest.AuthToken,
            Proxy = apiRequest.Proxy,
            CancellationToken = apiRequest.CancellationToken,
            UserAgent = KnownUserAgents.OkHttp,
			SpoofSteamId = 0,
        };
        using var response = Downloader.GetProtobuf(request);
		if (!response.Success)
			return null;
		if (response.EResult != EResult.OK)
			return null;

		try
		{
			var obj = Serializer.Deserialize<CLoyaltyRewards_GetSummary_Response>(response.Stream!);
			return obj;
		}
		catch (Exception)
		{
			return null;
		}
	}
	public static async Task<CLoyaltyRewards_GetSummary_Response?> GetSummaryAsync(ApiRequest apiRequest, CLoyaltyRewards_GetSummary_Request proto)
	{
		using var ms = new MemoryStream();
		Serializer.Serialize(ms, proto);
		var base64 = Convert.ToBase64String(ms.ToArray());

		var request = new ProtobufRequest(SteamApiUrls.ILoyaltyRewardsService_GetSummary_v1, base64)
		{
			AccessToken = apiRequest.AuthToken,
			Proxy = apiRequest.Proxy,
			CancellationToken = apiRequest.CancellationToken,
			UserAgent = KnownUserAgents.OkHttp,
			SpoofSteamId = 0,
		};
		using var response = await Downloader.GetProtobufAsync(request);
		if (!response.Success)
			return null;
		if (response.EResult != EResult.OK)
			return null;

		try
		{
			var obj = Serializer.Deserialize<CLoyaltyRewards_GetSummary_Response>(response.Stream!);
			return obj;
		}
		catch (Exception)
		{
			return null;
		}
	}

	public static EResult AddReaction(ApiRequest apiRequest, CLoyaltyRewards_AddReaction_Request proto)
	{
		using var ms = new MemoryStream();
		Serializer.Serialize(ms, proto);
		var base64 = Convert.ToBase64String(ms.ToArray());

		var request = new ProtobufRequest(SteamApiUrls.ILoyaltyRewardsService_AddReaction_v1, base64)
		{
			AccessToken = apiRequest.AuthToken,
			CancellationToken = apiRequest.CancellationToken,
			Proxy = apiRequest.Proxy,
			UserAgent = KnownUserAgents.WindowsBrowser,
			SpoofSteamId = 0,
		};
		using var response = Downloader.PostProtobuf(request);
		return response.EResult;
	}
	public static async Task<EResult> AddReactionAsync(ApiRequest apiRequest, CLoyaltyRewards_AddReaction_Request proto)
	{
		using var ms = new MemoryStream();
		Serializer.Serialize(ms, proto);
		var base64 = Convert.ToBase64String(ms.ToArray());

		var request = new ProtobufRequest(SteamApiUrls.ILoyaltyRewardsService_AddReaction_v1, base64)
		{
			AccessToken = apiRequest.AuthToken,
			CancellationToken = apiRequest.CancellationToken,
			Proxy = apiRequest.Proxy,
			UserAgent = KnownUserAgents.WindowsBrowser,
			SpoofSteamId = 0,
		};
		using var response = await Downloader.PostProtobufAsync(request);
		return response.EResult;
	}

	/// <summary>
	/// Выполняеть запрос на получение доступных реакций.
	/// <para/>
	/// Для отправки запроса нужен ключ из <see cref="Script.Ajax.pointssummary_ajaxgetasyncconfig"/>
	/// </summary>
	/// <param name="target_type">1 - Review (обзор); 2 - UGC; 3 - профиль; 4 - ветка на форуме; 5 - комментарий</param>
	/// <param name="targetid">id того, где нужно поставить реакцию</param>
	/// <returns>Возвращает доступные реакции</returns>
	public static CLoyaltyRewards_GetReactionConfig_Response? GetReactionConfig(ApiRequest apiRequest)
	{
		var request = new ProtobufRequest(SteamApiUrls.ILoyaltyRewardsService_GetReactionConfig_v1, string.Empty)
		{
			CancellationToken= apiRequest.CancellationToken,
			Proxy = apiRequest.Proxy,
			UserAgent= KnownUserAgents.WindowsBrowser,
		};
		using var response = Downloader.GetProtobuf(request);
		if (!response.Success)
			return null;
		if (response.EResult != EResult.OK)
			return null;

		try
		{
			var obj = Serializer.Deserialize<CLoyaltyRewards_GetReactionConfig_Response>(response.Stream!);
			return obj;
		}
		catch (Exception)
		{
			return null;
		}
	}
	/// <summary>
	/// Выполняеть запрос на получение доступных реакций.
	/// <para/>
	/// Для отправки запроса нужен ключ из <see cref="Script.Ajax.pointssummary_ajaxgetasyncconfig"/>
	/// </summary>
	/// <param name="target_type">1 - Review (обзор); 2 - UGC; 3 - профиль; 4 - ветка на форуме; 5 - комментарий</param>
	/// <param name="targetid">id того, где нужно поставить реакцию</param>
	/// <returns>Возвращает доступные реакции</returns>
	public static async Task<CLoyaltyRewards_GetReactionConfig_Response?> GetReactionConfigAsync(ApiRequest apiRequest)
	{
		var request = new ProtobufRequest(SteamApiUrls.ILoyaltyRewardsService_GetReactionConfig_v1, string.Empty)
		{
			CancellationToken = apiRequest.CancellationToken,
			Proxy = apiRequest.Proxy,
			UserAgent = KnownUserAgents.WindowsBrowser,
		};
		using var response = await Downloader.GetProtobufAsync(request);
		if (!response.Success)
			return null;
		if (response.EResult != EResult.OK)
			return null;

		try
		{
			var obj = Serializer.Deserialize<CLoyaltyRewards_GetReactionConfig_Response>(response.Stream!);
			return obj;
		}
		catch (Exception)
		{
			return null;
		}
	}
}