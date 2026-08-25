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
public enum EMarketNotAllowedReason
{
    None,

    /// <summary>
    /// A back-end call failed or something that might work again on retry.
    /// </summary>
    TemporaryFailure = 1,

    /// <summary>
    /// Disabled account.
    /// </summary>
    AccountDisabled = 2,

    /// <summary>
    /// Locked account.
    /// </summary>
    AccountLockedDown = 4,

    /// <summary>
    /// Limited account; no purchases.
    /// </summary>
    AccountLimited = 8,

    /// <summary>
    /// The account is banned from trading items.
    /// </summary>
    TradeBanned = 16,

    /// <summary>
    /// Wallet funds aren't tradable because the user has had no purchase activity in the last year or has had no purchases prior to last month.
    /// </summary>
    AccountNotTrusted = 32,

    /// <summary>
    /// The user doesn't have Steam Guard enabled.
    /// </summary>
    SteamGuardNotEnabled = 64,

    /// <summary>
    /// The user has Steam Guard but it hasn't been enabled for the required number of days.
    /// </summary>
    SteamGuardOnlyRecentlyEnabled = 128,

    /// <summary>
    /// The user has recently forgotten their password and reset it.
    /// </summary>
    RecentPasswordReset = 256,

    /// <summary>
    /// The user has recently funded his or her wallet with a new payment method.
    /// </summary>
    NewPaymentMethod = 512,

    /// <summary>
    /// An invalid cookie was sent by the user.
    /// </summary>
    InvalidCookie = 1024,

    /// <summary>
    /// The user has Steam Guard but is using a new computer or web browser.
    /// </summary>
    UsingNewDevice = 2048,

    /// <summary>
    /// The user has Steam Guard, but is using a new computer or web browser.
    /// </summary>
    RecentSelfRefund = 4096,

    /// <summary>
    /// The user has recently funded his or her wallet with a new payment method that cannot be verified.
    /// </summary>
    NewPaymentMethodCannotBeVerified = 8192,

    /// <summary>
    /// Not only is the account not trusted but they have no recent purchases at all.
    /// </summary>
    NoRecentPurchases = 16384,

    /// <summary>
    /// User accepted a wallet gift that was recently purchased.
    /// </summary>
    AcceptedWalletGift = 32768,

    /// <summary>
    /// User did something that triggered a trade cooldown (like reversing trades).
    /// </summary>
    TradeCooldown = 65536,
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