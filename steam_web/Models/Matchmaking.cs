namespace SteamWeb.Models;
public class Matchmaking
{
    public const string DefaultPlate = "&nbsp;";
    public bool Success { get; init; } = false;
    public string Error { get; init; } = null;
    public MatchmakingMode Competitive { get; init; } = new();
    public MatchmakingMode Wingman { get; init; } = new();
    public DangerZoneMode DangerZone { get; init; } = new();
    public ModeLastMatch[] LastMatches { get; internal set; } = new ModeLastMatch[0];

    public Matchmaking() { }
    public Matchmaking(string Error) => this.Error = Error;
}
