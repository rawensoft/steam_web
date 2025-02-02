using SteamWeb;
using SteamWeb.Auth.v2;

namespace ExampleAuth2;
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

		var userLogin = new UserLogin(login, passwd, SteamWeb.Auth.v2.Enums.EAuthTokenPlatformType.WebBrowser);
		userLogin.Data = fa2;
		var (result, session) = Steam.Auth(userLogin);
		if (result != SteamWeb.Auth.v2.Enums.LoginResult.LoginOkay)
			Console.WriteLine($"Ошибка при входе {result}; eresult={userLogin.LastEResult}");
		else
			Console.WriteLine("Вход выполнен; access_token=" + session!.AccessToken);
		Console.ReadKey();
	}
}