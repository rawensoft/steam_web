using SteamWeb.Auth.v2;
using SteamWeb.Extensions;
using LinkResult = SteamWeb.Auth.v1.Enums.LinkResult;
using FinalizeResult = SteamWeb.Auth.v1.Enums.FinalizeResult;
using System.Text.Json;
using System.Diagnostics;
using SDAOld = SteamWeb.Auth.v1.SteamGuardAccount;
using SessionOld = SteamWeb.Auth.v1.Models.SessionData;

namespace ExampleSteamGuardEmail;

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
					return _executePathCache = codeBase!.Replace("file://", "").Replace("steam_guard_email.dll", "");
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
		Console.Title = "Добавление мобильного аутентификатора через почту";
		Console.WriteLine("Текущий путь для сохранения smafile: " + SmafileDir);
		Console.WriteLine("Текущий путь для сохранения mafile: " + MafileDir);

		Console.WriteLine("Введите логин:");
		var userName = Console.ReadLine();
		if (userName.IsEmpty() || userName!.Length <= 3)
		{
			Console.WriteLine("Не верный логин...");
			Console.ReadKey();
			Environment.Exit(0);
		}

		Console.WriteLine("Введите пароль:");
		var password = Console.ReadLine();
		if (password.IsEmpty() || password!.Length <= 6)
		{
			Console.WriteLine("Не верный пароль...");
			Console.ReadKey();
			Environment.Exit(0);
		}


		Console.WriteLine("Начинаем авторизацию..");
		var userLogin = new UserLogin(userName, password, EAuthTokenPlatformType.MobileApp);
		var resultBegin = userLogin.BeginAuthSessionViaCredentials();
		if (!resultBegin)
		{
			Console.WriteLine($"Не удалось начать авторизацию ({userLogin.Result}|{userLogin.LastEResult})...");
			Console.ReadKey();
			Environment.Exit(0);
		}

		if (userLogin.IsNeedEmailCode)
		{
			Console.WriteLine("Введите пароль:");
			var code = Console.ReadLine();
			if (code.IsEmpty() || code!.Length != 5)
			{
				Console.WriteLine("Не верный код с почты...");
				Console.ReadKey();
				Environment.Exit(0);
			}

			var resultUpdate = userLogin.UpdateAuthSessionWithSteamGuardCode(code);
			if (!resultUpdate)
			{
				Console.WriteLine($"Не удалось подтвердить авторизацию ({userLogin.Result}|{userLogin.LastEResult})...");
				Console.ReadKey();
				Environment.Exit(0);
			}
		}
		else if (userLogin.IsNeedTwoFactorCode)
		{
			Console.WriteLine("Уже привязан Steam Guard...");
			Console.ReadKey();
			Environment.Exit(0);
		}

		var resultPoll = userLogin.PollAuthSessionStatus();
		if (!resultPoll)
		{
			Console.WriteLine($"Не удалось закончить авторизацию ({userLogin.Result}|{userLogin.LastEResult})...");
			Console.ReadKey();
			Environment.Exit(0);
		}


		Console.WriteLine("Начинаем добавление аутентификатора..");
		var linker = new AuthenticatorLinker(userLogin.Session!, null);
		var resultAdd = linker.AddAuthenticatorEmail();
		if (resultAdd != LinkResult.AwaitingFinalization)
		{
			Console.WriteLine($"Не удалось начать добавление аутентификатора ({resultAdd})...");
			Console.ReadKey();
			Environment.Exit(0);
		}

		SaveLinker(linker.LinkedAccount);
		Console.WriteLine("Введите код с почты:");
		var emailCode = Console.ReadLine();
		if (emailCode.IsEmpty() || emailCode!.Length != 5)
		{
			Console.WriteLine("Не верный код с почты...");
			Console.ReadKey();
			Environment.Exit(0);
		}

		var resultFinalize = linker.FinalizeAddAuthenticatorEmail(emailCode!);
		if (resultFinalize != FinalizeResult.Success)
		{
			Console.WriteLine($"Не удалось начать добавление аутентификатора ({resultFinalize})...");
			Console.ReadKey();
			Environment.Exit(0);
		}


		SaveLinker(linker.LinkedAccount);
		Console.WriteLine("Аутентификатор добавлен...");
		Console.ReadKey();
	}
	static void SaveLinker(SteamGuardAccount sda)
	{
		var options = new JsonSerializerOptions
		{
			Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
			WriteIndented = true,
		};
		var data1 = JsonSerializer.Serialize(sda, options);
		var filename1 = Path.Join(SmafileDir, sda.AccountName.ToLower() + ".smafile");
		File.WriteAllText(filename1, data1);

		var sessionOld = new SessionOld
		{
			AccessToken = sda.Session!.AccessToken,
			RefreshToken = sda.Session!.RefreshToken,
			SessionID = sda.Session!.SessionID,
			SteamID = sda.Session!.SteamID,
			SteamLanguage = sda.Session!.SteamLanguage,
		};
		var sdaOld = new SDAOld
		{
			AccountName = sda.AccountName,
			DeviceID = sda.DeviceID,
			FullyEnrolled = true,
			IdentitySecret = sda.IdentitySecret,
			RevocationCode = sda.RevocationCode,
			Secret1 = sda.Secret1,
			SerialNumber = sda.SerialNumber.ToString(),
			ServerTime = sda.ServerTime,
			SharedSecret = sda.SharedSecret,
			Status = sda.Status,
			TokenGID = sda.TokenGID,
			URI = sda.URI,
			Session = sessionOld
		};
		var data2 = JsonSerializer.Serialize(sdaOld, options);
		var filename2 = Path.Join(MafileDir, sdaOld.AccountName.ToLower() + ".mafile");
		File.WriteAllText(filename2, data2);
	}
}
