using ProtoBuf;
using SteamWeb.Auth.v2.Enums;

namespace SteamWeb.Auth.v2.DTO;

[ProtoContract]
class UpdateTokenRequest
{
    [ProtoMember(1)] public string refresh_token { get; set; }
    [ProtoMember(2, DataFormat = DataFormat.FixedSize)] public ulong steamid { get; set; }
    [ProtoMember(3)] public ETokenRenewalType renewal_type { get; set; } = ETokenRenewalType.k_ETokenRenewalType_Allow;
}
