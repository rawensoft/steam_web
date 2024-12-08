using SteamWeb.Extensions;
using System.Text.RegularExpressions;

namespace SteamWeb.Models;

public class SteamOfferItem
{
    private static Regex _rgxId = new(@"^item(\d{3,9})_(\d{1,3})_(\d{1,})$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    private static Regex _rgxEconomy = new(@"^\w+/(\d{3,9})/(\d{1,11})(?:/(\d{1,11})){0,1}$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    public uint AppId { get; init; }
    public ulong ClassId { get; init; }
	public ulong InstanceId { get; init; }

	/// <summary>
	/// Парсит html атрибут id класса trade_item
	/// </summary>
	/// <param name="id">данные из html атрибута</param>
	/// <returns>true данные спарсены; app id; context id; asset id</returns>
	internal static (bool, uint, uint, ulong) ParseItemId(string? id)
    {
        if (id.IsEmpty())
            return (false, 0, 0, 0);
        try
        {
            var match = _rgxId.Match(id!);
            if (match.Success)
            {
                var appid = match.Groups[1].Value.ParseUInt32();
                var contextid = match.Groups[2].Value.ParseUInt32();
                var assetid = match.Groups[3].Value.ParseUInt64();
                return (true, appid, contextid, assetid);
            }
            return (false, 0, 0, 0);
        }
        catch (RegexMatchTimeoutException)
        {
            return (false, 0, 0, 0);
        }
    }
    /// <summary>
    /// Парсит html атрибут data-economy-item класса trade_item
    /// </summary>
    /// <param name="id">данные из html атрибута</param>
    /// <returns>true данные спарсены; app id; class id; instance id</returns>
    internal static (bool, uint, ulong, ulong) ParseEconomyData(string? id)
    {
        if (id.IsEmpty())
            return (false, 0, 0, 0);
        try
        {
            var match = _rgxEconomy.Match(id!);
            if (match.Success)
            {
                var appid = match.Groups[1].Value.ParseUInt32();
                var classid = match.Groups[2].Value.ParseUInt64();
				var instanceid = match.Groups[3].Value.ParseUInt64();
                if (instanceid == 0)
				    return (true, appid, instanceid, classid);
                return (true, appid, classid, instanceid);
			}
			return (false, 0, 0, 0);
		}
        catch (RegexMatchTimeoutException)
		{
			return (false, 0, 0, 0);
		}
    }
}