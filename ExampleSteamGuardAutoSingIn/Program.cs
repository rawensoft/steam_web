using System.Diagnostics;
using System.Text.Json;
using SteamWeb.Auth.v2;

namespace ExampleSteamGuardAutoSingIn;
internal static class Program
{
	private static string? _executePathCache = null;

	/// <summary>
	/// Значение в 20 сек использовано из-за его использования в приложении steam между вызовами <see cref="SteamGuardAccount.CheckSession"/> 
	/// </summary>
	public const int Delay = 20000;
	public const string SmafilesFolderName = "smafiles";
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
					return _executePathCache = codeBase!.Replace("file://", "").Replace("steam_auto_sign_in.dll", "");
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

	static void Main(string[] args)
	{
		Console.Title = "Автопринятие попыток входа в аккаунт Steam";
		Console.WriteLine("Текущий путь для загрузки smafile: " + SmafileDir);

		var smafiles = Directory.GetFiles(SmafileDir);
		var accounts = new List<SteamGuardAccount>(smafiles.Length + 2);
		foreach (var smafile in smafiles)
		{
			var data = File.ReadAllText(smafile);
			var obj = JsonSerializer.Deserialize<SteamGuardAccount>(data);
			accounts.Add(obj!);
			var thread = new Thread(() => AutoAccept(obj!));
			thread.Start();
		}

		using var mres = new ManualResetEventSlim(false);
		mres.Wait();
	}

	static void AutoAccept(SteamGuardAccount guard)
	{
		if (guard.Session == null)
			return;
		using var mres = new ManualResetEventSlim(false);
		ulong lastClientId = guard.LastClientId;
		while (true)
		{
			/*
			 * Проверяем сессию, т.к. через неё получаем нужный client_id
			 * Если access_token истёк, тогда обновляем его
			 */
			if (!guard.CheckSession() || (!guard.RefreshSession() && !guard.CheckSession()))
				continue;
			if (lastClientId != guard.LastClientId)
			{
				var request = new SteamWeb.Auth.v2.DTO.CAuthentication_GetAuthSessionInfo_Request()
				{
					client_id = guard.LastClientId
				};
				var sessionInfo = guard.GetAuthSessionInfo(request);
                if (sessionInfo != null)
                {
					guard.UpdateAuthSessionWithMobileConfirmation(request, sessionInfo);
				}
			}
			lastClientId = guard.LastClientId;
			mres.Wait(Delay);
		}
	}
}
