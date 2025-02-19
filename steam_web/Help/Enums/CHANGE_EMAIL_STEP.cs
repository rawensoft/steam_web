namespace SteamWeb.Help.Enums;

public enum CHANGE_EMAIL_STEP : byte
{
    /// <summary>
    /// Запрос на изменение пароля выполнен, но не был выполнен переход по хешу
    /// </summary>
    Ok,
    /// <summary>
    /// Пароль изменён на аккаунте
    /// </summary>
    Done,
    /// <summary>
    /// Код на новую почту не отправлен
    /// </summary>
    EmailCodeNotSend,
    /// <summary>
    /// Запрос на получение RSA не выполнен
    /// </summary>
    ErrorRequest,
    /// <summary>
    /// Не верная последовательность шагов
    /// </summary>
    WrongStep,
}