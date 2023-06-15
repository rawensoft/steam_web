using ProtoBuf;

namespace SteamWeb.Auth.v2.Models;

[ProtoContract]
class AuthGuardRequest
{
    [ProtoMember(1)] public ulong client_id { get; set; }
    [ProtoMember(2, DataFormat = DataFormat.FixedSize)] public ulong steamid { get; set; }
    [ProtoMember(3)] public string code { get; set; }
    [ProtoMember(4)] public EAuthSessionGuardType code_type { get; set; } = EAuthSessionGuardType.DeviceCode;
}
