using System.Text.Json.Serialization;
using SteamWeb.Extensions;

namespace SteamWeb.API.Models.ICSGOPlayers_730;
public class MatchSharingCode
{
    public const string NoneNextCode = "n/a";

    [JsonPropertyName("nextcode")] public string? NextCode { get; init; }
    [JsonIgnore] public bool IsNext => !NextCode.IsEmpty() && NoneNextCode != NextCode;
}