using SteamWeb.Auth.v2.Models;
using SteamWeb.Help;
using SteamWeb.Help.Enums;
using SteamWeb.Models;

namespace ExampleChangePassword;

internal static class Program
{
    private static void Main(string[] args)
    {
        if (args.Length != 4)
        {
            Console.WriteLine("Не переданы все аргументы...");
            Console.ReadKey();
            return;
        }

        var access_token = args[0];
        var browserid = args[1];
        var sessionid = args[2];
        var steam_country = args[3];
        var jwt = JwtData.Deserialize(access_token);

        var session = new SessionData
        {
            AccessToken = access_token,
            BrowserId = browserid,
            PlatformType = SteamWeb.Auth.v2.Enums.EAuthTokenPlatformType.WebBrowser,
            RefreshToken = null,
            SessionID = sessionid,
            SteamCountry = steam_country,
            SteamID = jwt.Subject,
        };
        var cancelationTokenSource = new CancellationTokenSource();
        var passwordProvider = new ChangePasswordProvider(session, null);

        // Начало шаг 1
        Console.WriteLine("Начинаем смену пароля");
        var step_result = passwordProvider.Step1(cancelationTokenSource.Token);
        if (step_result != STEP_STATUS.Ok)
        {
            Console.WriteLine("Начало выполнено с ошибкой: " + step_result + "...");
            Console.ReadKey();
            return;
        }

        // Начало шаг 2
        OWNER_STEP_STATUS owner_status = OWNER_STEP_STATUS.Done;
        Console.WriteLine("Начинаем подтверждение владения аккаунтов");
        using var mres = new ManualResetEventSlim(false);
        bool is_sda_accepted = false;
        while ((owner_status = passwordProvider.Step2(cancelationTokenSource.Token)) == OWNER_STEP_STATUS.NeedTakeAction)
        {
            /* Если на аккаунте стоит MobileGuard защита (SDA\mafile), тогда 100% нужно подтверждать его
                            * Если пропустить шаг SteamMobileApp, тогда происходит моментальное перенаправление на восстановление аккаунта, через удаление гуарда
                            * Можно пропустить шаг с получением смс на телефон
                            * Варианты при MobileGuard: подтвердить в SDA, подтвердить почту\пароль, пропустить всё остальное
                            * Варианты при EmailGuard: подтвердить почту, возможно, пароль, всё остальное пропустить
                            * Для других вариантов всегда подтверждать почту, телефон можно никогда не подтверждать, а вот, если будет вход по паролю, тогда его нужно тоже
                           */

            if (passwordProvider.Provider != null && passwordProvider.Provider.Type == PROVIDER_LIST.SteamMobileApp)
            {
                var result1 = passwordProvider.Provider.Accept(cancelationTokenSource.Token);
                if (result1)
                    Console.WriteLine("Начинаем подтверждение через этот провайдер: " + passwordProvider.Provider.Type);
                else
                {
                    Console.WriteLine("Не удалось начать подтверждать через этот провайдер...");
                    Console.ReadKey();
                    return;
                }

                POLL_STATUS poll_status = POLL_STATUS.Error;
                for (var g_datePollStart = DateTime.UtcNow; 600000 >= (DateTime.UtcNow - g_datePollStart).TotalSeconds; mres.Wait(3000))
                {
                    poll_status = passwordProvider.Provider.Poll(cancelationTokenSource.Token);
                    Console.WriteLine("Ожидание принятия в SteamMobileApp; статус: " + poll_status);
                    if (poll_status == POLL_STATUS.Success)
                        break;
                    else if (poll_status == POLL_STATUS.Expired)
                    {
                        Console.WriteLine("Истекло время ожидания...");
                        Console.ReadKey();
                        return;
                    }
                }

                var verify_status = passwordProvider.Provider.Verify(string.Empty, cancelationTokenSource.Token);
                Console.WriteLine("Проверяем подтверждение в SteamMobileApp; статус: " + verify_status);
                if (verify_status == VERIFY_STATUS.Success)
                    is_sda_accepted = true;
                if (verify_status != VERIFY_STATUS.Success)
                {
                    Console.WriteLine("Не удалось подтвердить владение аккаунтом...");
                    Console.ReadKey();
                    return;
                }
            }
            if (passwordProvider.Provider != null && passwordProvider.Provider.Type == PROVIDER_LIST.Email)
            {
                if (!is_sda_accepted)
                {
                    var result1 = passwordProvider.Provider.Accept(cancelationTokenSource.Token);
                    if (result1)
                        Console.WriteLine("Начинаем подтверждение через этот провайдер: " + passwordProvider.Provider.Type);
                    else
                    {
                        Console.WriteLine("Не удалось начать подтверждать через этот провайдер...");
                        Console.ReadKey();
                        return;
                    }

                    Console.WriteLine("Введите код с почты:");
                    string email_code = Console.ReadLine()!;
                    var verify_status = passwordProvider.Provider.Verify(email_code, cancelationTokenSource.Token);
                    Console.WriteLine("Проверяем код; статус: " + verify_status);
                    if (verify_status != VERIFY_STATUS.Success)
                    {
                        Console.WriteLine("Не удалось подтвердить владение аккаунтом...");
                        Console.ReadKey();
                        return;
                    }
                }
                else
                {
                    var result1 = passwordProvider.Provider.Decline(cancelationTokenSource.Token);
                    if (result1)
                        Console.WriteLine("Пропустили подтверждение через почту, т.к. уже подтвердили через SteamMobileApp");
                    else
                    {
                        Console.WriteLine("Не смогли пропустить подтверждение через почту...");
                        Console.ReadKey();
                        return;
                    }
                }
            }
            if (passwordProvider.Provider != null && passwordProvider.Provider.Type == PROVIDER_LIST.Phone)
            {
                var result1 = passwordProvider.Provider.Decline(cancelationTokenSource.Token);
                if (result1)
                    Console.WriteLine("Пропустили подтверждение через телефон");
                else
                {
                    Console.WriteLine("Не смогли пропустить подтверждение через телефон...");
                    Console.ReadKey();
                    return;
                }
            }
            if (passwordProvider.Provider != null && passwordProvider.Provider.Type == PROVIDER_LIST.Login)
            {
                var result1 = passwordProvider.Provider.Accept(cancelationTokenSource.Token);
                if (result1)
                    Console.WriteLine("Начинаем подтверждение через этот провайдер: " + passwordProvider.Provider.Type);
                else
                {
                    Console.WriteLine("Не удалось начать подтверждать через этот провайдер...");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine("Введите текущий пароль от аккаунта:");
                string password = Console.ReadLine()!;
                var verify_status = passwordProvider.Provider.Verify(password, cancelationTokenSource.Token);
                Console.WriteLine("Проверка пароля; статус: " + verify_status);
                if (verify_status != VERIFY_STATUS.Success)
                {
                    Console.WriteLine("Не удалось подтвердить владение аккаунтом...");
                    Console.ReadKey();
                    return;
                }
            }
        }
        if (owner_status != OWNER_STEP_STATUS.Done)
        {
            Console.WriteLine("Не удалось пройти подтверждение владения аккаунтов: " + owner_status + "...");
            Console.ReadKey();
            return;
        }

        // Начало шаг 3
RetryPassword:
        Console.WriteLine("Введите новый пароль:");
        var new_password = Console.ReadLine()!;
        var change_status = passwordProvider.Step3(new_password, cancelationTokenSource.Token);
        if (change_status == CHANGE_PASSWORD_STEP.BadPassword)
        {
            Console.WriteLine("Введёный пароль не подходит, попробуйте другой");
            goto RetryPassword;
        }
        if (change_status == CHANGE_PASSWORD_STEP.Done)
        {
            Console.WriteLine("Пароль изменён, хотите изменить почту? Y-да: ");
            var key = Console.ReadKey();
            Console.WriteLine();
            if (key.Key != ConsoleKey.Y)
            {
                Console.WriteLine("Почта не будет изменена...");
                Console.ReadKey();
                return;
            }

            var emailProvider = passwordProvider.CreateChangeEmailProvider()!;
            Console.WriteLine("Начало изменения почты");
            owner_status = emailProvider.Step2(cancelationTokenSource.Token);
            if (owner_status != OWNER_STEP_STATUS.Done)
            {
                Console.WriteLine("Статус подтверждения владения аккаунтов должен был быть пройден автоматически; статус: " + owner_status + "...");
                Console.ReadKey();
                return;
            }

RetryEmail:
            Console.WriteLine("Введите новую почту:");
            var email = Console.ReadLine()!;
            Console.WriteLine();
            var email_status = emailProvider.Step3(email, cancelationTokenSource.Token);
            if (email_status != CHANGE_EMAIL_STEP.Done)
            {
                Console.WriteLine("Не удалось отправить код на почту из-за ошибки: " + email_status);
                goto RetryEmail;
            }

RetryEmailCode:
            Console.WriteLine("Введите код с почты:");
            var email_code = Console.ReadLine()!;
            Console.WriteLine();
            email_status = emailProvider.Step4(email_code, cancelationTokenSource.Token);
            if (email_status != CHANGE_EMAIL_STEP.Done)
            {
                Console.WriteLine("Не удалось подтвердить код с почты из-за ошибки: " + email_status);
                goto RetryEmailCode;
            }
            Console.WriteLine("Пароль и почта изменены...");
        }
        else
            Console.WriteLine("Пароль изменён...");
        Console.ReadKey();
    }
}