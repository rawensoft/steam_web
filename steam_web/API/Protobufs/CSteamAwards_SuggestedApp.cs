using System.Text.Json.Serialization;
using ProtoBuf;

namespace SteamWeb.API.Protobufs;

[ProtoContract]
public class CSteamAwards_SuggestedApp
{
    [ProtoMember(1)]
    [JsonPropertyName("appid")]
    public uint AppId { get; init; }
}