using SteamWeb.Enums;

namespace SteamWeb.Models;
public class WebTradeEligibilityReason
{
    public EMarketNotAllowedReason State { get; }
    /// <summary>
    /// You must have a valid Steam purchase that is between 7 days and a year old with no recent chargebacks or payment disputes
    /// </summary>
    public bool SteamPurchase { get; }
    /// <summary>
    /// Your account's password has recently been reset
    /// </summary>
    public bool PasswordReset { get; }
    /// <summary>
    /// You have recently reset or enabled Steam Guard, you must wait 15 days after Steam Guard was enabled
    /// </summary>
    public bool SteamGuardEnabled { get; }
    /// <summary>
    /// Your account must be protected by Steam Guard for at least 15 days
    /// </summary>
    public bool SteamGuardRecentEnabled { get; }
    /// <summary>
    /// Your account is limited
    /// </summary>
    public bool IsLimited { get; }
    /// <summary>
    /// Your account is currently locked
    /// </summary>
    public bool IsLocked { get; }
    /// <summary>
    /// You are logging into The Market from a device that has not been protected by Steam Guard for 7 days
    /// </summary>
    public bool SteamGuard7Days { get; }
    /// <summary>
    /// Your account is currently trade banned
    /// </summary>
    public bool TradeBanned { get; }
    /// <summary>
    /// Steam is having trouble using your browser's cookie.
    /// </summary>
    public bool CoockieProblem { get; }
    /// <summary>
    /// Можно ли отправить трейд
    /// </summary>
    public bool CanTrade { get; }
    /// <summary>
    /// Можно ли продавать на маркете
    /// </summary>
    public bool CanMarket { get; }
    /// <summary>
    /// Аккаунт отключён, возможно в него не получится войти
    /// </summary>
    public bool AccountDisabled { get; }
    /// <summary>
    /// Временная ошибка
    /// </summary>
    public bool TemporaryFailure { get; }
    /// <summary>
    /// Добавлен новый метод оплаты; пример привязана карта VISA к аккаунту
    /// </summary>
    public bool NewPaymentMethod { get; }
    /// <summary>
    /// Недавно был refund чего-то (игры? предмета?)
    /// </summary>
    public bool RecentSelfRefund { get; }
    /// <summary>
    /// После привязки метода оплаты произошла ошибка; пример банк отклонил привязку(?)
    /// </summary>
    public bool NewPaymentMethodCannotBeVerified { get; }
    /// <summary>
    /// К аккаунту мало доверия; пример отменён трейд на удержании
    /// </summary>
    public bool NotTrusted { get; }
    /// <summary>
    /// Был принят гифт на баланс
    /// </summary>
    public bool AcceptedWalletGift { get; }

    public WebTradeEligibilityReason(WebTradeEligibility web)
    {
        State = (EMarketNotAllowedReason)web.Reason;
        TemporaryFailure = State.HasFlag(EMarketNotAllowedReason.TemporaryFailure);
        AccountDisabled = State.HasFlag(EMarketNotAllowedReason.AccountDisabled);
        IsLocked = State.HasFlag(EMarketNotAllowedReason.AccountLockedDown);
        IsLimited = State.HasFlag(EMarketNotAllowedReason.AccountLimited);
        TradeBanned = State.HasFlag(EMarketNotAllowedReason.TradeBanned);
        NotTrusted = State.HasFlag(EMarketNotAllowedReason.AccountNotTrusted);
        SteamGuardEnabled = State.HasFlag(EMarketNotAllowedReason.SteamGuardNotEnabled);
        SteamGuardRecentEnabled = State.HasFlag(EMarketNotAllowedReason.SteamGuardOnlyRecentlyEnabled);
        PasswordReset = State.HasFlag(EMarketNotAllowedReason.RecentPasswordReset);
        CoockieProblem = State.HasFlag(EMarketNotAllowedReason.InvalidCookie);
        SteamGuard7Days = State.HasFlag(EMarketNotAllowedReason.UsingNewDevice);
        RecentSelfRefund = State.HasFlag(EMarketNotAllowedReason.RecentSelfRefund);
        NewPaymentMethodCannotBeVerified = State.HasFlag(EMarketNotAllowedReason.NewPaymentMethodCannotBeVerified);
        SteamPurchase = State.HasFlag(EMarketNotAllowedReason.NoRecentPurchases);
        AcceptedWalletGift = State.HasFlag(EMarketNotAllowedReason.AcceptedWalletGift);

        CanMarket = web.Reason == 0 || web.Reason == (uint)EMarketNotAllowedReason.TradeBanned;
        CanTrade = !(AccountDisabled || IsLocked || IsLimited || TradeBanned || NotTrusted || SteamGuardEnabled || SteamGuardRecentEnabled || PasswordReset || CoockieProblem || SteamGuard7Days || SteamPurchase);
    }
}