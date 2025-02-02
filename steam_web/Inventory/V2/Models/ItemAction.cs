using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using SteamWeb.Extensions;

namespace SteamWeb.Inventory.V2.Models;
public class ItemAction
{
    private const string _javascript = "javascript:";
	private const string _https = "https://";
    private readonly static Regex _rgx = new(@"^javascript:GetGooValue\( '%contextid%', '%assetid%', (\d{1,11}), (\d{1,11}), (\d{1,11}) \)$", RegexOptions.Compiled | RegexOptions.Singleline, TimeSpan.FromMilliseconds(100));

	[JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("link")]
    public string Link { get; init; }

    public bool IsJavascript() => Link.StartsWith(_javascript);
	public bool IsUrl() => Link.StartsWith(_https);
	/// <summary>
	/// Получает из <see cref="Link"/> appid, amount и border для вызова <see cref="Steam.GrindIntoGoo(SteamWeb.Models.DefaultRequest, uint, ulong, byte, uint)"/>
	/// </summary>
	/// <returns>AppId; Gem Amount; Border. 0 если не получилось узнать</returns>
	public (uint, uint, uint) GetGooValue()
    {
		const uint def_value = 0;
		try
		{
			var match = _rgx.Match(Link);
			if (match.Success)
				return (def_value, def_value, def_value);

			var appid = match.Groups[1].Value.ParseUInt32();
			var amount = match.Groups[2].Value.ParseUInt32();
			var border = match.Groups[3].Value.ParseUInt32();
			return (appid, amount, border);
		}
		catch (RegexMatchTimeoutException)
		{
			return (def_value, def_value, def_value);
		}
    }
}