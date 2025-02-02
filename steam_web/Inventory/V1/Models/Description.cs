using System.Text.Json.Serialization;

namespace SteamWeb.Inventory.V1.Models;
public class Description
{
    [JsonPropertyName("actions")]
    public Action[] Actions { get; init; } = Array.Empty<Action>();

	[JsonPropertyName("appid")]
	public uint AppId { get; init; }

	[JsonPropertyName("background_color")]
	public string BackgroundColor { get; init; } = string.Empty;

	[JsonPropertyName("classid")]
	public ulong ClassId { get; init; }

	[JsonPropertyName("commodity")]
	public ulong Commodity { get; init; }

	[JsonPropertyName("currency")]
	public ulong Currency { get; init; }

	[JsonPropertyName("descriptions")]
	public DescriptionItem[] Descriptions { get; init; } = Array.Empty<DescriptionItem>();

	[JsonPropertyName("icon_url")]
	public string IconUrl { get; init; }

	[JsonPropertyName("icon_url_large")]
	public string IconUrlLarge { get; init; }

	[JsonPropertyName("instanceid")]
	public ulong InstanceId { get; init; }

	[JsonPropertyName("market_actions")]
	public Action[] MarketActions { get; init; } = Array.Empty<Action>();

	[JsonPropertyName("market_hash_name")]
	public string MarketHashName { get; init; }

	[JsonPropertyName("market_name")]
	public string MarketName { get; init; }

	[JsonPropertyName("market_tradable_restriction")]
	public sbyte MarketTradableRestriction { get; init; }

	[JsonPropertyName("marketable")]
	public sbyte Marketable { get; init; }

	[JsonPropertyName("name")]
	public string Name { get; init; }

	[JsonPropertyName("name_color")]
	public string NameColor { get; init; }

	[JsonPropertyName("tags")]
	public Tag[] Tags { get; init; } = Array.Empty<Tag>();

	[JsonPropertyName("tradable")]
	public sbyte Tradable { get; init; }

	[JsonPropertyName("type")]
	public string Type { get; init; }
}