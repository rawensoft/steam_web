using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using SteamWeb.Extensions;

namespace SteamWeb.Models;
public sealed class AboutProfile
{
    private const string SG_Good = "sg_good.png";
    private const string SG_Fair = "sg_fair.png";
    private const string SG_Poor = "sg_poor.png";

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
    /// Логин аккаунта
    /// </summary>
    public string? Login { get; internal set; } = null;
    /// <summary>
    /// Страна (регион) пользователя
    /// </summary>
    public string? Country { get; internal set; } = null;
    /// <summary>
    /// SteamID64 аккаунта
    /// </summary>
    public ulong SteamID { get; internal set; } = 0;
    /// <summary>
    /// Последние цифры телефона
    /// </summary>
    public string? LastPhoneDigit { get; internal set; } = null;

    internal static AboutProfile Deserialize(string html)
    {
        var about = new AboutProfile();
        if (html == "<!DOCTYPE html>")
        {
            return about;
        }

        var parser = new HtmlParser();
        var doc = parser.ParseDocument(html);
        var global_action_links = doc.GetElementsByClassName("global_action_link");
        foreach (var global_action_link in global_action_links)
        {
            var uri = global_action_link.GetAttribute("href");
            if (uri != null && uri.StartsWith("https://store.steampowered.com/login/?redir="))
            {
                // Deauth
                return about;
            }    
        }

        var settings_sub_blocks = doc.GetElementsByClassName("account_setting_sub_block");
        foreach (var settings_sub_block in settings_sub_blocks)
        {
            var account_data = settings_sub_block.GetElementsByClassName("accountData");
            if (account_data.Any())
            {
                var account = account_data.First();
                about.Balance = account.TextContent.GetClearWebString();
                continue;
            }

            var country_settings = settings_sub_block.GetElementsByClassName("country_settings");
            if (country_settings.Any())
            {
                var country = country_settings.First().GetElementsByClassName("account_data_field").First();
                about.Country = country.TextContent.GetClearWebString();
                continue;
            }

            var phone_header_description = settings_sub_block.GetElementsByClassName("phone_header_description");
            if (phone_header_description.Any())
            {
                var phone = phone_header_description.First().GetElementsByClassName("account_data_field").First();
                about.LastPhoneDigit = phone.TextContent.GetClearWebString();
                continue;
            }

            var securityBlock = settings_sub_block.GetElementsByClassName("account_security_block");
            if (securityBlock.Any())
            {
                var images = securityBlock.First().GetElementsByTagName("img");
                if (images.Any())
                {
                    var img = images.First() as IHtmlImageElement;
                    switch (img!.Source!.Split('/')[^1])
                    {
                        case SG_Good:
                            about.FA2 = Enums.FA2.SteamMobileApp;
                            break;

                        case SG_Fair:
                            about.FA2 = about.LastPhoneDigit != null ? Enums.FA2.EmailCodeWithPhone : Enums.FA2.EmailCode;
                            break;

                        case SG_Poor:
                            about.FA2 = about.LastPhoneDigit != null ? Enums.FA2.NonGuardWithPhone : Enums.FA2.NonGuard;
                            break;
                    }
                    continue;
                }
            }

            // есть проблема что под это могут подходить все блоки
            var contacts = settings_sub_block.GetElementsByClassName("account_data_field");
            if (contacts.Any())
            {
                var country = contacts.First();
                about.Email = country.TextContent.GetClearWebString();
                continue;
            }
        }

        var youraccount_steamid = doc.GetElementsByClassName("youraccount_steamid");
        if (youraccount_steamid.Any())
        {
            var steamid64 = youraccount_steamid.First();
            about.SteamID = steamid64.TextContent.GetClearWebString()?.Split(": ")[1].ParseUInt64() ?? 0;
        }

        var account_name = doc.GetElementsByClassName("account_name");
        if (account_name.Any())
        {
            var login = account_name.First();
            about.Login = login.TextContent.GetClearWebString();
        }

        var persona_name_text_content = doc.GetElementsByClassName("persona_name_text_content");
        if (persona_name_text_content.Any())
        {
            var username = persona_name_text_content.First();
            about.Username = username.TextContent.GetClearWebString();
        }

        return about;
    }
}