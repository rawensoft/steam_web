using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IEconService;
public class TradeAssets
{
	[JsonPropertyName("appid")] public uint AppId { get; init; }
	[JsonPropertyName("contextid")] public uint ContextId { get; init; }
	/// <summary>
	/// either assetid or currencyid will be set
	/// </summary>
	[JsonPropertyName("assetid")] public ulong AssetId { get; init; }
	/// <summary>
	/// either assetid or currencyid will be set
	/// </summary>
	[JsonPropertyName("currencyid")] public byte CurrencyId { get; init; }
	/// <summary>
	/// together with instanceid, uniquely identifies the display of the item
	/// </summary>
	[JsonPropertyName("classid")] public ulong ClassId { get; init; }
	/// <summary>
	/// together with classid, uniquely identifies the display of the item
	/// </summary>
	[JsonPropertyName("instanceid")] public ulong InstanceId { get; init; }
	/// <summary>
	/// the amount offered in the trade, for stackable items and currency
	/// </summary>
	[JsonPropertyName("amount")] public uint Amount { get; init; }
	/// <summary>
	/// a boolean that indicates the item is no longer present in the user's inventory
	/// </summary>
	[JsonPropertyName("missing")] public bool Missing { get; init; } = false;
	/// <summary>
	/// a string that represent Steam's determination of the item's value, in whole USD pennies. How this is determined is unknown.
	/// </summary>
	[JsonPropertyName("est_usd")] public uint EstUsd { get; init; }
}