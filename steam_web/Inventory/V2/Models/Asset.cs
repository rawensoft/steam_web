using System.Text.Json.Serialization;

namespace SteamWeb.Inventory.V2.Models;
public class Asset
{
    [JsonPropertyName("id")] public ulong Id { get; init; }
    [JsonPropertyName("classid")] public string ClassId { get; init; }
    [JsonPropertyName("instanceid")] public string InstanceId { get; init; }
    [JsonPropertyName("amount")] public uint Amount { get; init; }
    /// <summary>
    /// 1 - Не отображяется для китая
    /// </summary>
    [JsonPropertyName("hide_in_china")] public byte HideInChina { get; init; }
    /// <summary>
    /// Позиция в инвентаре
    /// </summary>
    [JsonPropertyName("pos")] public ushort Position { get; init; }
}