using SteamWeb.Auth.v2.Models;
using SteamWeb.Extensions;

namespace ExampleRefreshJwt;
internal static class Program
{
	static void Main(string[] args)
	{
		Console.Title = "Обновление steamLoginSecure";
		Console.WriteLine("Введите sessionId:");
		var sessionId = Console.ReadLine();

		Console.WriteLine("Введите browserId:");
		var browserId = Console.ReadLine();

		Console.WriteLine("Введите steamCountry:");
		var steamCountry = Console.ReadLine();

		Console.WriteLine("Введите steamRefresh_steam:");
		var steamRefresh_steam = Console.ReadLine();

		Console.WriteLine("Введите steamcommunity.com steamLoginSecure (начинается со steamid) или оставьте пустым, если не хотите обновлять:");
		var steamCommunityToken = Console.ReadLine();

		Console.WriteLine("Введите store.steampowered.com steamLoginSecure (начинается со steamid) или оставьте пустым, если не хотите обновлять:");
		var steamPoweredToken = Console.ReadLine();

		if (steamRefresh_steam.IsEmpty())
		{
			Console.WriteLine("Не указан steamRefresh_steam");
			Console.ReadKey();
			Environment.Exit(0);
		}

		var steamId = steamRefresh_steam!.Split("%7C")[0].ParseUInt64();
		if (!steamCommunityToken.IsEmpty())
		{
			var communitySession = new SessionData
			{
				SessionID = sessionId,
				AccessToken = steamCommunityToken!.Split("%7C%7C")[1],
				BrowserId = browserId,
				SteamCountry = steamCountry,
				PlatformType = EAuthTokenPlatformType.WebBrowser,
				SteamID = steamId,
			};
			var jwtRefreshResponse = SteamWeb.Script.Ajax.jwt_ajaxrefresh(new(communitySession), steamRefresh_steam!);
			if (!jwtRefreshResponse.Success)
			{
				Console.WriteLine("Ошибка при обновлении community jwt (ajaxrefresh) eresult=" + jwtRefreshResponse.Error);
				Console.ReadKey();
				Environment.Exit(0);
			}
			var setTokenResponse = SteamWeb.Script.Ajax.login_settoken(new(communitySession), jwtRefreshResponse, steamCommunityToken);
			if (setTokenResponse.Result != EResult.OK)
			{
				Console.WriteLine("Ошибка при обновлении community jwt (settoken) eresult=" + jwtRefreshResponse.Error);
				Console.ReadKey();
				Environment.Exit(0);
			}
			Console.WriteLine("Новый community token: " + communitySession.AccessToken);
		}
		if (!steamPoweredToken.IsEmpty())
		{
			var poweredSession = new SessionData
			{
				SessionID = sessionId,
				AccessToken = steamPoweredToken!.Split("%7C%7C")[1],
				BrowserId = browserId,
				SteamCountry = steamCountry,
				PlatformType = EAuthTokenPlatformType.WebBrowser,
				SteamID = steamId,
			};
			var jwtRefresh = SteamWeb.Script.Ajax.store_jwtrefresh(new(poweredSession), steamRefresh_steam);
			if (jwtRefresh.Result != EResult.OK)
			{
				Console.WriteLine("Ошибка при обновлении powered jwt (jwtrefresh) eresult=" + jwtRefresh.Result);
				Console.ReadKey();
				Environment.Exit(0);
			}
			Console.WriteLine("Новый powered token: " + poweredSession.AccessToken);
		}

		Console.WriteLine("Все токены обновлены");
		Console.ReadKey();
	}
}