using ProtoBuf;

namespace SteamWeb.Auth.v2.DTO;

[ProtoContract]
class CTwoFactor_RemoveAuthenticator_Request
{
    [ProtoMember(2)]
    public string revocation_code { get; set; }
    /// <summary>
    /// Reason the authenticator is being removed
    /// </summary>
    [ProtoMember(5)]
    public int revocation_reason { get; set; }
    /// <summary>
    /// Type of Steam Guard to use once token is removed
    /// </summary>
    [ProtoMember(6)]
    public int steamguard_scheme { get; set; }
    /// <summary>
    /// Remove all steamguard cookies
    /// </summary>
    [ProtoMember(7)]
    public bool remove_all_steamguard_cookies { get; set; } = true;
}

