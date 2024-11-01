using System.Text.Json.Serialization;
using ProtoBuf;

namespace SteamWeb.API.Protobufs;
[ProtoContract]
public class CLoyaltyRewards_GetReactionConfig_Response
{
	[ProtoMember(3)]
	[JsonPropertyName("reactions")]
	public CLoyaltyRewards_GetReactionConfig_Response_ReactionConfig[]? Reactions { get; init; }
}