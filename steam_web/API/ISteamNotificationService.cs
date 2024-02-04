﻿using ProtoBuf;
using SteamWeb.API.Protobufs;
using SteamWeb.Auth.v2.Models;
using SteamWeb.Web;

namespace SteamWeb.API;
public static class ISteamNotificationService
{
	/// <summary>
	/// Получение уведомлений аккаунта
	/// </summary>
	/// <param name="session">Сессия аккаунта</param>
	/// <param name="proxy">Прокси, если необходим</param>
	/// <param name="request">Запрос</param>
	/// <returns>Null если запрос не выполнен, в других случаях ответ на запрос</returns>
	public static CSteamNotification_GetSteamNotifications_Response? GetSteamNotifications(SessionData session, Proxy? proxy, CSteamNotification_GetSteamNotifications_Request request)
	{
		using var memStream1 = new MemoryStream();
		Serializer.Serialize(memStream1, request);
		var base64 = Convert.ToBase64String(memStream1.ToArray());
		var protobufRequest = new ProtobufRequest(SteamPoweredUrls.ISteamNotificationService_GetSteamNotifications_v1, base64)
		{
			UserAgent = KnownUserAgents.OkHttp,
			Proxy = proxy,
			AccessToken = session.AccessToken,
			IsMobile = true
		};
		using var protobufResponse = Downloader.GetProtobuf(protobufRequest);
		if (protobufResponse.EResult != EResult.OK)
			return null;
		var obj = Serializer.Deserialize<CSteamNotification_GetSteamNotifications_Response>(protobufResponse.Stream);
		return obj;
	}
	/// <summary>
	/// Получение уведомлений аккаунта
	/// </summary>
	/// <param name="session">Сессия аккаунта</param>
	/// <param name="proxy">Прокси, если необходим</param>
	/// <param name="request">Запрос</param>
	/// <returns>Null если запрос не выполнен, в других случаях ответ на запрос</returns>
	public static async Task<CSteamNotification_GetSteamNotifications_Response?> GetSteamNotificationsAsync(SessionData session, Proxy? proxy, CSteamNotification_GetSteamNotifications_Request request)
	{
		using var memStream1 = new MemoryStream();
		Serializer.Serialize(memStream1, request);
		var base64 = Convert.ToBase64String(memStream1.ToArray());
		var protobufRequest = new ProtobufRequest(SteamPoweredUrls.ISteamNotificationService_GetSteamNotifications_v1, base64)
		{
			UserAgent = KnownUserAgents.OkHttp,
			Proxy = proxy,
			AccessToken = session.AccessToken,
			IsMobile = true
		};
		using var protobufResponse = await Downloader.GetProtobufAsync(protobufRequest);
		if (protobufResponse.EResult != EResult.OK)
			return null;
		var obj = Serializer.Deserialize<CSteamNotification_GetSteamNotifications_Response>(protobufResponse.Stream);
		return obj;
	}
}