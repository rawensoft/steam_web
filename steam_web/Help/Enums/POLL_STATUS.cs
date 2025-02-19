namespace SteamWeb.Help.Enums;

public enum POLL_STATUS : byte
{
    /// <summary>
    ///  Ошибка при выполнении запроса
    /// </summary>
    Error,
    /// <summary>
    ///  Нужно дальше посылать запрос Poll
    /// </summary>
    Continue,
    /// <summary>
    /// Подтверждение выполнено
    /// </summary>
    Success,
    /// <summary>
    /// Нужно начать процедуру смены данных заново
    /// </summary>
    Expired,
    /// <summary>
    /// Недоступно для этого провайдера
    /// </summary>
    NotAvailable,
}
