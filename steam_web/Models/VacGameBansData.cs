using SteamWeb.Extensions;
using AngleSharp.Html.Parser;
using System.Web;
using System.Collections.Immutable;

namespace SteamWeb.Models;
public class VacGameBansData
{
    public bool Success { get; init; } = false;
    public string? Error { get; init; } = null;
    public required ImmutableList<VacGameBanModel> Apps { get; init; }
    public bool HasAnyBans { get; init; } = false;

    public VacGameBansData() { }
    public VacGameBansData(string error) => Error = error;

    /// <exception cref="InvalidOperationException"></exception>
    internal static VacGameBansData Deserialize(string html)
    {
        const string VACBanText = "Bans applied by VAC or Valve Anti-Cheat";
        const string GameBanText = "Bans applied by the Game Developer";
        const string href_str = "href";
        const string appid_str_1 = "appid";
        const string appid_str_2 = "steamAppId";
        const string highlight_str = "help_highlight_text";
        const string refund_info_box_str = "refund_info_box";
        const string vac_ban_header_str = "vac_ban_header";
        const string battle_eye_str = "Visit BattlEye Support";

        HtmlParser parser = new HtmlParser();
        var doc = parser.ParseDocument(html);
        if (doc.GetElementsByClassName("no_vac_bans_header").Any())
            return new() { Success = true, Apps = ImmutableList<VacGameBanModel>.Empty, HasAnyBans = false };

        var help_issue_details = doc.GetElementsByClassName("help_issue_details");
        var list = new Dictionary<uint, VacGameBanModel>(help_issue_details.Length * 2);
        foreach (var help_issue_detail in help_issue_details)
        {
            var refund_info_boxes = help_issue_detail.GetElementsByClassName(refund_info_box_str);
            var vac_ban_header = help_issue_detail.GetElementsByClassName(vac_ban_header_str).First();
            var title = vac_ban_header.TextContent.GetClearWebString();
            bool isGameBan = title == GameBanText;
            bool isVACBan = title == VACBanText;

			foreach (var refund_info_box in refund_info_boxes)
			{
				var gameName = refund_info_box.GetElementsByClassName(highlight_str).First();
                var appName = gameName?.TextContent.GetClearWebString() ?? "unknown game name";
				uint appId;
				var gameUri = refund_info_box.GetElementsByTagName("a").First();
                if (gameUri.TextContent == battle_eye_str)
                {
                    appId = 4294967295;
                    while (true)
                    {
						if (!list.ContainsKey(appId))
                            break;
                        appId--;
					}
                    list.Add(appId, new() { AppId = appId, Name = appName, BattleEye = true });
				}
                else
                {
					var uri = new Uri(gameUri.GetAttribute(href_str)!);
					var queries = HttpUtility.ParseQueryString(uri.Query);
					appId = queries.Get(appid_str_1).ParseUInt32();
					if (appId == 0)
                        appId = queries.Get(appid_str_2).ParseUInt32();
                    if (appId == 0)
                        throw new InvalidOperationException($"Не обнаружен appid из query '{uri.Query}'");
					if (!list.TryGetValue(appId, out var gameInfo))
					{
						gameInfo = new()
						{
							AppId = appId,
							Name = appName,
						};
						list.Add(appId, gameInfo);
					}
					if (isGameBan)
						gameInfo.GameBan = isGameBan;
					if (isVACBan)
						gameInfo.VACBan = isVACBan;
				}
			}
        }
        return new()
        {
            Success = true,
            Apps = list.Values.ToImmutableList(),
            HasAnyBans = list.Count != 0,
        };
    }
}