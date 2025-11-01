namespace SteamWeb.API.Models.ISteamUser;
public class PlayerBan
{
    public ulong SteamId { get; init; }
    public bool CommunityBanned { get; init; } = false;
    public bool VACBanned { get; init; } = false;
    public int NumberOfVACBans { get; init; }
    public int DaysSinceLastBan { get; init; }
    public int NumberOfGameBans { get; init; }
    public string? EconomyBan { get; init; } = null;
}