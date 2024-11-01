using System.Text.Json.Serialization;
using ProtoBuf;

namespace SteamWeb.API.Protobufs;
[ProtoContract]
public class CLoyaltyRewards_GetSummary_Response_Summary
{
    /// <summary>
    /// Текущее кол поинтов
    /// </summary>
    [ProtoMember(1)]
    [JsonPropertyName("points")]
    public long Points { get; init; }

    /// <summary>
    /// Поинтов получено
    /// </summary>
    [ProtoMember(2)]
    [JsonPropertyName("points_earned")]
    public long PointsEarned { get; init; }

    /// <summary>
    /// Поинтов потрачено
    /// </summary>
    [ProtoMember(3)]
    [JsonPropertyName("points_spent")]
    public long PointsSpent { get; init; }
}