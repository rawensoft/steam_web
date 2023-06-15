using ProtoBuf;

namespace SteamWeb.Auth.v2.DTO;

[ProtoContract]
class UpdateTokenRequest
{
    [ProtoMember(1)] public string refresh_token { get; set; }
    [ProtoMember(2, DataFormat = DataFormat.FixedSize)] public ulong steamid { get; set; }
}
