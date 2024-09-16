using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IPlayerService;
public class NicknameModel
{
    [JsonPropertyName("accountid")] public uint AccountId { get; init; }
    [JsonPropertyName("nickname")] public string Nickname { get; init; }
}