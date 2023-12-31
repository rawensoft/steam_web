using ProtoBuf;

namespace SteamWeb.Auth.v2.DTO;

[ProtoContract]
public class CTwoFactor_RemoveAuthenticatorViaChallengeContinue_Response
{
    [ProtoMember(1)]
    public bool Success { get; set; } = false;
    [ProtoMember(2)]
    public CRemoveAuthenticatorViaChallengeContinue_Replacement_Token? replacement_token { get; set; }
}
