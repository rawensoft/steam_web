using System.Text.Json.Serialization;

namespace SteamWeb.Script.Models;
public class AjaxSetTokenResponse
{
	[JsonPropertyName("result")] public EResult Result { get; init; } = EResult.Invalid;
	[JsonPropertyName("rtExpiry")] public int RtExpiry { get; init; }
}