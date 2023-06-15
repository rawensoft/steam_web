namespace SteamWeb.Models;
public class AccountMain
{
    public const string DefaultPlate = "None";
    public bool Success { get; init; } = false;
    public string Error { get; init; } = null;
    public SubscriptionServiceActiveUntil SubscriptionService { get; init; } = new();

    public string PrimeAccountStatusActiveSince { get; internal set; } = DefaultPlate;
    public string LoggedOutOfCSGO { get; internal set; } = DefaultPlate;
    public string LaunchedCSGOUsingSteamClient { get; internal set; } = DefaultPlate;
    public string StartedPlayingCSGO { get; internal set; } = DefaultPlate;
    public string FirstCounterStrikeFranchiseGame { get; internal set; } = DefaultPlate;
    public string LaunchedCSGOUsingPerfectWorldCSGOLauncher { get; internal set; } = DefaultPlate;

    public string LastKnownIPAddress { get; internal set; } = DefaultPlate;
    public string EarnedAServiceMedal { get; internal set; } = DefaultPlate;
    public ushort CSGOProfileRank { get; internal set; } = 0;
    public ushort ExperiencePointsEarnedTowardsNextRank { get; internal set; } = 0;
    public string AntiAddictionOnlineTime { get; internal set; } = DefaultPlate;

    public AccountMain() { }
    public AccountMain(string Error) => this.Error = Error;
}
