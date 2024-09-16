using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IPlayerService;
public class NicknamesModel
{
    [JsonPropertyName("nicknames")] public NicknameModel[] Nicknames { get; init; } = Array.Empty<NicknameModel>();
}