using ProtoBuf;
using SteamWeb.Auth.v2.Enums;
using System.Text.Json.Serialization;

namespace SteamWeb.API.Protobufs;

[ProtoContract]
public class CAuthentication_RefreshToken_Enumerate_Response_RefreshTokenDescription
{
    [ProtoMember(1, DataFormat = DataFormat.FixedSize)]
    [JsonPropertyName("token_id")]
    public ulong TokenId { get; init; }

    [ProtoMember(2)]
    [JsonPropertyName("token_description")]
    public string TokenDescription { get; init; } = string.Empty;

    [ProtoMember(3)]
    [JsonPropertyName("time_updated")]
    public uint TimeUpdated { get; init; }

    [ProtoMember(4)]
    [JsonPropertyName("platform_type")]
    public EAuthTokenPlatformType PlatformType { get; init; }

    [ProtoMember(5)]
    [JsonPropertyName("logged_in")]
    public bool LoggedIn { get; init; }

    [ProtoMember(6)]
    [JsonPropertyName("os_platform")]
    public uint OSPlatform { get; init; }

    [ProtoMember(7)]
    [JsonPropertyName("auth_type")]
    public uint AuthType { get; init; }

    [ProtoMember(8)]
    [JsonPropertyName("gaming_device_type")]
    public uint GamingDeviceType { get; init; }

    [ProtoMember(9, IsRequired = true)]
    [JsonPropertyName("first_seen")]
    public CAuthentication_RefreshToken_Enumerate_Response_TokenUsageEvent FirstSeen { get; init; } = new();

    [ProtoMember(10, IsRequired = true)]
    [JsonPropertyName("last_seen")]
    public CAuthentication_RefreshToken_Enumerate_Response_TokenUsageEvent LastSeen { get; init; } = new();

    [ProtoMember(11)]
    [JsonPropertyName("os_type")]
    public int OSType { get; init; }

    [ProtoMember(12)]
    [JsonPropertyName("authentication_type")]
    public EAuthenticationType AuthenticationType { get; init; }

    [ProtoMember(13)]
    [JsonPropertyName("effective_token_state")]
    public EAuthTokenState EffectiveTokenState { get; init; }
}