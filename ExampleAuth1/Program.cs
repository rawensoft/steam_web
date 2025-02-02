using SteamWeb;

namespace ExampleAuth1;
internal static class Program
{
	static void Main(string[] args)
	{
		Console.Title = "Выполнение входа в Steam";

		Console.WriteLine("Введите логин:");
		var login = Console.ReadLine()!;

		Console.WriteLine("Введите пароль:");
		var passwd = Console.ReadLine()!;

		Console.WriteLine("Введите 2fa для SteamGuard или оставьте пустым для NoneGuard:");
		var fa2 = Console.ReadLine()!;

		var (result, session) = Steam.Auth(login, passwd, fa2, null, SteamWeb.Auth.v2.Enums.EAuthTokenPlatformType.WebBrowser);
		if (result != SteamWeb.Auth.v2.Enums.LoginResult.LoginOkay)
			Console.WriteLine("Ошибка при входе: " + result);
		else
			Console.WriteLine("Вход выполнен; access_token=" + session!.AccessToken);
		Console.ReadKey();
	}
}