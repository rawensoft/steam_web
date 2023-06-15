using AngleSharp.Html.Parser;
using SteamWeb.Extensions;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;
using SteamWeb.Script.DTO;

namespace SteamWeb.Models;
public sealed class MarketItem
{
    public string Data { get; internal set; } = null;
    public bool IsError { get; internal set; } = false;
    public bool IsZeroItemsListed { get; internal set; } = false;
    public bool IsTooManyRequests { get; internal set; } = false;
    public string Username { get; internal set; }
    public bool LoggedIn { get; internal set; } = false;
    public string Wallet { get; internal set; }
    public string SessionID { get; internal set; }
    public string SteamID { get; internal set; }
    public string Language { get; internal set; }
    public WalletInfo WalletInfo { get; internal set; } = new();
    public bool RequiresBillingInfo { get; internal set; } = false;
    public bool HasBillingStates { get; internal set; } = false;
    public string CountryCode { get; internal set; }
    public ItemPriceHistory[] HistoryGraph { get; internal set; } = new ItemPriceHistory[0];
    public string item_nameid { get; internal set; }

    public ItemPriceHistory[] GetHistoryGraphByHours(int hours = 24)
    {
        if (hours == 0) return new ItemPriceHistory[0];
        var list = new List<ItemPriceHistory>(HistoryGraph.Length);
        var ticks_one_hour = 36000000000;
        var now_ticks = DateTime.Now.Ticks;
        var ticks_to_scan = (now_ticks - (ticks_one_hour * hours));
        for (int i = 0; i < HistoryGraph.Length; i++)
        {
            var item = HistoryGraph[i];
            var ticks_item = item.Time.Ticks;
            if (ticks_item < ticks_to_scan)
                break;
            list.Add(item);
        }
        return list.ToArray();
    }

    public static MarketItem Deserialize(string html)
    {
        var item = new MarketItem();
        item.CountryCode = html.GetBetween("g_strCountryCode = \"", "\";");
        item.HasBillingStates = html.Contains("g_bHasBillingStates = true;");
        item.LoggedIn = html.Contains("g_bLoggedIn = true;");
        try
        {
            item.RequiresBillingInfo = html.Contains("g_bRequiresBillingInfo = true;");
        }
        catch (Exception) { }
        item.item_nameid = html.GetBetween("Market_LoadOrderSpread(", ");")?.Replace(" ", "");
        item.Language = html.GetBetween("g_strLanguage = \"", "\";");
        item.SessionID = html.GetBetween("g_sessionID = \"", "\";");
        item.SteamID = html.GetBetween("g_steamID = \"", "\";");
        var options = new JsonSerializerOptions()
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };
        try
        {
            string raw = html.GetBetween("var g_rgWalletInfo = ", ";");
            if (raw != null)
                item.WalletInfo = JsonSerializer.Deserialize<WalletInfo>(raw, options);
        }
        catch (Exception) { }

        HtmlParser parser = new HtmlParser();
        var doc = parser.ParseDocument(html);

        var element = doc.GetElementById("market_buyorder_dialog_myaccountname");
        item.Username = element?.TextContent;

        element = doc.GetElementById("header_wallet_balance");
        item.Wallet = element?.TextContent;

        try
        {
            var list = JsonSerializer.Deserialize<JsonElement[][]>(html.GetBetween("var line1=", ";"));
            item.HistoryGraph = PriceHistory.SortPriceHistory(list);
        }
        catch (Exception ex) { }
        return item;
    }
}
