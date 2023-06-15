namespace SteamWeb.Script.DTO;

public record SteamLoyaltyStore
{
    public bool success { get; internal set; } = false;
    public string webapi_token { get; init; }
    public Conversion points_conversion { get; init; } = new();
    public class Conversion
    {
        public long? unit_spend { get; init; }
        public string points { get; init; }
    }
}
