namespace SteamWeb.Help.Enums;

public enum STEP_STATUS : byte
{
    /// <summary>
    /// Можно переходить к следующему шагу
    /// </summary>
    Ok,
    /// <summary>
    /// Не верный шаг в текущий момент
    /// </summary>
    WrongStep,
    /// <summary>
    /// Ошибка запроса
    /// </summary>
    ResponseError,
    /// <summary>
    /// Нет данных в query
    /// </summary>
    EmptyQuery,
    /// <summary>
    /// Не найдены все необходимые параметры в query
    /// </summary>
    BadQueryData,
    /// <summary>
    /// Сессия истекла, необходимо обновить её, либо войти заново
    /// </summary>
    SignIn,
}
