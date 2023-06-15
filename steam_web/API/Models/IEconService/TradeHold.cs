namespace SteamWeb.API.Models.IEconService;

public record TradeHold
{
    public uint escrow_end_duration_seconds { get; init; }
    /// <summary>
    /// Не null при escrow_end_duration_seconds > 0
    /// </summary>
    public int? escrow_end_date { get; init; }
    /// <summary>
    /// Не null при escrow_end_duration_seconds > 0. Пример: 2022-12-27T12:20:00Z
    /// </summary>
    public string escrow_end_date_rfc3339 { get; init; }
}
