using ProtoBuf;

namespace SteamWeb.Auth.v2.Models;

[ProtoContract]
class CPhone_SendPhoneVerificationCode_Request
{
    [ProtoMember(1)] public int language { get; set; } = 0;
}
