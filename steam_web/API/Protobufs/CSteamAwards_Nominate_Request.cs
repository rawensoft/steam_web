using System.Text.Json.Serialization;
using ProtoBuf;

namespace SteamWeb.API.Protobufs;

[ProtoContract]
public class CSteamAwards_Nominate_Request
{
    [ProtoMember(1)]
    [JsonPropertyName("category_id")]
    public uint CategoryId { get; init; }

    [ProtoMember(2, IsRequired = false)]
    [JsonPropertyName("nominated_id")]
    public uint NominatedId { get; init; }

    [ProtoMember(4, IsRequired = false)]
    [JsonPropertyName("store_appid")]
    public uint StoreAppId { get; init; }
}
