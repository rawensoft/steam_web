using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO;

public class CreateBuyOrder
{
    /// <summary>
    /// 16 - проблемы с серверами
    /// </summary>
    [JsonPropertyName("success")]
    public EResult Success { get; init; } = EResult.Invalid;

    [JsonPropertyName("message")]
    public string? Message { get; init; }
}