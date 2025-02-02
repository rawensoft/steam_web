using System.Text.Json.Serialization;
using ProtoBuf;
using SteamWeb.API.Enums;

namespace SteamWeb.API.Protobufs;

[ProtoContract]
public class CStore_SkipDiscoveryQueueItem_Request
{
    [ProtoMember(1)]
    [JsonPropertyName("queue_type")]
    public EStoreDiscoveryQueueType QueueType { get; init; } = EStoreDiscoveryQueueType.k_EStoreDiscoveryQueueTypeNew;

    [ProtoMember(2)]
    [JsonPropertyName("appid")]
    public uint AppId { get; init; }
}
