using System.Text.Json.Serialization;
using ProtoBuf;
using SteamWeb.API.Enums;

namespace SteamWeb.API.Protobufs;

[ProtoContract]
public class CStore_GetDiscoveryQueue_Request
{
    [ProtoMember(1, IsRequired = false)]
    [JsonPropertyName("queue_type")]
    public EStoreDiscoveryQueueType QueueType { get; init; } = EStoreDiscoveryQueueType.k_EStoreDiscoveryQueueTypeNew;

    [ProtoMember(2, IsRequired = false)]
    [JsonPropertyName("country_code")]
    public string CountryCode { get; init; } = string.Empty;

    [ProtoMember(3, IsRequired = false)]
    [JsonPropertyName("rebuild_queue")]
    public bool RebuildQueue { get; init; } = false;

    [ProtoMember(4, IsRequired = false)]
    [JsonPropertyName("settings_changed")]
    public bool SettingsChanged { get; init; } = false;

    [ProtoMember(5, IsRequired = false)]
    [JsonPropertyName("settings")]
    public CStoreDiscoveryQueueSettings? Settings { get; init; }

    [ProtoMember(6, IsRequired = false)]
    [JsonPropertyName("rebuild_queue_if_stale")]
    public bool RebuildQueueIfStale { get; init; } = false;

    [ProtoMember(8, IsRequired = false)]
    [JsonPropertyName("ignore_user_preferences")]
    public bool IgnoreUserPreferences { get; init; } = false;

    [ProtoMember(9, IsRequired = false)]
    [JsonPropertyName("no_experimental_results")]
    public bool NoExperimentalResults { get; init; } = false;

    [ProtoMember(10, IsRequired = false)]
    [JsonPropertyName("experimental_cohort")]
    public uint? ExperimentalCohort { get; init; }

    [ProtoMember(11, IsRequired = false)]
    [JsonPropertyName("debug_get_solr_query")]
    public bool DebugGetSolrQuery { get; init; } = false;
}
