using System.Text.Json.Serialization;
using ProtoBuf;

namespace SteamWeb.API.Protobufs;

[ProtoContract]
public class CStore_GetDiscoveryQueue_Response
{
    [ProtoMember(1)]
    [JsonPropertyName("appids")]
    public uint[]? AppIds { get; init; }

    [ProtoMember(2)]
    [JsonPropertyName("country_code")]
    public string CountryCode { get; init; } = string.Empty;

    [ProtoMember(3)]
    [JsonPropertyName("settings")]
    public CStoreDiscoveryQueueSettings? Settings { get; init; }

    [ProtoMember(4)]
    [JsonPropertyName("skipped")]
    public uint Skipped { get; init; }

    [ProtoMember(5)]
    [JsonPropertyName("exhausted")]
    public bool Exhausted { get; init; } = false;

    [ProtoMember(6)]
    [JsonPropertyName("experimental_cohort")]
    public uint ExperimentalCohort { get; init; } = 0;

    [ProtoMember(7)]
    [JsonPropertyName("debug_solr_query")]
    public string? DebugSolrQuery { get; init; }
}
