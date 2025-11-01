using System.Text.Json.Serialization;
using SteamWeb.Script.DTO;

namespace SteamWeb.Models;

public class GridIntoGoo : DataResponse
{
	[JsonPropertyName("goo_value_received ")]
	public uint GooValueReceived { get; init; }

	[JsonPropertyName("goo_value_total")]
	public uint GooValueTotal { get; init; }

	[JsonPropertyName("strHTML")]
	public string? StrHTML { get; init; }
}