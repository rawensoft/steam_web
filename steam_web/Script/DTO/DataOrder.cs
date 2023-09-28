namespace SteamWeb.Script.DTO;

public class DataOrder: Success
{
	public ulong buy_orderid { get; init; }
	public string message { get; init; }
}
