using System.Collections.Generic;
using System.Text.Json;
using System;
using SteamWeb.Extensions;
using AngleSharp.Html.Parser;
using SteamWeb.Script.DTO;

namespace SteamWeb.Script.DTO.Listinging;

public record Listing
{
    public bool success { get; init; } = false;
    public int pagesize { get; init; } = 0;
    public int? total_count { get; init; } = 0;
    public Dictionary<string, Dictionary<string, Dictionary<string, ListingItem>>> assets { get; init; } = new(1);
    public int start { get; init; } = 0;
    public int num_active_listings { get; init; } = 0;
    public string results_html { get; init; }
    public OrderItem[] Orders { get; internal set; } = new OrderItem[0];

    public ListingItem[] GetItemsByAppID(int app_id, bool include_with_confirmation = false)
    {
        if (!assets.ContainsKey(app_id.ToString())) return new ListingItem[0];
        var app = assets[app_id.ToString()];
        foreach (var context in app)
        {
            var items = new List<ListingItem>(1000);
            foreach (var item in context.Value)
            {
                if (!include_with_confirmation && item.Value.Status == 0) continue;
                items.Add(item.Value);
            }
            return items.ToArray();
        }
        return new ListingItem[0];
    }
    public ListingItem[] GetItems(bool include_with_confirmation = false)
    {
        var items = new List<ListingItem>(1000);
        foreach (var app in assets)
        {
            foreach (var context in app.Value)
            {
                foreach (var item in context.Value)
                {
                    if (!include_with_confirmation && item.Value.Status == 0) continue;
                    items.Add(item.Value);
                }
            }
        }
        return items.ToArray();
    }
    public static Listing Deserialize(string data)
    {
        if (data.Contains("\"assets\":[]")) data = data.Replace("\"assets\":[]", "\"assets\":{}");
        var obj = JsonSerializer.Deserialize<Listing>(data);

        var html = new HtmlParser();
        var doc = html.ParseDocument($"<!DOCTYPE html><html class=\"responsive\" lang=\"en\"><body class=\"responsive_page\">{obj.results_html}</body></html>");
        var el = doc.GetElementsByClassName("market_listing_row");
        var items = obj.GetItems(true);
        var buyOrders = new List<OrderItem>(el.Length);
        for (int i = 0; i < el.Length; i++)
        {
            if (el[i].Id.ToLower().Contains("mybuyorder"))
            {
                try
                {
                    var el_price = el[i].Children[2].TextContent;
                    var el_name = el[i].Children[4].Children[0].TextContent;
                    var el_game = el[i].Children[4].Children[2].TextContent;
                    var el_count = el[i].Children[3].TextContent.GetOnlyDigit();
                    var el_id = el[i].GetElementsByClassName("item_market_action_button item_market_action_button_edit nodisable")[0].GetAttribute("href").GetBetween("'", "'");

                    var order = new OrderItem();
                    order.Game = el_game.Replace("\n", "").Replace("\t", "").Replace("\r", "");
                    order.PriceFull = el_price.Replace("\n", "").Replace("\t", "").Replace("\r", "").Split('@')[1];
                    order.Name = el_name.Replace("\n", "").Replace("\t", "").Replace("\r", "");
                    order.ID = ulong.Parse(el_id.GetOnlyDigit());
                    order.Count = int.Parse(el_count.GetOnlyDigit());
                    buyOrders.Add(order);
                }
                catch (Exception ex)
                {
                }
                continue;
            }
            try
            {
                var tmp1 = el[i].GetElementsByClassName("market_listing_my_price");
                var tmp2 = tmp1[0];
                var tmp3 = tmp2.Children;
                var tmp4 = tmp3[0];
                var tmp5 = tmp4.Children;
                var tmp6 = tmp5[0];
                var tmp7 = tmp6.Children;
                var tmp8 = tmp7[0];
                var el_price = tmp8.TextContent.Replace("\n", "").Replace("\t", "");
                var splitted = el_price.Split('(');
                items[i].BuyerPrice = splitted[0];
                items[i].YourPrice = splitted[1].Replace(")", "");
            }
            catch (Exception ex)
            { }
            try
            {
                var tmp1 = el[i].GetElementsByClassName("market_listing_cancel_button");
                var tmp2 = tmp1[0];
                var tmp3 = tmp2.Children;
                var tmp4 = tmp2.Children[0];
                var el_remove = tmp4.OuterHtml.Replace(" ", "");
                items[i].Remove_ID = el_remove.GetBetween("'mylisting','", "',");
            }
            catch (Exception ex)
            { }
        }
        //obj.results_html = null;
        obj.Orders = buyOrders.ToArray();
        return obj;
    }
}
