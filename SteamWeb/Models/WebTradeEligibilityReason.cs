namespace SteamWeb.Models;
public sealed class WebTradeEligibilityReason
{
    /// <summary>
    /// You must have a valid Steam purchase that is between 7 days and a year old with no recent chargebacks or payment disputes
    /// </summary>
    public bool SteamPurchase { get; init; } = false; // 16416
    /// <summary>
    /// Your account's password has recently been reset
    /// </summary>
    public bool PasswordReset { get; init; } = false; // 256
    /// <summary>
    /// You have recently reset or enabled Steam Guard, you must wait 15 days after Steam Guard was enabled
    /// </summary>
    public bool SteamGuardEnabled { get; init; } = false; // 64
    /// <summary>
    /// Your account must be protected by Steam Guard for at least 15 days
    /// </summary>
    public bool SteamGuard15Days { get; init; } = false; // 64
    /// <summary>
    /// Your account is limited
    /// </summary>
    public bool IsLimited { get; init; } = false; // 8
    /// <summary>
    /// Your account is currently locked
    /// </summary>
    public bool IsLocked { get; init; } = false; // 4
    /// <summary>
    /// You are logging into The Market from a device that has not been protected by Steam Guard for 7 days
    /// </summary>
    public bool SteamGuard7Days { get; init; } = false; // 2048
    /// <summary>
    /// Your account is currently trade banned
    /// </summary>
    public bool TradeBanned { get; init; } = false; // 16
    /// <summary>
    /// Steam is having trouble using your browser's cookie.
    /// </summary>
    public bool CoockieProblem { get; init; } = false; // 1024
    public bool CanTrade => !PasswordReset && !SteamGuardEnabled && !SteamGuard15Days && !IsLocked && !TradeBanned && !SteamGuard7Days && !CoockieProblem;
    public bool CanMarket => !(SteamPurchase && PasswordReset && SteamGuardEnabled && SteamGuard15Days && IsLimited && IsLocked && SteamGuard7Days && TradeBanned && CoockieProblem);

    public WebTradeEligibilityReason(WebTradeEligibility web)
    {
        if (web.reason == 4)
        {
            IsLocked = true;
        }
        else if (web.reason == 8)
        {
            IsLimited = true;
        }
        else if (web.reason == 16)
        {
            TradeBanned = true;
        }
        else if (web.reason == 128)
        {
            SteamGuardEnabled = true;
            SteamGuard15Days = true;
        }
        else if (web.reason == 1024)
        {
            CoockieProblem = true;
        }
        else if (web.reason == 16416)
        {
            SteamPurchase = true;
        }
        else if (web.reason == 16420)
        {
            IsLocked = true;
            SteamPurchase = true;
        }
        else if (web.reason == 16424)
        {
            IsLimited = true;
            SteamPurchase = true;
        }
        else if (web.reason == 16484)
        {
            IsLocked = true;
            SteamPurchase = true;
            SteamGuard15Days = true;
        }
        else if (web.reason == 16544)
        {
            SteamPurchase = true;
            SteamGuardEnabled = true;
            SteamGuard15Days = true;
        }
        else if (web.reason == 16672)
        {
            SteamPurchase = true;
            PasswordReset = true;
        }
        else if (web.reason == 16800)
        {
            SteamPurchase = true;
            PasswordReset = true;
            SteamGuardEnabled = true;
            SteamGuard15Days = true;
        }
        else if (web.reason == 18720)
        {
            SteamPurchase = true;
            PasswordReset = true;
            SteamGuard7Days = true;
        }
        else if (web.reason == 1028)
        {
            CoockieProblem = true;
            IsLocked = true;
        }
        else if (web.reason == 1032)
        {
            CoockieProblem = true;
            IsLimited = true;
        }
        else if (web.reason == 1040)
        {
            CoockieProblem = true;
            TradeBanned = true;
        }
        else if (web.reason == 1152)
        {
            CoockieProblem = true;
            SteamGuardEnabled = true;
            SteamGuard15Days = true;
        }
        else if (web.reason == 17440)
        {
            CoockieProblem = true;
            SteamPurchase = true;
        }
        else if (web.reason == 17444)
        {
            CoockieProblem = true;
            IsLocked = true;
            SteamPurchase = true;
        }
        else if (web.reason == 17448)
        {
            CoockieProblem = true;
            IsLimited = true;
            SteamPurchase = true;
        }
        else if (web.reason == 17508)
        {
            CoockieProblem = true;
            IsLocked = true;
            SteamPurchase = true;
            SteamGuard15Days = true;
        }
        else if (web.reason == 17568)
        {
            CoockieProblem = true;
            SteamPurchase = true;
            SteamGuardEnabled = true;
            SteamGuard15Days = true;
        }
        else if (web.reason == 17696)
        {
            CoockieProblem = true;
            SteamPurchase = true;
            PasswordReset = true;
        }
        else if (web.reason == 17824)
        {
            CoockieProblem = true;
            SteamPurchase = true;
            PasswordReset = true;
            SteamGuardEnabled = true;
            SteamGuard15Days = true;
        }
        else if (web.reason == 19744)
        {
            CoockieProblem = true;
            SteamPurchase = true;
            PasswordReset = true;
            SteamGuard7Days = true;
        }
    }
}
