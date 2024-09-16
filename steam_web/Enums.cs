namespace SteamWeb.Enums;
public enum FA2: byte
{
    NonGuard,
    EmailCode,
    SteamMobileApp,
    NonGuardWithPhone,
    EmailCodeWithPhone,
    Deauth
}
[Flags]
public enum EligibilityStates
{
    None,
    Unknown1 = 1,
    Unknown2 = 2,
    IsLocked = 4,
    IsLimited = 8,
    TradeBanned = 16,
    Unknown3 = 32,
    SteamGuardEnabledFor15Days = 64,
    Unknown4 = 128,
    PasswordReset = 256,
    Unknown5 = 512,
    CoockieProblem = 1024,
    SteamGuard7Days = 2048,
    Unknown6 = 4096,
    Unknown7 = 8192,
    SteamPurchase = 16384,
    Unknown8 = 32768,
    Unknown9 = 65536,
}
public enum ETradeOfferState : byte
{
    NotFind = 0,
    /// <summary>
    /// Недействительным
    /// </summary>
    k_ETradeOfferStateInvalid = 1,
    /// <summary>
    /// Это торговое предложение было отправлено, ни одна из сторон еще не предприняла никаких действий.
    /// </summary>
    k_ETradeOfferStateActive = 2,
    /// <summary>
    /// Получатель принял предложение об обмене, и предметы были обменены.
    /// </summary>
    k_ETradeOfferStateAccepted = 3,
    /// <summary>
    /// Получатель сделал встречное предложение
    /// </summary>
    k_ETradeOfferStateCountered = 4,
    /// <summary>
    /// Торговое предложение не было принято до истечения срока действия
    /// </summary>
    k_ETradeOfferStateExpired = 5,
    /// <summary>
    /// Отправитель отменил предложение
    /// </summary>
    k_ETradeOfferStateCanceled = 6,
    /// <summary>
    /// Получатель отклонил предложение
    /// </summary>
    k_ETradeOfferStateDeclined = 7,
    /// <summary>
    /// Некоторые элементы в предложении больше не доступны (на это указывает отсутствующий флаг в выходных данных)
    /// </summary>
    k_ETradeOfferStateInvalidItems = 8,
    /// <summary>
    /// Предложение еще не отправлено и ожидает подтверждения по электронной почте / с мобильного телефона. Предложение видно только отправителю.
    /// </summary>
    k_ETradeOfferStateCreatedNeedsConfirmation = 9,
    /// <summary>
    /// Любая из сторон отменила предложение по электронной почте / мобильному телефону. Предложение видно обеим сторонам, даже если отправитель отменил его до отправки.
    /// </summary>
    k_ETradeOfferStateCanceledBySecondFactor = 10,
    /// <summary>
    /// Сделка приостановлена. Все предметы, участвующие в торговле, были удалены из запасов обеих сторон и будут автоматически доставлены в будущем.
    /// </summary>
    k_ETradeOfferStateInEscrow = 11
}