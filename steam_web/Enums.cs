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
public enum PURCHASE_TYPE : byte
{
    /// <summary>
    /// Неизвестный тип покупки
    /// </summary>
    Unknown,
    /// <summary>
    /// Покупка игры или программы
    /// </summary>
    Purchase,
    /// <summary>
    /// Покупка подарка
    /// </summary>
    GiftPurchase,
    /// <summary>
    /// Внутриигровая покупка
    /// </summary>
    InGamePurchase,
    /// <summary>
    /// Одна транзакция на маркете
    /// </summary>
    MarketTransaction,
    /// <summary>
    /// Более одной транзакции на маркете
    /// </summary>
    MarketTransactions,
    /// <summary>
    /// Возврат средств по запросу отмены покупки
    /// </summary>
    Refund,
    /// <summary>
    /// Конверсия одной валюты в другую
    /// </summary>
    Conversion,
}
/// <summary>
/// Информация взята из <see href="https://pub.dev/documentation/steamworks/latest/steamworks/EMarketNotAllowedReasonFlags.html"/>
/// </summary>
[Flags]
public enum EligibilityStates
{
    None,
    TemporaryFailure = 1,
    AccountDisabled = 2,
    AccountLockedDown = 4,
    AccountLimited = 8,
    TradeBanned = 16,
    AccountNotTrusted = 32,
    SteamGuardNotEnabled = 64,
    SteamGuardOnlyRecentlyEnabled = 128,
    RecentPasswordReset = 256,
    NewPaymentMethod = 512,
    InvalidCookie = 1024,
    UsingNewDevice = 2048,
    RecentSelfRefund = 4096,
    NewPaymentMethodCannotBeVerified = 8192,
    NoRecentPurchases = 16384,
    AcceptedWalletGift = 32768,
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
/// <summary>
/// Определяет набор отношений, которые могут быть между пользователями Steam.
/// </summary>
public enum EFriendRelationship: byte
{
    /// <summary>
    /// У пользователей нет отношений.
    /// </summary>
    k_EFriendRelationshipNone = 0,
    /// <summary>
    /// Пользователь только что нажал «Игнорировать» в ответ на полученное приглашение. Не сохраняется.
    /// </summary>
    k_EFriendRelationshipBlocked = 1,
    /// <summary>
    /// Пользователь отправил запрос на дружбу с текущим пользователем.
    /// </summary>
    k_EFriendRelationshipRequestRecipient = 2,
    /// <summary>
    /// «Обычный» друг.
    /// </summary>
    k_EFriendRelationshipFriend = 3,
    /// <summary>
    /// Текущий игрок отправил приглашение в друзья.
    /// </summary>
    k_EFriendRelationshipRequestInitiator = 4,
    /// <summary>
    /// Текущий пользователь заблокировал другого пользователя в комментариях, чате и т. п. Это значение сохраняется.
    /// </summary>
    k_EFriendRelationshipIgnored = 5,
    /// <summary>
    /// Пользователь проигнорировал текущего пользователя.
    /// </summary>
    k_EFriendRelationshipIgnoredFriend = 6,
	/// <summary>
	/// Устаревший параметр, не используется.
	/// <para/>
	/// removed "was used by the original implementation of the facebook linking feature; but now unused."
	/// </summary>
	k_EFriendRelationshipSuggested_DEPRECATED = 7,
    /// <summary>
    /// Общее число отношений, используется в циклах и для проверок.
    /// </summary>
    k_EFriendRelationshipMax = 8,
}