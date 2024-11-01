using System.Text.Json.Serialization;
using ProtoBuf;
using SteamWeb.API.Enums;

namespace SteamWeb.API.Protobufs;
[ProtoContract]
public class CLoyaltyRewards_GetReactionConfig_Response_ReactionConfig
{
	/// <summary>
	/// Id реакции
	/// </summary>
	[ProtoMember(1)]
	[JsonPropertyName("reactionid")]
	public int ReactionId { get; init; }

	/// <summary>
	/// Стоимость, сколько нужно заплатить
	/// </summary>
	[ProtoMember(2)]
	[JsonPropertyName("points_cost")]
	public uint PointsCost { get; init; }

	/// <summary>
	/// Передадутся поинтов, кому ставят реакцию
	/// </summary>
	[ProtoMember(3)]
	[JsonPropertyName("points_transferred")]
	public uint PointsTransferred { get; init; }

	/// <summary>
	/// Где применима эта реакция
	/// </summary>
	[ProtoMember(4)]
	[JsonPropertyName("valid_target_types")]
	public ELoyaltyReactionTargetType[] ValidTargetTypes { get; init; } = Array.Empty<ELoyaltyReactionTargetType>();

	/// <summary>
	/// При каких случаях можно ставить реакцию если <see cref="ValidTargetTypes"/>==<see cref="ELoyaltyReactionTargetType.UserGeneratedContent"/>
	/// </summary>
	[ProtoMember(5)]
	[JsonPropertyName("valid_ugc_types")]
	public uint[] ValidUgcTypes { get; init; } = Array.Empty<uint>();
}