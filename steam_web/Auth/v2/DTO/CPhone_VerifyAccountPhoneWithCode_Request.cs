using ProtoBuf;

namespace SteamWeb.Auth.v2.Models;

[ProtoContract]
class CPhone_VerifyAccountPhoneWithCode_Request
{
    [ProtoMember(1)] public string code { get; set; }
}
