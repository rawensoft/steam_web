namespace SteamWeb.Script.DTO;

public record CreateBuyOrder
{
    /// <summary>
    /// 16 - проблемы с серверами
    /// </summary>
    public int success { get; init; } = 0;
    public string message { get; init; }
}
