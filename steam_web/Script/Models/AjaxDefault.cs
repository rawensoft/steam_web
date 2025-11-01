using SteamWeb.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace SteamWeb.Script.Models;
public class AjaxDefault
{
    /// <summary>
    /// This account recovery session has expired. Please select 'Find Account' and start again.
    /// </summary>
    public const string Error_SessionExpired = "This account recovery session has expired. Please select 'Find Account' and start again.";

    [JsonPropertyName("success")]
    [MemberNotNullWhen(true, nameof(Hash))]
    [MemberNotNullWhen(false, nameof(ErrorMsg))]
    public bool Success { get; init; } = false;

    [JsonPropertyName("hash")]
    public string? Hash { get; init; } = null;

    [JsonPropertyName("errorMsg")]
    public string? ErrorMsg { get; init; } = null;

    [JsonIgnore]
    [MemberNotNullWhen(false, nameof(Hash))]
    [MemberNotNullWhen(true, nameof(ErrorMsg))]
    public bool IsErrorMsg => !ErrorMsg.IsEmpty();

    [JsonIgnore]
    [MemberNotNullWhen(true, nameof(Hash))]
    [MemberNotNullWhen(false, nameof(ErrorMsg))]
    public bool IsHash => !Hash.IsEmpty();
}
