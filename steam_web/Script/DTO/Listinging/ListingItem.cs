using System.Text.Json.Serialization;
using SteamWeb.Extensions;
using SteamWeb.Script.Enums;

namespace SteamWeb.Script.DTO.Listinging;
public class ListingItem
{
    public class Description
    {
        [JsonPropertyName("type")] public string Type { get; init; }
        [JsonPropertyName("value")] public string Value { get; init; }
    }
    public class Action
    {
        [JsonPropertyName("link")] public string Link { get; init; }
        [JsonPropertyName("name")] public string Name { get; init; }
    }

    [JsonPropertyName("currency")] public byte Currency { get; init; }
    [JsonPropertyName("appid")] public uint AppId { get; init; }
    [JsonPropertyName("contextid")] public byte ContextId { get; init; }
    [JsonPropertyName("id")] public ulong Id { get; init; }
    [JsonPropertyName("classid")] public ulong ClassId { get; init; }
    [JsonPropertyName("instanceid")] public uint InstanceId { get; init; }
    [JsonPropertyName("amount")] public ushort Amount { get; init; }
    [JsonPropertyName("status")] public LISTING_STATUS Status { get; init; }
    [JsonPropertyName("original_amount")] public ushort OriginalAmount { get; init; }
    [JsonPropertyName("unowned_id")] public ulong UnownedId { get; init; }
    [JsonPropertyName("unowned_contextid")] public byte UnownedContextId { get; init; }
    [JsonPropertyName("rollback_new_id")] public ulong? RollbackNewId { get; init; }
    [JsonPropertyName("rollback_new_contextid")] public byte? RollbackNewContextId { get; init; }
    [JsonPropertyName("background_color")] public string BackgroundColor { get; init; } = string.Empty;
    [JsonPropertyName("icon_url")] public string? IconUrl { get; init; }
    [JsonPropertyName("icon_url_large")] public string? IconUrlLarge { get; init; }
    [JsonPropertyName("descriptions")] public Description[] Descriptions { get; init; } = Array.Empty<Description>();
    [JsonPropertyName("owner_actions")] public Action[] OwnerActions { get; init; } = Array.Empty<Action>();
    [JsonPropertyName("name")] public string Name { get; init; }
    [JsonPropertyName("name_color")] public string NameColor { get; init; }
    [JsonPropertyName("type")] public string Type { get; init; }
    [JsonPropertyName("market_name")] public string MarketName { get; init; }
    [JsonPropertyName("market_hash_name")] public string MarketHashName { get; init; }
    [JsonPropertyName("market_fee_app")] public uint? MarketFeeApp { get; init; }
    [JsonPropertyName("commodity")] public byte Commodity { get; init; }
    [JsonPropertyName("market_tradable_restriction")] public sbyte MarketTradableRestriction { get; init; }
    [JsonPropertyName("market_marketable_restriction")] public sbyte MarketMarketableRestriction { get; init; }
    [JsonPropertyName("item_expiration")] public string? ItemExpiration { get; init; }
    [JsonPropertyName("owner")] public uint? Owner { get; init; }
    [JsonPropertyName("marketable")] public byte Marketable { get; set; }
    [JsonPropertyName("tradable")] public byte Tradable { get; set; }
    [JsonPropertyName("remove_id")] public ulong RemoveId { get; set; }
    [JsonPropertyName("your_price")] public string? YourPrice { get; set; }
    [JsonPropertyName("buyer_price")] public string? BuyerPrice { get; set; }
    /// <summary>
    /// День и месяц, когда произошёл листинг предмета; если не удалось узнать <see cref="DateOnly.MinValue"/>
    /// </summary>
    [JsonPropertyName("listing_day")] public DateOnly ListingDay { get; set; } = DateOnly.MinValue;
    [JsonPropertyName("app_name")] public string? AppName { get; set; }
    [JsonIgnore]
    public bool IsTradable
    {
        get => Tradable > 0;
        set => Tradable = value ? (byte)1 : (byte)0;
    }
    [JsonIgnore]
    public bool IsMarketable
    {
        get => Marketable > 0;
        set => Marketable = value ? (byte)1 : (byte)0;
    }

    /// <summary>
    /// Парсит строку цены, превращая её, в decimal число
    /// </summary>
    /// <returns>-1 в случае, если неудалось спарсить, либо цена в decimal формате</returns>
    public decimal GetDecimalYourPrice() => YourPrice.ToDecimalPrice();
    /// <summary>
    /// Парсит строку цены, превращая её, в uint32 число
    /// </summary>
    /// <returns>0 в случае, если неудалось спарсить, либо цена в decimal формате</returns>
    public uint GetUInt32YourPrice() => YourPrice.ToUInt32Price();
    /// <summary>
    /// Парсит строку цены, превращая её, в decimal число
    /// </summary>
    /// <returns>-1 в случае, если неудалось спарсить, либо цена в decimal формате</returns>
    public decimal GetDecimalBuyerPrice() => BuyerPrice.ToDecimalPrice();
    /// <summary>
    /// Парсит строку цены, превращая её, в uint32 число
    /// </summary>
    /// <returns>0 в случае, если неудалось спарсить, либо цена в decimal формате</returns>
    public uint GetUInt32BuyerPrice() => BuyerPrice.ToUInt32Price();
}