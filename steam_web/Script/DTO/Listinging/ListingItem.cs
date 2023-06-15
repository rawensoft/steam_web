using System.Text.Json.Serialization;
using System;
using SteamWeb.Extensions;
using System.Text;

namespace SteamWeb.Script.DTO.Listinging;

public record ListingItem
{
    public record Description
    {
        [JsonPropertyName("type")] public string Type { get; init; }
        [JsonPropertyName("value")] public string Value { get; init; }
    }
    public record Action
    {
        [JsonPropertyName("link")] public string Link { get; init; }
        [JsonPropertyName("name")] public string Name { get; init; }
    }
    [JsonPropertyName("currency")] public int Currency { get; init; }
    [JsonPropertyName("appid")] public uint AppID { get; init; }
    [JsonPropertyName("contextid")] public string ContextID { get; init; }
    [JsonPropertyName("remove_id")] public string Remove_ID { get; internal set; }
    [JsonPropertyName("id")] public string ID { get; init; }
    [JsonPropertyName("classid")] public string ClassID { get; init; }
    [JsonPropertyName("instanceid")] public string InstanceID { get; init; }
    [JsonPropertyName("amount")] public string Amount { get; init; }
    /// <summary>
    /// 0 - Ожидает подтверждение в SDA, 2 - Продаётся, 4 продался, 8 - создан\убран
    /// </summary>
    [JsonPropertyName("status")] public byte Status { get; init; }
    [JsonPropertyName("original_amount")] public string Original_Amount { get; init; }
    [JsonPropertyName("unowned_id")] public string Unowned_ID { get; init; }
    [JsonPropertyName("unowned_contextid")] public string Unowned_ContextID { get; init; }
    [JsonPropertyName("rollback_new_id")] public string rollback_new_id { get; init; }
    [JsonPropertyName("rollback_new_contextid")] public string rollback_new_contextid { get; init; }
    [JsonPropertyName("background_color")] public string Background_Color { get; init; }
    [JsonPropertyName("icon_url")] public string Icon_URL { get; init; }
    [JsonPropertyName("icon_url_large")] public string Icon_URL_Large { get; init; }
    [JsonPropertyName("descriptions")] public Description[] Descriptions { get; init; } = new Description[0];
    [JsonPropertyName("tradable")] public byte Tradable { get; internal set; }
    [JsonPropertyName("owner_actions")] public Action[] Owner_Actions { get; init; } = new Action[0];
    [JsonPropertyName("name")] public string Name { get; init; }
    [JsonPropertyName("name_color")] public string Name_Color { get; init; }
    [JsonPropertyName("type")] public string Type { get; init; }
    [JsonPropertyName("market_name")] public string MarketName { get; init; }
    [JsonPropertyName("market_hash_name")] public string MarketHashName { get; init; }
    [JsonPropertyName("market_fee_app")] public int MarketFeeApp { get; init; }
    [JsonPropertyName("commodity")] public int Commodity { get; init; }
    [JsonPropertyName("market_tradable_restriction")] public short MarketTradableRestriction { get; init; }
    [JsonPropertyName("market_marketable_restriction")] public short MarketMarketableRestriction { get; init; }
    [JsonPropertyName("item_expiration")] public string ItemExpiration { get; init; }
    [JsonPropertyName("marketable")] public byte Marketable { get; internal set; }
    [JsonPropertyName("owner")] public int Owner { get; init; }
    /// <summary>
    /// Ордер на продажу снять
    /// </summary>
    public bool is_canceled { get; init; } = false;
    /// <summary>
    /// Ордер на продажу создан
    /// </summary>
    public bool is_created { get; init; } = false;
    /// <summary>
    /// Предмет куплен вами
    /// </summary>
    public bool is_buyed { get; init; } = false;
    /// <summary>
    /// Предмет продан
    /// </summary>
    public bool is_selled { get; init; } = false;
    public float MarketPrice { get; internal set; } = 0;
    public string YourPrice { get; internal set; }
    public string BuyerPrice { get; internal set; }
    public float GetYourPrice => GetClearDigitString(YourPrice).ParseFloat();
    public float GetBuyerPrice => GetClearDigitString(BuyerPrice).ParseFloat();
    public bool IsTradable
    {
        get => Tradable > 0;
        set => Tradable = value ? (byte)1 : (byte)0;
    }
    public bool IsMarketable
    {
        get => Marketable > 0;
        set => Marketable = value ? (byte)1 : (byte)0;
    }

    private const char dot = '.';
    private const char comma = ',';
    private static string GetClearDigitString(string data)
    {
        var sb = new StringBuilder();
        int length = data.Length;
        for (int i = 0; i < length; i++)
        {
            var ch = data[i];
            if (char.IsDigit(ch) || ch == dot || ch == comma)
                sb.Append(ch);
        }
        if (sb[0] == comma || sb[0] == dot)
            sb.Remove(0, 1);
        if (sb[^1] == comma || sb[^1] == dot)
            sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }
}
