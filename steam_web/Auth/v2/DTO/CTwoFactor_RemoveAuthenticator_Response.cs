using ProtoBuf;

namespace SteamWeb.Auth.v2.DTO;

[ProtoContract]
public class CTwoFactor_RemoveAuthenticator_Response
{
    [ProtoMember(1)] public bool success { get; set; } = false;
    /// <summary>
    /// Reason the authenticator is being removed
    /// </summary>
    //[ProtoMember(3)] public long server_time { get; set; }
    /// <summary>
    /// Type of Steam Guard to use once token is removed
    /// </summary>
    [ProtoMember(5)] public int revocation_attempts_remaining { get; set; } = -1;
}

