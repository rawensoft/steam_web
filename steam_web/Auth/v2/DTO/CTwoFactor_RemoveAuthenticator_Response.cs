using ProtoBuf;

namespace SteamWeb.Auth.v2.DTO;

[ProtoContract]
class CTwoFactor_RemoveAuthenticator_Response
{
    [ProtoMember(1)] public bool success { get; set; }
    /// <summary>
    /// Reason the authenticator is being removed
    /// </summary>
    //[ProtoMember(3)] public ulong server_time { get; set; }
    /// <summary>
    /// Type of Steam Guard to use once token is removed
    /// </summary>
    [ProtoMember(5)] public uint revocation_attempts_remaining { get; set; }
}

