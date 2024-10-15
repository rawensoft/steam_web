using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO;
public class CancelTrade
{
	[JsonPropertyName("success")] public byte Success { get; set; }
	/// <summary>
	/// a unique identifier for the trade offer
	/// </summary>
	[JsonPropertyName("tradeofferid")] public ulong TradeOfferId { get; init; }
}