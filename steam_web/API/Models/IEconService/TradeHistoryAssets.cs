using System.Text.Json.Serialization;
namespace SteamWeb.API.Models.IEconService;
public class TradeHistoryAssets
{
	[JsonPropertyName("appid")] public uint AppId { get; init; }
	[JsonPropertyName("context")] public ulong Context { get; init; }
	[JsonPropertyName("assetid")] public ulong AssetId { get; init; }
	[JsonPropertyName("amount")] public uint Amount { get; init; }
	/// <summary>
	/// together with instanceid, uniquely identifies the display of the item
	/// </summary>
	[JsonPropertyName("classid")] public ulong ClassId { get; init; }
	/// <summary>
	/// together with classid, uniquely identifies the display of the item
	/// </summary>
	[JsonPropertyName("instanceid")] public ulong InstanceId { get; init; }
	/// <summary>
	/// the asset ID given to the item after the trade completed
	/// </summary>
	[JsonPropertyName("new_assetid")] public ulong NewAssetId { get; init; }
	/// <summary>
	/// the context ID the item was placed in
	/// </summary>
	[JsonPropertyName("new_contextid")] public ulong NewContextId { get; init; }
	/// <summary>
	/// if the trade has been rolled back, the new asset ID given in the rollback
	/// </summary>
	[JsonPropertyName("rollback_new_assetid")] public ulong RollbackNewAssetId { get; init; }
	/// <summary>
	/// if the trade has been rolled back, the context ID the new asset was placed in
	/// </summary>
	[JsonPropertyName("rollback_new_contextid")] public ulong RollbackNewContextId { get; init; }
}