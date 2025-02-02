using System.Diagnostics;
using System.Text.Json;
using SteamWeb.Auth.v2;
using SteamWeb.Extensions;

namespace ExampleMoveSteamGuard;
internal static class Program
{
	private static string? _executePathCache = null;

	public const string SmafilesFolderName = "smafiles";
	public const string MafilesFolderName = "mafiles";
	public static string? ExecutePath
	{
		get
		{
			if (_executePathCache != null)
				return _executePathCache;
			if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				try
				{
					var assembly = System.Reflection.Assembly.GetExecutingAssembly();
#pragma warning disable SYSLIB0044 // Тип или член устарел
					var codeBase = assembly.GetName().CodeBase;
#pragma warning restore SYSLIB0044 // Тип или член устарел
					return _executePathCache = codeBase!.Replace("file://", "").Replace("steam_guard_mover.dll", "");
				}
				catch
				{
					/*
					 * Если будет исключение, то будем пробовать через метод для windows
					 * Скорее всего ошибка также будет и в нём, но попробовать нужно
					 */
				}
			}
			var proc = Process.GetCurrentProcess();
			var module = proc.MainModule;
			if (module == null)
				return null;
			string? filename = module.FileName;
			if (filename == null)
				return null;
			if (module.ModuleName == null)
				return null;
			string path = _executePathCache = filename.Replace(module.ModuleName, "");
			return path;
		}
	}
	public static string SmafileDir
	{
		get
		{
			var path = Path.Join(ExecutePath, SmafilesFolderName);
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			return path;
		}
	}
	public static string MafileDir
	{
		get
		{
			var path = Path.Join(ExecutePath, MafilesFolderName);
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			return path;
		}
	}

	static void Main(string[] args)
	{
		Console.Title = "Перенос Steam Guard";
		Console.WriteLine("Текущий путь для сохранения smafile: " + SmafileDir);
		Console.WriteLine("Текущий путь для сохранения mafile: " + MafileDir);
		

		Console.WriteLine("Введите логин:");
		string? login = Console.ReadLine();
		if (login == null)
		{
			Console.WriteLine("Не введён логин...");
			Console.ReadKey();
			Environment.Exit(0);
		}

		Console.WriteLine("Введите пароль:");
		string? passwd = Console.ReadLine();
		if (passwd.IsEmpty())
		{
			Console.WriteLine("Не введён пароль...");
			Console.ReadKey();
			Environment.Exit(0);
		}
		if (passwd!.Length < 8)
		{
			Console.WriteLine("Пароль менее 8-и символов...");
			Console.ReadKey();
			Environment.Exit(0);
		}


		Console.WriteLine("Начинаем авторизацию..");
		var userLogin = new UserLogin(login, passwd, SteamWeb.Auth.v2.Enums.EAuthTokenPlatformType.MobileApp);
		var resultBegin = userLogin.BeginAuthSessionViaCredentials();
		if (!resultBegin)
		{
			Console.WriteLine($"Не удалось начать авторизацию ({userLogin.Result}|{userLogin.LastEResult})...");
			Console.ReadKey();
			Environment.Exit(0);
		}
		if (!userLogin.IsNeedTwoFactorCode)
		{
			Console.WriteLine("Не привязан Steam Guard...");
			Console.ReadKey();
			Environment.Exit(0);
		}

		Console.WriteLine("Начинаем перенос..");
		var moverGuard = new AuthenticatorMover(userLogin);
		var resultSms = moverGuard.RemoveAuthenticatorViaChallengeStart();
		if (!resultSms.Success)
		{
			Console.WriteLine("Не удалось отправить смс код...");
			Console.ReadKey();
			Environment.Exit(0);
		}

		Console.WriteLine("Введите смс код:");
		string? smsCode = Console.ReadLine();
		if (smsCode.IsEmpty())
		{
			Console.WriteLine("Смс код не указан");
			Console.ReadKey();
			Environment.Exit(0);
		}

		var request = new SteamWeb.Auth.v2.DTO.CTwoFactor_RemoveAuthenticatorViaChallengeContinue_Request
		{
			generate_new_token = true,
			SmsCode = smsCode!,
			version = 1
		};
		var result = moverGuard.RemoveAuthenticatorViaChallengeContinue(request);
		if (!result.Success)
		{
			Console.WriteLine("Не удалось продолжить перенос steam guard...");
			Console.ReadKey();
			Environment.Exit(0);
		}


		var options = new JsonSerializerOptions
		{
			Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
			WriteIndented = true
		};

		var sdaV1 = moverGuard.GetSteamGuardv1();
		var dataV1 = JsonSerializer.Serialize(sdaV1, options);
		var filenameV1 = Path.Combine(SmafileDir!, login + ".smafile");
		File.WriteAllText(filenameV1, dataV1);

		var sdaV2 = moverGuard.GetSteamGuardv2();
		var dataV2 = JsonSerializer.Serialize(sdaV2, options);
		var filenameV2 = Path.Combine(MafileDir!, login + ".mafile");
		File.WriteAllText(filenameV2, dataV2);


		if (sdaV2 != null)
		{
			Console.WriteLine("Steam Guard перенесён..");
			var fa2 = sdaV2.GenerateSteamGuardCode();
			Console.WriteLine("Подтверждаем вход..");
			if (!userLogin.UpdateAuthSessionWithSteamGuardCode(fa2))
			{
				Console.WriteLine("Не удалось авторизоваться...");
				Console.ReadKey();
				Environment.Exit(0);
			}
			Console.WriteLine("Получаем сессию..");
			if (!userLogin.PollAuthSessionStatus())
			{
				Console.WriteLine("Не удалось авторизоваться...");
				Console.ReadKey();
				Environment.Exit(0);
			}

			sdaV2.Session = userLogin.Session;
			dataV2 = JsonSerializer.Serialize(sdaV2, options);
			File.WriteAllText(filenameV2, dataV2);
			Console.WriteLine("Steam сессия сохранена...");
		}
		else
			Console.WriteLine("Steam Guard перенесён...");


		Console.ReadKey();
		Environment.Exit(0);
	}
}
