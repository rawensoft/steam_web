using SteamWeb.Script.DTO;
using System.Text.Json.Serialization;

namespace SteamWeb.Script.Models;

public class GooValue : Data
{
	[JsonPropertyName("goo_value")]
	public uint goo_value { get; init; }
}