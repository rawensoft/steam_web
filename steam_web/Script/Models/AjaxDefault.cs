using SteamWeb.Extensions;
using System.Text.Json.Serialization;

namespace SteamWeb.Script.Models;
public class AjaxDefault
{
    /// <summary>
    /// This account recovery session has expired. Please select 'Find Account' and start again.
    /// </summary>
    public const string Error_SessionExpired = "This account recovery session has expired. Please select 'Find Account' and start again.";

    [JsonPropertyName("success")]
    public bool Success { get; init; } = false;

    [JsonPropertyName("hash")]
    public string? Hash { get; init; } = null;

    [JsonPropertyName("errorMsg")]
    public string? ErrorMsg { get; init; } = null;

    [JsonIgnore]
    public bool IsErrorMsg => !ErrorMsg.IsEmpty();

    [JsonIgnore]
    public bool IsHash => !Hash.IsEmpty();
}
