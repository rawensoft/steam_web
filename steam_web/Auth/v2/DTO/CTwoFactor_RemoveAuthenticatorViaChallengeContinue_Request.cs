using ProtoBuf;

namespace SteamWeb.Auth.v2.DTO;

[ProtoContract]
public class CTwoFactor_RemoveAuthenticatorViaChallengeContinue_Request
{
    /// <summary>
    /// Code from SMS
    /// </summary>
    [ProtoMember(1)]
    public string SmsCode { get; set; }
    /// <summary>
    /// Generate new token (instead of removing old one)
    /// </summary>
    [ProtoMember(2)]
    public bool generate_new_token { get; set; }
    /// <summary>
    /// What the version of our token should be
    /// </summary>
    [ProtoMember(3)]
    public uint version { get; set; }
}
