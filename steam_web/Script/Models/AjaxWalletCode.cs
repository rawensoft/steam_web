using System.Text.Json.Serialization;
namespace SteamWeb.Script.Models;
public class AjaxWalletCode
{
	[JsonPropertyName("success")] public EResult Success { get; init; } = EResult.Invalid;
}