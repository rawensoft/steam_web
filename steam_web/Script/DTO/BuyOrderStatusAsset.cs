using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO;
public class BuyOrderStatusAsset
{
	[JsonPropertyName("accountid_seller")] public uint AccountidSeller { get; init; }
	[JsonPropertyName("appid")] public uint AppId { get; init; }
	[JsonPropertyName("assetid")] public ulong AssetId { get; init; }
	[JsonPropertyName("contextid")] public uint ContextId { get; init; }
	[JsonPropertyName("currency")] public byte Currency { get; init; }
	[JsonPropertyName("listingid")] public long ListingId { get; init; }
	[JsonPropertyName("price_fee")] public uint PriceFee { get; init; }
	[JsonPropertyName("price_subtotal")] public uint PriceSubTotal { get; init; }
	[JsonPropertyName("price_total")] public uint PriceTotal { get; init; }
}