namespace SteamWeb.Models;

public class VacGameBanModel
{
    public required uint AppId { get; init; }
    public required string Name { get; init; }
    public bool GameBan { get; internal set; } = false;
    public bool VACBan { get; internal set; } = false;
}