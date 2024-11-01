using System.Text.Json.Serialization;
using ProtoBuf;

namespace SteamWeb.API.Protobufs;
[ProtoContract]
public class CLoyaltyRewards_GetSummary_Response
{
    [ProtoMember(1)]
    [JsonPropertyName("summary")]
    public CLoyaltyRewards_GetSummary_Response_Summary Summary { get; init; } = new();

    [ProtoMember(2)]
    [JsonPropertyName("timestamp_updated")]
    public int TimestampUpdated { get; init; }

    [ProtoMember(3)]
    [JsonPropertyName("auditid_highwater")]
    public ulong AuditIdHighWater { get; init; }
}