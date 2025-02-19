namespace SteamWeb.Help.Enums;

public enum CHANGE_STEP : byte
{
    /// <summary>
    /// Ожидание выполнения шагов
    /// </summary>
    Waiting = 0,
    /// <summary>
    /// Шаг загрузки необходимых данные для выполнения последующих действий
    /// </summary>
    Step1 = 1,
    /// <summary>
    /// Шаг подтверждения владения аккаунтом
    /// </summary>
    Step2 = 2,
    /// <summary>
    /// Смена данных
    /// </summary>
    Step3 = 3,
    /// <summary>
    ///
    /// </summary>
    Step4 = 4,
    /// <summary>
    /// Этот провайдер смены данных выполнил свою работу
    /// </summary>
    Done,
}
