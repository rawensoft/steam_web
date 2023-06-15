using Response = SteamWeb.Script.DTO.Response;
namespace SteamWeb.Script.Models;

public record RecoveryConfirmation : Response
{
    public bool @continue { get; init; } = false;
}
