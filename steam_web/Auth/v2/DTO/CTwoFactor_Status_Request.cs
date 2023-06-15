using ProtoBuf;

namespace SteamWeb.Auth.v2.Models;

[ProtoContract]
class CTwoFactor_Status_Request
{
    [ProtoMember(1, DataFormat = DataFormat.FixedSize)] public ulong steamid { get; set; }
}
