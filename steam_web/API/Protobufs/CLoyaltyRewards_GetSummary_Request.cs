using System.Text.Json.Serialization;
using ProtoBuf;

namespace SteamWeb.API.Protobufs;
[ProtoContract]
public class CLoyaltyRewards_GetSummary_Request
{
	[ProtoMember(1, DataFormat = DataFormat.FixedSize)]
	[JsonPropertyName("steamid")]
	public ulong SteamId { get; init; }
}