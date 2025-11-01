using System.Text.Json.Serialization;

namespace SteamWeb.Auth.v2.Models;

public class ConfirmationsResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; init; } = false;

    [JsonPropertyName("needauth")]
    public bool NeedAuth { get; init; } = false;

    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("detail")]
    public string? Detail { get; init; }

    [JsonPropertyName("creation_time")]
    public Confirmation[] Conf { get; init; } = Array.Empty<Confirmation>();

    /// <summary>
    /// Indexing array <see cref="Conf"/>
    /// </summary>
    public Confirmation this[int index]
	{
		get => Conf[index];
		set => Conf[index] = value;
	}
}