namespace SteamWeb.Help.Enums;

public enum ACCEPT_PROVIDER_STATUS : byte
{
    /// <summary>
    /// Ожидание принятия решения
    /// </summary>
    NoAction,
    /// <summary>
    /// Ожидание подверждения через этот провайдер
    /// </summary>
    Waiting,
    /// <summary>
    /// Выполнено подтверждение через этот провайдер
    /// </summary>
    Accepted,
    /// <summary>
    /// Отклонено подтверждение через этот провайдер
    /// </summary>
    Declined,
}
