using ProtoBuf;

namespace SteamWeb.Auth.v2.Models;

[ProtoContract]
class AuthSessionConfirmation
{
    [ProtoMember(1)] public EAuthSessionGuardType message { get; set; }
    [ProtoMember(2)] public string associated_message { get; set; }
}
