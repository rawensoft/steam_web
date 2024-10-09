using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO.ItemRender;

public class ItemRenderListing
{
    [JsonPropertyName("listingid")] public ulong ListingId { get; init; }
    [JsonPropertyName("price")] public uint Price { get; init; }
    [JsonPropertyName("fee")] public uint Fee { get; init; }
    [JsonIgnore] public uint TotalPrice => Price + Fee;
    [JsonPropertyName("publisher_fee_app")] public uint PublisherFeeApp { get; init; }
    [JsonPropertyName("publisher_fee_percent")] public decimal PublisherFeePercent { get; init; }
    /// <summary>
    /// Начинается от 2000
    /// </summary>
    [JsonPropertyName("currencyid")] public ushort CurrencyId { get; init; }
    [JsonPropertyName("steam_fee")] public uint SteamFee { get; init; }
    [JsonPropertyName("publisher_fee")] public uint PublisherFee { get; init; }
    [JsonPropertyName("converted_price")] public uint ConvertedPrice { get; init; }
    [JsonPropertyName("converted_fee")] public uint ConvertedFee { get; init; }
    /// <summary>
    /// Начинается от 2000
    /// </summary>
    [JsonPropertyName("converted_currencyid")] public uint ConvertedCurrencyId { get; init; }
    [JsonPropertyName("converted_steam_fee")] public uint ConvertedSteamFee { get; init; }
    [JsonPropertyName("converted_publisher_fee")] public uint ConvertedPublisherFee { get; init; }
    [JsonPropertyName("converted_price_per_unit")] public uint ConvertedPricePerUnit { get; init; }
    [JsonPropertyName("converted_fee_per_unit")] public uint ConvertedFeePerUnit { get; init; }
    [JsonPropertyName("converted_steam_fee_per_unit")] public uint ConvertedSteamFeePerUnit { get; init; }
    [JsonPropertyName("converted_publisher_fee_per_unit")] public uint ConvertedPublisherFeePerUnit { get; init; }
    [JsonPropertyName("asset")] public ItemRenderListingAsset Asset { get; init; } = new();
}
