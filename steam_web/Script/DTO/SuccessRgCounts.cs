namespace SteamWeb.Script.DTO;

public class SuccessRgCounts
{
    public int success { get; init; } = 0;
    public RgCounts rgCounts { get; init; } = new();
}
