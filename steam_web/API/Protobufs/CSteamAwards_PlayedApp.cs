using System.Text.Json.Serialization;
using ProtoBuf;

namespace SteamWeb.API.Protobufs;

[ProtoContract]
public class CSteamAwards_PlayedApp
{
    [ProtoMember(1)]
    [JsonPropertyName("appid")]
    public uint AppId { get; init; }

    [ProtoMember(2)]
    [JsonPropertyName("playtime")]
    public uint Playtime { get; init; }
}
