using SteamWeb.Auth.v2;

namespace ExampleAuth3;
internal static class Program
{
	static void Main(string[] args)
	{
		Console.Title = "Выполнение входа в Steam";

		Console.WriteLine("Введите логин:");
		var login = Console.ReadLine()!;

		Console.WriteLine("Введите пароль:");
		var passwd = Console.ReadLine()!;

		var userLogin = new UserLogin(login, passwd, SteamWeb.Auth.v2.Enums.EAuthTokenPlatformType.WebBrowser);
		if (!userLogin.BeginAuthSessionViaCredentials())
		{
			Console.WriteLine($"Ошибка в begin_auth_session_via_credentials {userLogin.Result}; eresult={userLogin.LastEResult}");
			Console.ReadKey();
			Environment.Exit(0);
		}
		while (!userLogin.FullyEnrolled)
		{
			// true будет только в одном из 3, либо будет во всех false
			if (userLogin.IsNeedEmailCode)
			{
				Console.WriteLine("Введите код с почты:");
				var emailCode = Console.ReadLine()!;
				userLogin.Data = emailCode;
			}
			else if (userLogin.IsNeedTwoFactorCode)
			{
				Console.WriteLine("Введите 2fa код с приложения:");
				var fa2 = Console.ReadLine()!;
				userLogin.Data = fa2;
			}
			else if (userLogin.IsNeedConfirm)
			{
				Console.WriteLine("Нужно подтвердить вход, отправьте Y после подтверждения:");
				var text = Console.ReadLine()!;
				if (text != "Y")
				{
					Console.WriteLine("Вход прерван пользователем...");
					Console.ReadKey();
					Environment.Exit(0);
				}
			}

			if ((userLogin.IsNeedEmailCode || userLogin.IsNeedTwoFactorCode) && !userLogin.UpdateAuthSessionWithSteamGuardCode(null))
			{
				Console.WriteLine($"Ошибка в update_auth_session_with_steam_guard_code {userLogin.Result}; eresult={userLogin.LastEResult}");
				Console.ReadKey();
				Environment.Exit(0);
			}
		}

		using var mres = new ManualResetEventSlim(false);
		while ((userLogin.NextStep == SteamWeb.Auth.v2.Enums.NEXT_STEP.Poll || userLogin.IsNeedConfirm) && !userLogin.FullyEnrolled)
		{
			mres.Wait(1000); // делаем паузу как делается на самой странице входа
			if (!userLogin.PollAuthSessionStatus())
			{
				Console.WriteLine($"Ошибка в poll_auth_session_status {userLogin.Result}; eresult={userLogin.LastEResult}");
				Console.ReadKey();
				Environment.Exit(0);
			}
			else
				break;
		}

		var session = userLogin.Session;
		Console.WriteLine("Вход выполнен; access_token=" + session!.AccessToken);
		Console.ReadKey();
	}
}