using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace SteamWeb.Script.Models;
public class AjaxRefreshResponse
{
	[JsonPropertyName("success")]
    [MemberNotNullWhen(true, [nameof(LoginUrl), nameof(Nonce), nameof(Redir), nameof(Auth)])]
    public bool Success { get; init; } = false;

	[JsonPropertyName("error")]
    public EResult Error { get; init; } = EResult.Invalid;

	[JsonPropertyName("login_url")]
    public string? LoginUrl { get; init; }

	[JsonPropertyName("steamID")]
    public ulong SteamId { get; init; }

	[JsonPropertyName("nonce")]
    public string? Nonce { get; init; }

	[JsonPropertyName("redir")]
    public string? Redir { get; init; }

	[JsonPropertyName("auth")]
    public string? Auth { get; init; }
}