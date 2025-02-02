using System.Text.Json.Serialization;
using ProtoBuf;

namespace SteamWeb.Script.DTO;

[ProtoContract]
public class CStoreSalesService_SetVote_Response
{
    [ProtoMember(1)]
    [JsonPropertyName("votes")]
    public CStoreSalesService_SetVote[] Votes { get; init; } = Array.Empty<CStoreSalesService_SetVote>();
}