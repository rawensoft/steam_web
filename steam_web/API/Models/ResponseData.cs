using System.Text.Json.Serialization;

namespace SteamWeb.API.Models;
public class ResponseData<T>
{
    [JsonPropertyName("success")] public bool Success { get; internal set; } = false;
    /// <summary>
    /// default(T)
    /// </summary>
    [JsonPropertyName("response")] public T? Response { get; init; } = default;
}
public class ResultData<T>
{
    [JsonPropertyName("success")] public bool Success { get; internal set; } = false;
    /// <summary>
    /// default(T)
    /// </summary>
    [JsonPropertyName("result")] public T? Result { get; init; } = default;
}
public class ResponseData
{
    [JsonPropertyName("success")] public bool Success { get; init; } = false;
}