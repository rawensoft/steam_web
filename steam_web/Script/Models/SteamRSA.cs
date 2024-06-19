namespace SteamWeb.Script.Models;
public class SteamRSA
{
    public bool success { get; init; } = false;
    public string? publickey_mod { get; init; } = null;
    public string? publickey_exp { get; init; } = null;
    public string? timestamp { get; init; } = null;
    public string? token_gid { get; init; } = null;
}
