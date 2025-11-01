using SteamWeb.Script.DTO;
using System.Text.Json.Serialization;

namespace SteamWeb.Script.Models;

public class GooValueData : DataResponse
{
	[JsonPropertyName("goo_value")]
	public uint GooValue { get; init; }
}