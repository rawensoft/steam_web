using System.Text.Json.Serialization;
using ProtoBuf;

namespace SteamWeb.Script.DTO;

[ProtoContract]
public class CStoreSalesService_SetVote_Request
{
    [ProtoMember(1)]
    [JsonPropertyName("voteid")]
    public uint VoteId { get; init; }

    [ProtoMember(2)]
    [JsonPropertyName("appid")]
    public uint AppId { get; init; }

    [ProtoMember(3)]
    [JsonPropertyName("sale_appid")]
    public uint SaleAppId { get; init; }
}