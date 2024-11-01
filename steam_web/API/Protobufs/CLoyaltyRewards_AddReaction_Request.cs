using ProtoBuf;
using SteamWeb.API.Enums;

namespace SteamWeb.API.Protobufs;
[ProtoContract]
public class CLoyaltyRewards_AddReaction_Request
{
	[ProtoMember(1)]
	public ELoyaltyReactionTargetType target_type { get; init; }
	[ProtoMember(2)]
	public ulong targetid { get; init; }
	[ProtoMember(3)]
	public int reactionid { get; init; }
}