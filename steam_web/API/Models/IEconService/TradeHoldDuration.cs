using System.Text.Json.Serialization;
namespace SteamWeb.API.Models.IEconService;
public class TradeHoldDuration
{
	[JsonPropertyName("my_escrow")] public TradeHold? MyEscrow { get; init; }
	[JsonPropertyName("their_escrow")] public TradeHold? TheirEscrow { get; init; }
	[JsonPropertyName("both_escrow")] public TradeHold? BothEscrow { get; init; }
}