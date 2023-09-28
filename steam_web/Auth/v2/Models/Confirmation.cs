using SteamWeb.Auth.v1.Enums;

namespace SteamWeb.Auth.v2.Models;

public class Confirmation
{
    public int creation_time { get; init; }
    public ConfirmationType type { get; init; }
    public string type_name { get; init; }
    public string id { get; init; }
    public ulong creator_id { get; init; }
    public string nonce { get; init; }
    public bool multi { get; init; } = false;
    public string headline { get; init; }
    public string[] summary { get; init; } = new string[0];
    public string[] warn { get; init; } = new string[0];
}

