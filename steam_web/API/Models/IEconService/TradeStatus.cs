using System.Text.Json.Serialization;
using SteamWeb.Extensions;

namespace SteamWeb.API.Models.IEconService;
public class TradeStatus
{
	[JsonPropertyName("trades")] public TradeHistory[] Trades { get; init; } = Array.Empty<TradeHistory>();
	/// <summary>
	/// Доступно если get_descriptions == true
	/// </summary>
	[JsonPropertyName("descriptions")] public List<TradeHistoryDescription> Descriptions { get; init; } = new(1);

	/// <summary>
	/// Доступно если get_descriptions == true
	/// </summary>
	/// <param name="classid"></param>
	/// <param name="instanceid"></param>
	/// <returns>Null если описание не найдено</returns>
	public TradeHistoryDescription? GetDescriptionForItem(string classid, string instanceid)
	{
		var cid = classid.ParseUInt64();
		var iid = instanceid.ParseUInt64();
		var item = Descriptions
			.Where(x => x.ClassId == cid)
			.FirstOrDefault(x => x.InstanceId == iid);
		return item;
	}
	/// <summary>
	/// Доступно если get_descriptions == true
	/// </summary>
	/// <param name="classid"></param>
	/// <param name="instanceid"></param>
	/// <returns>Null если описание не найдено</returns>
	public TradeHistoryDescription? GetDescriptionForItem(ulong classid, ulong instanceid)
	{
		var item = Descriptions
			.Where(x => x.ClassId == classid)
			.FirstOrDefault(x => x.InstanceId == instanceid);
		return item;
	}
}