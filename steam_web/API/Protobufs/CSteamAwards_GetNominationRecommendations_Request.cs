using System.Text.Json.Serialization;
using ProtoBuf;

namespace SteamWeb.API.Protobufs;

[ProtoContract]
public class CSteamAwards_GetNominationRecommendations_Request
{
    [ProtoMember(1)]
    [JsonPropertyName("category_id")]
    public uint CategoryId { get; init; }
}
