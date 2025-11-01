using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using SteamWeb.Extensions;

namespace SteamWeb.Script.Models;
public class RecoveryChangeEmail
{
    [JsonPropertyName("hash")]
    public string? Hash { get; init; }

    [JsonPropertyName("errorMsg")]
    public string? ErrorMsg { get; init; }

    /// <summary>
    /// true если нужно подтвердить код с почты
    /// </summary>
    [JsonPropertyName("show_confirmation")]
    public bool ShowConfirmation { get; init; } = false;

    [JsonIgnore]
    [MemberNotNullWhen(true, [nameof(Hash)])]
    [MemberNotNullWhen(false, [nameof(ErrorMsg)])]
    public bool IsHash => !Hash.IsEmpty();

    [JsonIgnore]
    [MemberNotNullWhen(false, [nameof(Hash)])]
    [MemberNotNullWhen(true, [nameof(ErrorMsg)])]
    public bool IsError => ErrorMsg.IsEmpty();
}
