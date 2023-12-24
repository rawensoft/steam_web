using ProtoBuf;

namespace SteamWeb.Auth.v2.Models;

[ProtoContract]
class CTwoFactor_FinalizeAddAuthenticator_Response
{
    /// <summary>
    /// True if succeeded, or want more tries
    /// </summary>
    [ProtoMember(1)] public bool success { get; set; } = false;
    /// <summary>
    /// Current server time
    /// </summary>
    [ProtoMember(3)] public int server_time { get; set; }
    /// <summary>
    /// Result code. 2 - это success=true
    /// </summary>
    [ProtoMember(4)] public int status { get; set; }
}
