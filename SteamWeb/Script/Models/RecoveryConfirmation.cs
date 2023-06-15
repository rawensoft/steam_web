using SteamWeb.Script.DTO;

namespace SteamWeb.Script.Models;

public record RecoveryConfirmation : Response
{
    public bool @continue { get; init; } = false;
}
