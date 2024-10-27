using System.Text.Json.Serialization;
namespace SteamWeb.API.Models.IEconService;
public class TradeOffer
{
	[JsonPropertyName("offer")] public Trade? Offer { get; init; }
}