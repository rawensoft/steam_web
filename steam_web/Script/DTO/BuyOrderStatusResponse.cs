using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO;
public class BuyOrderStatusResponse : ResponseSuccess
{
	[JsonPropertyName("active")] public byte Active { get; init; }
	[JsonPropertyName("purchase_amount_text")] public string? PurchaseAmountText { get; init; }
	[JsonPropertyName("purchased")] public ushort Purchased { get; init; }
	[JsonPropertyName("purchases")] public List<BuyOrderStatusAsset> Purchases { get; init; } = new(2);
	[JsonPropertyName("quantity")] public ushort Quantity { get; init; }
	[JsonPropertyName("quantity_remaining")] public ushort QuantityRemaining { get; init; }
}