using AngleSharp.Html.Parser;
using SteamWeb.Extensions;

namespace SteamWeb.Models;

public class SteamRefundMethods
{
    public bool Success { get; init; } = false;

    /// <summary>
    /// In your country, the following payment methods support refunds
    /// </summary>
    public List<string> SupportRefunds { get; init; } = new(9);

    /// <summary>
    ///In your country, the following payment methods can not be refunded
    /// </summary>
    public List<string> CannotRefunded { get; init; } = new(9);

    internal static SteamRefundMethods Deserialize(string html)
    {
        var parser = new HtmlParser();
        var doc = parser.ParseDocument(html);
        var main_content = doc.GetElementById("main_content");
        if (main_content == null)
            return new();

        byte flag = 0;
        var children = main_content.Children;
        var steamRefundMethods = new SteamRefundMethods() { Success = true };
        foreach (var child in children)
        {
            if (flag == 0)
            {
                if (child.TextContent.EndsWith("refunds:"))
                    flag = 1;
            }
            else
            {
                if (flag == 1 && child.TextContent.EndsWith("refunded:"))
                    flag = 2;
                else if (child.TagName == "UL")
                {
                    Action<string> addMethod = flag == 1 ? steamRefundMethods.SupportRefunds.Add : steamRefundMethods.CannotRefunded.Add;
                    foreach (var item in child.Children)
                    {
                        var method = item.TextContent.GetClearWebString()!;
                        addMethod(method);
                    }
                }
            }
        }
        return steamRefundMethods;
    }
}