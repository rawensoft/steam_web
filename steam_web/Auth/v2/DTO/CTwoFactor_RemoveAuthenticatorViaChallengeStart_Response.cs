using ProtoBuf;

namespace SteamWeb.Auth.v2.DTO;

[ProtoContract]
public class CTwoFactor_RemoveAuthenticatorViaChallengeStart_Response
{
    [ProtoMember(1)]
    public bool Success { get; set; } = false;
}
