using SteamWeb.Script.Enums;
using System.Text.Json;
using SteamWeb.Extensions;
using AngleSharp.Html.Parser;
using SteamWeb.Script.DTO.Listinging;

namespace SteamWeb.Script.DTO.Historing;
public class Historing
{
    public bool success { get; internal set; } = false;
    public int pagesize { get; internal set; } = 0;
    public int? total_count { get; init; } = 0;
    public int start { get; init; } = 0;
    public bool IsAuthtorized { get; internal set; } = true;
    public bool IsError { get; init; } = false;
    public HistoryItem[] History { get; init; } = Array.Empty<HistoryItem>();

    public static Historing Deserialize(string data)
    {
        data = data.Replace("\"assets\":[]", "\"assets\":{}");
        var obj = JsonSerializer.Deserialize<Listing>(data)!;
        var html = new HtmlParser();
        var doc = html.ParseDocument(obj.ResultsHtml);
        var el = doc.GetElementsByClassName("market_listing_row");
        var list = new List<HistoryItem>(201);
        for (int i = 0; i < el.Length; i++)
        {
            var item = new HistoryItem();
            try
            {
                var el_price = el[i].GetElementsByClassName("market_listing_right_cell")[0].Children[0].TextContent.Replace("\n", "").Replace("\t", "");
                item.Your_Price = el_price != "" ? el_price : null;

                var el_name = el[i].Children[6].Children[0].TextContent.Replace("\n", "").Replace("\t", "");
                item.Name = el_name;

                var el_removeid = el[i].Id;
                item.RemoveID0 = el_removeid?.GetBetween("history_row_", "_");
                item.RemoveID1 = el_removeid?.GetBetween($"history_row_{item.RemoveID0}_", "_");
                if (item.RemoveID1 == "event") item.RemoveID1 = null;

                var el_type = el[i].Children[3].InnerHtml;
                item.Type = el_type.Contains("Listing canceled") ? TYPE_HISTORY_ITEM.Canceled : item.Type;
                item.Type = el_type.Contains("Listing created") ? TYPE_HISTORY_ITEM.Created : item.Type;

                var el_status = el[i].Children[0].InnerHtml.Replace("\n", "").Replace("\t", "");
                item.Type = el_status.Contains("-") ? TYPE_HISTORY_ITEM.Selled : item.Type;
                item.Type = el_status.Contains("+") ? TYPE_HISTORY_ITEM.Buyed : item.Type;

                var el_acted = el[i].Children[4].InnerHtml.Replace("\n", "").Replace("\t", "");
                item.ActedOn = el_acted == "" ? null : el_acted;

                var el_listed = el[i].Children[5].InnerHtml.Replace("\n", "").Replace("\t", "");
                item.ListedOn = el_acted == "" ? null : el_acted;

                var el_game = el[i].GetElementsByClassName("market_listing_game_name")[0].TextContent;
                item.Game = el_game;

                if (item.Type == TYPE_HISTORY_ITEM.Buyed || item.Type == TYPE_HISTORY_ITEM.Selled)
                {
                    var el_buyer = el[i].Children[3].TextContent.Replace("\n", "").Replace("\t", "").Split(':')[1];
                    var el_temp = el[i].Children[3].Children[0].Children[0].Children[0];
                    var el_buyer_url = el_temp.GetAttribute("href");
                    item.Buyer_Username = el_buyer;
                    item.Buyer_URL = el_buyer_url;
                }
                list.Add(item);
            }
            catch (Exception)
            {
                return new Historing()
                {
                    pagesize = obj.PageSize,
                    start = obj.Start,
                    success = false,
                    total_count = obj.TotalCount,
                    History = list.ToArray()
                };
            }
        }
        return new Historing()
        {
            pagesize = obj.PageSize,
            start = obj.Start,
            success = obj.Success,
            total_count = obj.TotalCount,
            History = list.ToArray()
        };
    }
}