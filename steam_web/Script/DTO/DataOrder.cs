using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO;
public class DataOrder: ResponseSuccess
{
	[JsonPropertyName("buy_orderid")] public ulong BuyOrderId { get; init; }
	[JsonPropertyName("message")] public string? Message { get; init; }
}