using ProtoBuf;
using SteamWeb.Auth.v2.Enums;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace SteamWeb.Auth.v2.DTO;

[ProtoContract]
public class UpdateTokenRequest
{
    [ProtoMember(1)]
    [JsonPropertyName("refresh_token")]
    public required string RefreshToken { get; init; }

    [ProtoMember(2, DataFormat = DataFormat.FixedSize)]
    [JsonPropertyName("steamid")]
    public ulong SteamId { get; init; }

    [ProtoMember(3)]
    [DefaultValue(ETokenRenewalType.k_ETokenRenewalType_Allow)]
    [JsonPropertyName("renewal_type")]
    public ETokenRenewalType RenewalType { get; init; } = ETokenRenewalType.k_ETokenRenewalType_Allow;
}