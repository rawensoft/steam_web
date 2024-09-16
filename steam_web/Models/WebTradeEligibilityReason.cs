using SteamWeb.Enums;

namespace SteamWeb.Models;
public class WebTradeEligibilityReason
{
    public EligibilityStates State { get; }
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
    public bool SteamGuard15Days { get; }
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

    public WebTradeEligibilityReason(WebTradeEligibility web)
    {
        State = (EligibilityStates)web.Reason;
        SteamPurchase = State.HasFlag(EligibilityStates.SteamPurchase);
        PasswordReset = State.HasFlag(EligibilityStates.PasswordReset);
        SteamGuardEnabled = SteamGuard15Days = State.HasFlag(EligibilityStates.SteamGuardEnabledFor15Days);
        IsLimited = State.HasFlag(EligibilityStates.IsLimited);
        IsLocked = State.HasFlag(EligibilityStates.IsLocked);
        SteamGuard7Days = State.HasFlag(EligibilityStates.SteamGuard7Days);
        TradeBanned = State.HasFlag(EligibilityStates.TradeBanned);
        CoockieProblem = State.HasFlag(EligibilityStates.CoockieProblem);

        CanTrade = !(PasswordReset && SteamGuardEnabled && SteamGuard15Days && IsLocked && TradeBanned && SteamGuard7Days && CoockieProblem);
        CanMarket = !(SteamPurchase && PasswordReset && SteamGuardEnabled && SteamGuard15Days && IsLimited && IsLocked && SteamGuard7Days && TradeBanned && CoockieProblem);
    }
}