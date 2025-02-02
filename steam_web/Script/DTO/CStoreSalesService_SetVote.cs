using System.Text.Json.Serialization;
using ProtoBuf;

namespace SteamWeb.Script.DTO;

[ProtoContract]
public class CStoreSalesService_SetVote
{
    [ProtoMember(1)]
    [JsonPropertyName("voteid")]
    public uint VoteId { get; init; }

    [ProtoMember(2)]
    [JsonPropertyName("appid")]
    public uint AppId { get; init; }

    [ProtoMember(3)]
    [JsonPropertyName("data")]
    public long Data { get; init; }
}