namespace SteamWeb.Script.DTO;

public record ItemGroup
{
    public string avatarHash { get; init; }
    public string name { get; init; }
    public string steamid { get; init; }
}
