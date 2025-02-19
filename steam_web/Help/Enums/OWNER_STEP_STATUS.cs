namespace SteamWeb.Help.Enums;

public enum OWNER_STEP_STATUS : byte
{
    /// <summary>
    /// Шаг выполнен
    /// </summary>
    Done,
    /// <summary>
    /// Нужно подтвердить владение аккаунтов
    /// </summary>
    NeedTakeAction,
    /// <summary>
    /// Неожиданная структура html
    /// </summary>
    ErrorHtml,
    /// <summary>
    /// Не выполнен запрос
    /// </summary>
    ErrorRequest,
    /// <summary>
    /// Загружена неизвестная страница
    /// </summary>
    ErrorUnknownPage,
    /// <summary>
    /// Дошли до страницы снятия гуарда/Нужно подтвердить владение аккаунтом другим способом
    /// </summary>
    PageRemoveSteamGuard,
    /// <summary>
    /// Этот шаг сейчас не может быть выполнен
    /// </summary>
    WrongStep,
    /// <summary>
    /// Нужно подтвердить владение аккаунтов через Last4CardNumber или через отправку запроса на восстановление аккаунта. Для обхода можно попробовать привязать MobileGuard.
    /// </summary>
    FixIssue,
}
