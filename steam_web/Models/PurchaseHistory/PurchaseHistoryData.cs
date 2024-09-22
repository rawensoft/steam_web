using System.Text.Json;
using SteamWeb.Extensions;
using AngleSharp.Html.Parser;
using System.Globalization;
using System.Web;
using System.Collections.Immutable;
using AngleSharp.Dom;
using AngleSharp.Text;
using SteamWeb.Script.DTO;
using SteamWeb.Script;
using SteamWeb.Enums;

namespace SteamWeb.Models.PurchaseHistory;
public class PurchaseHistoryData
{
    public bool Success { get; init; } = false;
    public string? Error { get; init; }
    public bool IsError => Error != null;
    public required ImmutableList<PurchaseHistoryModel> History { get; init; }
    public PurchaseHistoryCursorModel? Cursor { get; init; }
    /// <summary>
    /// Информация о текущем пользователе
    /// </summary>
    public UserInfoModel? UserInfo { get; init; }
    /// <summary>
    /// Настройка магазина steam, для текущего пользователя
    /// </summary>
    public StoreUserConfig? StoreUserConfig { get; init; }

    public PurchaseHistoryData() { }
    public PurchaseHistoryData(string error) => Error = error;

    public static PurchaseHistoryData Deserialize(string html)
    {
        HtmlParser parser = new HtmlParser();
        var doc = parser.ParseDocument(html);
        var tbody = doc.GetElementsByTagName("tbody").FirstOrDefault();
        if (tbody == default)
            return new("Не удалось найти элемент tbody") { History = ImmutableList<PurchaseHistoryModel>.Empty };

        var trs = tbody.GetElementsByTagName("tr");
        var list = new List<PurchaseHistoryModel>(trs.Length);
        var cc = CultureInfo.GetCultureInfo("en-US");
        foreach (var tr in trs)
        {
            var obj = ParseHistoryModel(tr, parser, cc);
            if (obj != null)
                list.Add(obj);
        }

        var scripts = doc.Scripts;
        PurchaseHistoryCursorModel? cursor = null;
        foreach (var script in scripts)
        {
            if (script.Source != null || script.Type != "text/javascript")
                continue;
            var splitted = script.Text.Split('\n');
            var g_historyCursor = Array.Find(splitted, x => x.StartsWith("var g_historyCursor"));
            if (g_historyCursor != default)
            {
                var json = g_historyCursor.GetBetween("var g_historyCursor = ", ";")!;
                cursor = JsonSerializer.Deserialize<PurchaseHistoryCursorModel>(json, Steam.JsonOptions)!;
            }
        }

        var application_config = doc.GetElementById("application_config");
        StoreUserConfig? userCfg = null;
        UserInfoModel? userInfo = null;
        if (application_config != default)
        {
            var store_user_config = application_config.GetAttribute("data-store_user_config");
            if (store_user_config != null)
                userCfg = JsonSerializer.Deserialize<StoreUserConfig>(HttpUtility.HtmlDecode(store_user_config), Steam.JsonOptions)!;

            var userinfo = application_config.GetAttribute("data-userinfo");
            if (userinfo != null)
                userInfo = JsonSerializer.Deserialize<UserInfoModel>(HttpUtility.HtmlDecode(userinfo), Steam.JsonOptions)!;
        }

        return new()
        {
            Success = true,
            History = list.ToImmutableList(),
            Cursor = cursor,
            StoreUserConfig = userCfg,
            UserInfo = userInfo,
        };
    }
    /// <summary>
    /// Загружает больше истории продаж.
    /// <para/>
    /// Загружается только, если <see cref="Cursor"/>!=null, иначе будет ошибка.
    /// </summary>
    /// <returns>Новый класс со старой и новой историей</returns>
    public PurchaseHistoryData LoadMoreHistory(DefaultRequest defaultRequest)
    {
        if (Cursor == null)
            return new("Нет курсора") { History = ImmutableList<PurchaseHistoryModel>.Empty };
        var moreHistory = Ajax.account_loadmorehistory(defaultRequest, Cursor);
        if (moreHistory == null)
            return new("Запрос не выполнен") { History = ImmutableList<PurchaseHistoryModel>.Empty };
        if (moreHistory.Html.IsEmpty())
            return new("Нет данных html") { History = ImmutableList<PurchaseHistoryModel>.Empty };

        HtmlParser parser = new HtmlParser();
        var doc = parser.ParseDocument($"<html lang=\"en\"><body><table><tbody>{moreHistory.Html}</tbody></table></body></html>");

        var tbody = doc.GetElementsByTagName("tbody").First();
        var trs = tbody.GetElementsByTagName("tr");
        var list = new List<PurchaseHistoryModel>(History) { Capacity = trs.Length + History.Count + 1 };
        var cc = CultureInfo.GetCultureInfo("en-US");
        foreach (var tr in trs)
        {
            var obj = ParseHistoryModel(tr, parser, cc);
            if (obj != null)
                list.Add(obj);
        }

        return new()
        {
            Success = true,
            History = list.ToImmutableList(),
            Cursor = moreHistory.Cursor,
            StoreUserConfig = StoreUserConfig,
            UserInfo = UserInfo,
        };
    }
    /// <summary>
    /// Загружает больше истории продаж.
    /// <para/>
    /// Загружается только, если <see cref="Cursor"/>!=null, иначе будет ошибка.
    /// </summary>
    /// <returns>Новый класс со старой и новой историей</returns>
    public async Task<PurchaseHistoryData> LoadMoreHistoryAsync(DefaultRequest defaultRequest)
    {
        if (Cursor == null)
            return new("Нет курсора") { History = ImmutableList<PurchaseHistoryModel>.Empty };
        var moreHistory = await Ajax.account_loadmorehistory_async(defaultRequest, Cursor);
        if (moreHistory == null)
            return new("Запрос не выполнен") { History = ImmutableList<PurchaseHistoryModel>.Empty };
        if (moreHistory.Html.IsEmpty())
            return new("Нет данных html") { History = ImmutableList<PurchaseHistoryModel>.Empty };

        HtmlParser parser = new HtmlParser();
        var doc = await parser.ParseDocumentAsync($"<html lang=\"en\"><body><table><tbody>{moreHistory.Html}</tbody></table></body></html>");

        var tbody = doc.GetElementsByTagName("tbody").First();
        var trs = tbody.GetElementsByTagName("tr");
        var list = new List<PurchaseHistoryModel>(History) { Capacity = trs.Length + History.Count + 1 };
        var cc = CultureInfo.GetCultureInfo("en-US");
        foreach (var tr in trs)
        {
            var obj = ParseHistoryModel(tr, parser, cc);
            if (obj != null)
                list.Add(obj);
        }

        return new()
        {
            Success = true,
            History = list.ToImmutableList(),
            Cursor = moreHistory.Cursor,
            StoreUserConfig = StoreUserConfig,
            UserInfo = UserInfo,
        };
    }
    private static PurchaseHistoryModel? ParseHistoryModel(IElement tr, HtmlParser parser, CultureInfo cc)
    {
        const string date_format_1 = "d MMM, yyyy";
        const string date_format_2 = "MMM d, yyyy";
        const string more_history = "more_history";
        const string wht_date_str = "wht_date";
        const string wht_type_str = "wht_type";
        const string wht_items_str = "wht_items";
        const string wth_item_refunded_str = "wth_item_refunded";
        const string wth_payment_str = "wth_payment";
        const string miniprofile_str = "data-miniprofile";
        const string div_str = "div";
        const string wht_total_str = "wht_total";
        const string wallet_column_str = "wallet_column";
        const string onclick_str = "onclick";
        const string transid_str = "transid";
        const string appid_str = "appid";
        const string tooltip_str = "data-tooltip-html";
        const string td_str = "td";
        const string tooltip_first_str = "<html lang=\"en\"><body>";
        const string tooltip_last_str = "</body></html>";

        // в конце таблицы обычно находится скрытый tr для новых данных
        if (tr.Id == more_history)
            return null;

        var wht_date = tr.GetElementsByClassName(wht_date_str).First();
        if (!DateOnly.TryParseExact(wht_date.TextContent.GetClearWebString()!, new string[] { date_format_1, date_format_2 }, cc, DateTimeStyles.None, out var objDate))
            return null;

        uint? appId = null;
        ulong? transId = null;
        var href_data = tr.GetAttribute(onclick_str)!;
        Uri? uri = null;

        // не во всех покупках есть href атрибут
        if (href_data != null)
        {
            uri = new Uri(href_data!.GetBetween("location.href='", "'")!);
            var queries = HttpUtility.ParseQueryString(uri.Query);
            var transids = queries.GetValues(transid_str);
            // так же не во всех есть transid
            // например в маркет транзакциях нет
            if (transids != null)
                transId = transids[0].ParseUInt64();
            // а appid есть только во внутриигровых покупках
            var appids = queries.GetValues(appid_str);
            if (appids != null)
                appId = appids[0].ParseUInt32();
        }

        PurchasePaymentGameModel[] objItems;
        bool objHasRefund = false;
        string? objItemDescription = null;
        PURCHASE_TYPE objType = PURCHASE_TYPE.Unknown;
        var wht_items = tr.GetElementsByClassName(wht_items_str).First();
        // проверяем маркет транзакция или нет
        if (uri != null && uri.Host == "steamcommunity.com")
            objItems = new PurchasePaymentGameModel[] { new(wht_items.TextContent.GetClearWebString()!) };
        else
        {
            /// здесь у нас сохраняется название игры и
            /// добавляется она только в новой игре, либо после цикла
            /// сделано потому что теги с информацией о подарке находятся
            /// в следующих элементах, а изменять свойста мы не можем
            string? lastGameName = null;
            var games = new List<PurchasePaymentGameModel>(wht_items.Children.Length + 1);
            foreach (var children in wht_items.Children)
            {
                if (children.ClassList.Contains(wth_item_refunded_str))
                {
                    // это один из способ определить отменена ли эта покупка
                    // либо эта покупка подтверждение отмены
                    objHasRefund = true;
                    continue;
                }

                if (children.ClassList.Contains(wth_payment_str))
                {
                    var aTag = children.GetElementsByTagName("a").FirstOrDefault();
                    if (aTag != default)
                    {
                        var accountId = aTag.GetAttribute(miniprofile_str).ParseUInt32();
                        games.Add(new(lastGameName!) { AccountId = accountId, AccountName = aTag.TextContent.GetClearWebString()! });
                        lastGameName = null;
                    }
                    else
                    {
                        // если нет тега ссылки, тогда это доп описание купленого предмета внутри игры
                        objItemDescription = children.TextContent.GetClearWebString()!;
                        if (objItemDescription[0].IsWhiteSpaceCharacter())
                            objItemDescription = objItemDescription.Remove(0, 1);
                        // обычно перед предметов всегда есть whitespace символ для сдвига написания правее
                    }
                }
                else
                {
                    if (lastGameName == null)
                        lastGameName = children.TextContent.GetClearWebString()!;
                    else
                    {
                        games.Add(new(lastGameName));
                        lastGameName = children.TextContent.GetClearWebString()!;
                    }
                }
            }
            if (lastGameName != null)
                games.Add(new(lastGameName));
            objItems = games.ToArray();
        }

        PurchasePaymentMethodModel[]? objPaymentMethods = null;
        var wht_type = tr.GetElementsByClassName(wht_type_str).First();
        foreach (var wht_type_div in wht_type.Children)
        {
            // проверяем это тип покупки или информация о способах оплаты
            if (wht_type_div.ClassList.Contains(wth_payment_str))
            {
                // если есть div'ы, тогда много методов было использовано, значит перебираем через foreach
                var wth_payment_divs = wht_type_div.GetElementsByTagName(div_str);
                if (wth_payment_divs.Length > 0)
                {
                    var payments = new List<PurchasePaymentMethodModel>(wth_payment_divs.Length + 1);
                    foreach (var wth_payment_div in wth_payment_divs)
                        payments.Add(PurchasePaymentMethodModel.Parse(wth_payment_div.TextContent));
                    objPaymentMethods = payments.ToArray();
                }
                else // либо метод был один
                    objPaymentMethods = new PurchasePaymentMethodModel[] { PurchasePaymentMethodModel.Parse(wht_type_div.TextContent) };
            }
            else
                objType = wht_type_div.TextContent.GetClearWebString()!.ToEnumPurchaseType();
        }

        bool objIsCredit = false;
        string? objTotal = null;
        var wht_total = tr.GetElementsByClassName(wht_total_str).First();
        var wht_total_divs = wht_total.GetElementsByTagName(div_str);
        if (wht_total_divs.Length > 1)
        {
            foreach (var wht_total_div in wht_total_divs)
            {
                if (wht_total_div.ClassList.Contains(wth_payment_str))
                    objIsCredit = true;
                else
                    objTotal = wht_total_div.TextContent.GetClearWebString()!;
            }
        }
        else
            objTotal = wht_total.TextContent.GetClearWebString()!;

        string? objPreviousBalance = null;
        string? objChange = null;
        string? objNewBalance = null;
        var wallet_column = tr.GetElementsByClassName(wallet_column_str).FirstOrDefault();
        // в некоторых покупках не было обнаружено этого блока
        // причина не понятна, также как и закономерность
        // поэтому проверка обязательна
        if (wallet_column != null)
        {
            var data_tooltip_html = wallet_column.GetAttribute(tooltip_str)!;
            var tooltip = parser.ParseDocument(tooltip_first_str + data_tooltip_html + tooltip_last_str);
            var td = tooltip.GetElementsByTagName(td_str);
            // проверяем на количество на всякий случай
            if (td.Length == 6)
            {
                objPreviousBalance = td[1].TextContent.GetClearWebString()!;
                objChange = td[3].TextContent.GetClearWebString()!;
                objNewBalance = td[5].TextContent.GetClearWebString()!;
            }
        }

        var obj = new PurchaseHistoryModel
        {
            AppId = appId,
            TransactionId = transId,
            Date = objDate,
            Change = objChange,
            HasRefund = objHasRefund,
            IsCredit = objIsCredit,
            ItemDescription = objItemDescription,
            Items = objItems,
            NewBalance = objNewBalance,
            PaymentMethods = objPaymentMethods ?? Array.Empty<PurchasePaymentMethodModel>(),
            PreviousBalance = objPreviousBalance,
            Total = objTotal,
            Type = objType,
        };
        return obj;
    }
}