namespace SteamWeb.Help.Enums;

public enum VERIFY_STATUS : byte
{
    /// <summary>
    ///  Ошибка при выполнении запроса
    /// </summary>
    Error,
    /// <summary>
    /// Подтверждение выполнено
    /// </summary>
    Success,
    /// <summary>
    /// Нужно начать процедуру смены данных заново
    /// </summary>
    Expired,
}
