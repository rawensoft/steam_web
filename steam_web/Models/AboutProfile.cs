using AngleSharp.Html.Parser;
using SteamWeb.Extensions;

namespace SteamWeb.Models;
public sealed class AboutProfile
{
    private const string SG_Good = "sg_good.png";
    private const string SG_Fair = "sg_fair.png";
    private const string Icon_Mobile = "icon_mobile.png";

    /// <summary>
    /// Аутентификатор аккаунта
    /// </summary>
    public Enums.FA2 FA2 { get; internal set; } = Enums.FA2.Deauth;
    /// <summary>
    /// Почта аккаунта
    /// </summary>
    public string? Email { get; internal set; } = null;
    /// <summary>
    /// Баланс аккаунта
    /// </summary>
    public string? Balance { get; internal set; } = null;
    /// <summary>
    /// Имя пользователя аккаунта
    /// </summary>
    public string? Username { get; internal set; } = null;
    /// <summary>
    /// Страна (регион) пользователя
    /// </summary>
    public string? Country { get; internal set; } = null;
    /// <summary>
    /// Имеется ли красная табличка (КТ) на аккаунте
    /// </summary>
    public bool Red_Table { get; internal set; } = false;
    /// <summary>
    /// SteamID64 аккаунта
    /// </summary>
    public ulong SteamID { get; internal set; } = 0;

    internal static AboutProfile Deserialize(string html)
    {
        var about = new AboutProfile();
        if (html == "<!DOCTYPE html>")
        {
            about.Red_Table = true;
            return about;
        }

        if (html.Contains(SG_Good)) about.FA2 = Enums.FA2.SteamMobileApp;
        else if (html.Contains(SG_Fair) && html.Contains(Icon_Mobile)) about.FA2 = Enums.FA2.EmailCodeWithPhone;
        else if (html.Contains(SG_Fair) && !html.Contains(Icon_Mobile)) about.FA2 = Enums.FA2.EmailCode;
        else if (html.Contains(Icon_Mobile)) about.FA2 = Enums.FA2.NonGuardWithPhone;
        else if (html.Contains("store.steampowered.com/login/?redir=account")) about.FA2 = Enums.FA2.Deauth;
        else about.FA2 = Enums.FA2.NonGuard;

        if (about.FA2 != Enums.FA2.Deauth)
        {
            var parser = new HtmlParser();
            var doc = parser.ParseDocument(html);

            var el = doc.GetElementsByClassName("country_settings");
            if (el.Length > 0)
            {
                el = el[0].GetElementsByClassName("account_data_field");
                if (el.Length > 0)
                {
                    about.Country = el[0].TextContent;
                    about.Country = about.Country.GetClearWebString();
                }
            } // Country
            el = doc.GetElementsByClassName("persona_name_text_content");

            if (el.Length > 0) // Username
                about.Username = el[el.Length - 1].TextContent.GetClearWebString();

            el = doc.GetElementsByClassName("account_data_field");
            if (el.Length >= 2) // Email
                about.Email = el[1].TextContent.GetClearWebString();

            var element = doc.GetElementById("header_wallet_balance");
            if (element != null) // Email
                about.Balance = element.TextContent.GetClearWebString();

            el = doc.GetElementsByClassName("youraccount_steamid");
            if (el.Length > 0)
            {
                about.SteamID = el[0].TextContent.GetClearWebString().GetOnlyDigit().ParseUInt64(); // Через сплит делать только так: Split(new char['：',':']);
            } // Email

            //about.Country = data.GetBetween("\t\t\t\t\t\t\t\t<span class=\"account_data_field\">", "</span>");
            //about.Username = data.GetBetween("ShowMenu( this, 'account_dropdown', 'right', 'bottom', true );\">\r\n\t\t\t\t\t\t", "\t\t\t\t\t</span>");
            //if (about.Username != null && about.Username.Length > 60) about.Username = data.GetBetween("data-tooltip-type=\"selector\" data-tooltip-content=\".submenu_username\">", "</a>", null, 2)?.Replace("\n", "").Replace("\t", "").Replace("\r", "");
            //about.Email = data.GetBetween(":</span> <span class=\"account_data_field\">", "</span></div>");
            //about.Balance = data.GetBetween("Wallet <b>(", ")</b>");
            //if (about.Balance == null) about.Balance = data.GetBetween("https://store.steampowered.com/account/store_transactions/\">", "</a>");
            //if (about.Balance == null) about.Balance = data.GetBetween("<div class=\"accountData price\">", "</div>");
            //about.SteamID = data.GetBetween("Steam ID: ", "</div>");
        }
        if (html.Contains("global_header_toggle_button red global_header_account_alert tooltip"))
            about.Red_Table = true;
        if ((about.FA2 == Enums.FA2.NonGuard || about.FA2 == Enums.FA2.NonGuardWithPhone) &&
            about.SteamID == 0 && about.Balance == null &&
            about.Email == null && about.Username == null)
            about.FA2 = Enums.FA2.Deauth;
        return about;
    }
}
