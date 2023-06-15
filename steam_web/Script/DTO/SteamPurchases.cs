using System.Text.Json;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Web;

namespace SteamWeb.Script.DTO;

public class SteamPurchases
{
    public bool success { get; private set; } = false;
    public string html { get; init; }
    public SteamPurchaseCursor cursor { get; init; } = new();
    public SteamPurchase[] list { get; private set; } = new SteamPurchase[0];

    private static string Clearing(string source)
    {
        source = source.Replace("\t", "").Replace("\r", "").Replace("\n", "");
        if (source.StartsWith(' '))
            source = source.Remove(0, 1);
        if (source.EndsWith(' '))
            source = source.Remove(source.Length - 1, 1);
        return source;
    }
    public static SteamPurchases Deserialize(string data)
    {
        SteamPurchases obj;
        try
        {
            obj = JsonSerializer.Deserialize<SteamPurchases>(HttpUtility.HtmlDecode(data)); //HttpUtility.UrlDecode(data)
            obj.success = true;
        }
        catch (Exception ex)
        {
            return new();
        }
        var html = new HtmlParser();
        var doc = html.ParseDocument($"<!DOCTYPE html><html lang=\"ru\"><table class=\"wallet_history_table\"><thead><tr><th rowspan=\"2\" class=\"wht_date\">Date</th><th rowspan=\"2\" class=\"wht_items\">Items</th><th rowspan=\"2\" class=\"wht_type\">Type</th><th rowspan=\"2\" class=\"wht_total\">Total</th><th class=\"wht_wallet\" colspan=\"2\">Wallet</th><tr><th class=\"wht_wallet_change\">Change</th><th class=\"wht_wallet_balance\">Balance</th></tr></thead> <tbody>{obj.html}</tbody></table></html>");

        var els1 = doc.GetElementsByClassName("wallet_table_row_amt_change");
        int count_parsed = 0;
        var list = new List<SteamPurchase>(els1.Length);
        foreach (var el in els1)
        {
            try
            {
                bool is_scm = false;
                var wht_date = Clearing(el.GetElementsByClassName("wht_date")[0].TextContent);
                var wht_wallet_change = Clearing(el.GetElementsByClassName("wht_wallet_change")[0].TextContent);
                var wht_wallet_balance = Clearing(el.GetElementsByClassName("wht_wallet_balance")[0].TextContent);
                var wht_type = Clearing(el.GetElementsByClassName("wht_type")[0].GetElementsByTagName("div")[0].TextContent);

                var wth_payment = new List<string>();
                var wth_payment_divs = el.GetElementsByClassName("wht_type")[0].GetElementsByTagName("div")[1].GetElementsByTagName("div");
                if (wth_payment_divs.Length > 0)
                {
                    foreach (var item in wth_payment_divs)
                    {
                        wth_payment.Add(Clearing(item.TextContent));
                    }
                }
                else wth_payment.Add(Clearing(el.GetElementsByClassName("wht_type")[0].GetElementsByTagName("div")[1].TextContent));

                string wht_total = null;
                string wht_total_payment = null;
                var wht_total_div = el.GetElementsByClassName("wht_total")[0].GetElementsByTagName("div");
                if (wht_total_div.Length > 0)
                {
                    wht_total = Clearing(wht_total_div[0].TextContent);
                    wht_total_payment = wht_total_div.Length > 1 ? Clearing(wht_total_div[1].TextContent) : null;
                }
                else wht_total = Clearing(el.GetElementsByClassName("wht_total")[0].TextContent);

                var wht_items_div = el.GetElementsByClassName("wht_items")[0].GetElementsByTagName("div");
                var wht_items = new List<string>(wht_items_div.Length + 2);
                if (wht_items_div.Length > 0)
                {
                    foreach (var item in wht_items_div)
                    {
                        var tmp = Clearing(item.TextContent);
                        if (tmp == "Steam Community Market")
                            is_scm = true;
                        wht_items.Add(tmp);
                    }
                }
                else
                {
                    var tmp = Clearing(el.GetElementsByClassName("wht_items")[0].TextContent);
                    if (tmp == "Steam Community Market")
                        is_scm = true;
                    wht_items.Add(tmp);
                }

                bool wht_refunded = el.GetElementsByClassName("wht_refunded").Length > 0 || wht_type.ToLower() == "refund";
                var purch = new SteamPurchase()
                {
                    Names = wht_items.ToArray(),
                    IsRefunded = wht_refunded,
                    Date = wht_date,
                    WalletChange = wht_wallet_change,
                    WalletBalance = wht_wallet_balance,
                    Payments = wth_payment.ToArray(),
                    Type = wht_type,
                    Total = wht_total,
                    TotalPayment = wht_total_payment,
                    IsInGamePurchase = wht_type == "In-Game Purchase",
                    IsSteamCommunity = is_scm
                };
                list.Add(purch);
                count_parsed++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.WriteLine(obj.html);
            }
        }
        obj.list = list.ToArray();
        return obj;
    }
}
