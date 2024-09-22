using System.Text.Json;
using SteamWeb.Extensions;
using AngleSharp.Html.Parser;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Web;
using System.Globalization;

namespace SteamWeb.Script.DTO.Listinging;
public class Listing
{
    private static readonly Regex _rgxElementId = new(@"^mybuyorder_(\d{1,20})$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(50));
    private static readonly Regex _rgxItemUrl = new(@"^https://steamcommunity.com/market/listings/(\d{1,11})/(\S+)$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));
    private static readonly Regex _rgxListingAction = new(@"^javascript:(?:RemoveMarketListing|CancelMarketListingConfirmation)\('mylisting',\s{0,1}'(\d{1,20})',\s{0,1}(\d{1,11}),\s{0,1}'(\d{1,4})',\s{0,1}'(\d{1,20})'\)$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(150));

    [JsonPropertyName("success")] public bool Success { get; init; } = false;
    [JsonPropertyName("pagesize")] public int PageSize { get; init; } = 0;
    [JsonPropertyName("total_count")] public int? TotalCount { get; init; } = 0;
    [JsonPropertyName("assets")] public Dictionary<string, Dictionary<string, Dictionary<string, ListingItem>>> Assets { get; init; } = new(1);
    [JsonPropertyName("start")] public int Start { get; init; } = 0;
    [JsonPropertyName("num_active_listings")] public int NumActiveListings { get; init; } = 0;
    [JsonPropertyName("results_html")] public string ResultsHtml { get; init; } = string.Empty;
    [JsonPropertyName("Orders")] public OrderItem[] Orders { get; set; } = Array.Empty<OrderItem>();

    /// <summary>
    /// Возвращает все предметы с указанной игры, которые находятся в листинге
    /// </summary>
    /// <param name="app_id">Id игры, предметы которой нужно найти</param>
    /// <returns>Массив с предметами</returns>
    public ListingItem[] GetItemsByAppID(int app_id)
    {
        if (!Assets.TryGetValue(app_id.ToString(), out var app))
            return Array.Empty<ListingItem>();

        var count = app.Select(x => x.Value).Sum(x => x.Count);
        var list = new List<ListingItem>(count + 1);
        foreach (var (_, context) in app)
        {
            var items = context.Select(x => x.Value);
            list.AddRange(items);
        }
        return list.ToArray();
    }
    /// <summary>
    /// Возвращает все предметы с указанной игры, которые находятся в листинге
    /// </summary>
    /// <param name="app_id">Id игры, предметы которой нужно найти</param>
    /// <param name="include_with_confirmation">Включать ли предметы, которые ожидают подтверждения</param>
    /// <returns>Массив с предметами</returns>
    public ListingItem[] GetItemsByAppID(int app_id, bool include_with_confirmation)
    {
        if (!Assets.TryGetValue(app_id.ToString(), out var app))
            return Array.Empty<ListingItem>();

        var count = app.Select(x => x.Value).Sum(x => x.Count);
        var list = new List<ListingItem>(count + 1);
        foreach (var (_, context) in app)
        {
            var items = context.Where(x => include_with_confirmation || (!include_with_confirmation && x.Value.Status != 0))
                .Select(x => x.Value);
            list.AddRange(items);
        }
        return list.ToArray();
    }
    /// <summary>
    /// Возвращает все предметы, которые находятся в листинге
    /// </summary>
    /// <param name="include_with_confirmation">Включать ли предметы, которые ожидают подтверждения</param>
    /// <returns>Массив с предметами</returns>
    public ListingItem[] GetItems(bool include_with_confirmation)
    {
        var count = Assets.Select(x => x.Value.Sum(x => x.Value.Count)).Sum();
        var list = new List<ListingItem>(count + 1);
        foreach (var (_, app) in Assets)
        {
            foreach (var (_, context) in app)
            {
                var items = context.Where(x => include_with_confirmation || (!include_with_confirmation && x.Value.Status != 0))
                    .Select(x => x.Value);
                list.AddRange(items);
            }
        }
        return list.ToArray();
    }
    /// <summary>
    /// Возвращает все предметы, которые находятся в листинге
    /// </summary>
    /// <returns>Массив с предметами</returns>
    public ListingItem[] GetItems()
    {
        var count = Assets.Select(x => x.Value.Sum(x => x.Value.Count)).Sum();
        var list = new List<ListingItem>(count + 1);
        foreach (var (_, app) in Assets)
        {
            foreach (var (_, context) in app)
            {
                var items = context.Select(x => x.Value);
                list.AddRange(items);
            }
        }
        return list.ToArray();
    }

    /// <summary>
    /// Производит парсинг json данных ответа на запрос в <see cref="Ajax.market_mylistings(SteamWeb.Models.DefaultRequest, int, int)"/>
    /// </summary>
    /// <exception cref="RegexMatchTimeoutException"/>
    /// <exception cref="InvalidOperationException"/>
    public static Listing Deserialize(string data)
    {
        const string _assetArray_str = "\"assets\":[]";
        const string _assetObject_str = "\"assets\":{}";
        const string _market_listing_row_str = "market_listing_row";
        const string _item_market_action_button_str = "item_market_action_button";
        const string _href_str = "href";
        const string _market_listing_my_price_str = "market_listing_my_price";
        const string _market_listing_buyorder_qty_str = "market_listing_buyorder_qty";
        const string _market_listing_price_str = "market_listing_price";
        const string _market_listing_item_name_block_str = "market_listing_item_name_block";
        const string _span_tag_str = "SPAN";
        const string _market_listing_game_name_str = "market_listing_game_name";
        const string _market_listing_item_name_str = "market_listing_item_name";
        const string _market_listing_listed_date_str = "market_listing_listed_date";
        const string _format1 = "d MMM";
        const string _format2 = "MMM d";

        var index = data.IndexOf(_assetArray_str);
        if (index != -1)
            data = data.Remove(index, _assetArray_str.Length)
                .Insert(index, _assetObject_str);

        var obj = JsonSerializer.Deserialize<Listing>(data, Steam.JsonOptions)!;
        var html = new HtmlParser();
        var doc = html.ParseDocument("<html lang=\"en\"><body>" + obj.ResultsHtml + "</body></html>");
        var el = doc.GetElementsByClassName(_market_listing_row_str);
        var items = obj.GetItems();
        var buyOrders = new List<OrderItem>(el.Length);
        var cc = new CultureInfo("en-US");
        foreach (var item in el)
        {
            var matchBuyOrder = _rgxElementId.Match(item.Id!);
            if (matchBuyOrder.Success)
            {
                string? el_price = null;
                string? el_quantity = null;
                string? el_name = null;
                string? el_game = null;
                string? el_url = null;
                uint? el_appid = null;
                string? el_market_hash_name = null;

                var market_listing_my_prices = item.GetElementsByClassName(_market_listing_my_price_str);
                foreach (var market_listing_my_price in market_listing_my_prices)
                {
                    if (market_listing_my_price.ClassList.Contains(_market_listing_buyorder_qty_str))
                        el_quantity = market_listing_my_price.TextContent.GetClearWebString()!;
                    else
                    {
                        var market_listing_price_s = market_listing_my_price.GetElementsByClassName(_market_listing_price_str).First();
                        el_price = market_listing_price_s.TextContent.GetClearWebString()!;
                        var splitted = el_price.Split('@');
                        if (splitted.Length == 2)
                            el_price = splitted[1];
                    }
                }

                var market_listing_item_name_block = item.GetElementsByClassName(_market_listing_item_name_block_str).First();
                foreach (var span in market_listing_item_name_block.Children)
                {
                    if (span.TagName == _span_tag_str)
                    {
                        if (span.ClassName == _market_listing_game_name_str)
                            el_game = span.TextContent.GetClearWebString()!;
                        else if (span.ClassName == _market_listing_item_name_str)
                        {
                            var a = span.Children.FirstOrDefault();
                            if (a != null)
                            {
                                el_name = a.TextContent.GetClearWebString();
                                el_url = a.GetAttribute(_href_str);
                            }
                        }
                    }
                }

                var mathUrl = _rgxItemUrl.Match(el_url!);
                if (mathUrl.Success)
                {
                    el_appid = mathUrl.Groups[1].Value.ParseUInt32();
                    el_market_hash_name = HttpUtility.UrlDecode(mathUrl.Groups[2].Value);
                }

                var order = new OrderItem
                {
                    Count = el_quantity.ParseUInt16(),
                    Game = el_game!,
                    Name = el_name!,
                    Id = matchBuyOrder.Groups[1].Value.ParseUInt64(),
                    Price = el_price!,
                    AppId = el_appid ?? 0,
                    MarketHashName = el_market_hash_name ?? string.Empty,
                };
                buyOrders.Add(order);
                continue;
            }

            var item_market_action_button = item.GetElementsByClassName(_item_market_action_button_str).First();
            var href = item_market_action_button.GetAttribute(_href_str)!;
            var match = _rgxListingAction.Match(href);
            if (!match.Success)
                continue;

            ulong removeId = match.Groups[1].Value.ParseUInt64();
            uint appId = match.Groups[2].Value.ParseUInt32();
            byte contextId = match.Groups[3].Value.ParseByte();
            ulong assetId = match.Groups[4].Value.ParseUInt64();

            var listing = items.Where(x => x.AppId == appId)
                .Where(x => x.ContextId == contextId)
                .FirstOrDefault(x => x.Id == assetId);
            if (listing == default)
                continue;
            listing.RemoveId = removeId;

            var market_listing_price = item.GetElementsByClassName(_market_listing_price_str).First();
            foreach (var children in market_listing_price.Children.First().Children)
            {
                if (children.TagName == _span_tag_str)
                {
                    if (listing.BuyerPrice == null)
                        listing.BuyerPrice = children.TextContent.GetClearWebString()!;
                    else if (listing.YourPrice == null)
                    {
                        var str = children.TextContent.GetClearWebString()!.Remove(0, 1);
                        listing.YourPrice = str.Remove(str.Length - 1, 1);
                    }
                    else
                        break;
                }
            }

            var market_listing_listed_date = item.GetElementsByClassName(_market_listing_listed_date_str).First();
            string listedOn = market_listing_listed_date.TextContent.GetClearWebString()!;
            if (!DateOnly.TryParseExact(listedOn, new string[] { _format1, _format2 }, cc, DateTimeStyles.None, out var listedDate))
                listedDate = DateOnly.MinValue;
            listing.ListingDay = listedDate;

            var market_listing_game_name = item.GetElementsByClassName(_market_listing_game_name_str).First();
            listing.AppName = market_listing_game_name.TextContent.GetClearWebString()!;
        }
        obj.Orders = buyOrders.ToArray();
        return obj;
    }
}