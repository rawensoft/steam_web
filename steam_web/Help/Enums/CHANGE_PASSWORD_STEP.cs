namespace SteamWeb.Help.Enums;

public enum CHANGE_PASSWORD_STEP : byte
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
    /// Не правильный пароль, возможно, его длина менее 7-и символов, либо он помечен как скомпрометированный
    /// </summary>
    BadPassword,
    /// <summary>
    /// Запрос на изменение пароля не выполнен
    /// </summary>
    PasswordNotChanged,
    /// <summary>
    /// Запрос на получение RSA не выполнен
    /// </summary>
    ErrorRSA,
    /// <summary>
    /// Не верная последовательность шагов
    /// </summary>
    WrongStep,
}
