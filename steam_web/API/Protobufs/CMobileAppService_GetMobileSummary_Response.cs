using ProtoBuf;
using System.Text.Json.Serialization;

namespace SteamWeb.API.Protobufs;

[ProtoContract]
public class CMobileAppService_GetMobileSummary_Response
{
    [ProtoMember(1)]
    [JsonPropertyName("stale_time_seconds")]
    public uint StaleTimeSeconds { get; init; }

    [ProtoMember(2)]
    [JsonPropertyName("is_authenticator_valid")]
    public bool IsAuthenticatorValid { get; init; } = false;

    [ProtoMember(3)]
    [JsonPropertyName("owned_games")]
    public uint OwnedGames { get; init; }

    [ProtoMember(4)]
    [JsonPropertyName("friend_count")]
    public uint FriendCount { get; init; }

    [ProtoMember(5)]
    [JsonPropertyName("wallet_balance")]
    public string? WalletBalance { get; init; }

    [ProtoMember(6)]
    [JsonPropertyName("language")]
    public string? Language { get; init; }
}
