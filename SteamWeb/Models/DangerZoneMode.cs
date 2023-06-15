namespace SteamWeb.Models;
public class DangerZoneMode
{
    public ushort SoloWins { get; internal set; }
    public ushort SquadWins { get; internal set; }
    public ushort MatchesPlayed { get; internal set; }
    public string LastMatch { get; internal set; }
}
