using SteamWeb.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace SteamWeb.Script.Models;

public class NextRedirect
{
    [JsonPropertyName("hash")]
    public string? Hash { get; init; }

    [JsonPropertyName("errorMsg")]
    public string? ErrorMsg { get; init; }

    [JsonIgnore]
    [MemberNotNullWhen(true, [nameof(Hash)])]
    [MemberNotNullWhen(false, [nameof(ErrorMsg)])]
    public bool IsHash => !Hash.IsEmpty();

    [JsonIgnore]
    [MemberNotNullWhen(false, [nameof(Hash)])]
    [MemberNotNullWhen(true, [nameof(ErrorMsg)])]
    public bool IsError => ErrorMsg.IsEmpty();
}
