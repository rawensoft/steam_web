using System.Text.Json.Serialization;

namespace SteamWeb.Script.Models;

public class RequestKeyResponse
{
    /// <summary>
    /// 22 - ожидание подтверждения
    /// <para/>
    /// 15 - запрос уже был выполнен\истёк
    /// </summary>
    [JsonPropertyName("success")] public EResult Success { get; init; }
    [JsonPropertyName("api_key")] public string? ApiKey { get; init; }
    /// <summary>
    /// Null если передаётся request_id
    /// </summary>
    [JsonPropertyName("request_id")] public ulong? RequestId { get; init; }
    /// <summary>
    /// 1 - подтверждение через мобильное приложение
    /// </summary>
    [JsonPropertyName("requires_confirmation")] public int RequiresConfirmation { get; init; }
}