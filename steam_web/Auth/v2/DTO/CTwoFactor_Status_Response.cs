using ProtoBuf;

namespace SteamWeb.Auth.v2.Models;

[ProtoContract]
class CTwoFactor_Status_Response
{
    /// <summary>
    /// Authenticator state
    /// </summary>
    [ProtoMember(1)] public int state { get; set; }
    /// <summary>
    /// Inactivation reason (if any)
    /// </summary>
    [ProtoMember(2)] public int inactivation_reason { get; set; }
    /// <summary>
    /// Type of authenticator
    /// </summary>
    [ProtoMember(3)] public int authenticator_type { get; set; }
    /// <summary>
    /// Account allowed to have an authenticator?
    /// </summary>
    [ProtoMember(4)] public bool authenticator_allowed { get; set; } = false;
    /// <summary>
    /// Steam Guard scheme in effect
    /// </summary>
    [ProtoMember(5)] public int steamguard_scheme { get; set; }
    /// <summary>
    /// String rep of token GID assigned by server
    /// </summary>
    [ProtoMember(6)] public string token_gid { get; set; }
    /// <summary>
    /// Account has verified email capability
    /// </summary>
    [ProtoMember(7)] public bool email_validated { get; set; } = false;
    /// <summary>
    /// Authenticator (phone) identifier
    /// </summary>
    [ProtoMember(8)] public string device_identifier { get; set; }
    /// <summary>
    /// When the token was created
    /// </summary>
    [ProtoMember(9)] public int time_created { get; set; }
    /// <summary>
    /// Number of revocation code attempts remaining
    /// </summary>
    [ProtoMember(10)] public int revocation_attempts_remaining { get; set; }
    /// <summary>
    /// Agent that added the authenticator (e.g., ios / android / other)
    /// </summary>
    [ProtoMember(11)] public string classified_agent { get; set; }
    /// <summary>
    /// Allow a third-party authenticator (in addition to two-factor)
    /// </summary>
    [ProtoMember(12)] public bool allow_external_authenticator { get; set; } = false;
    /// <summary>
    /// When the token was transferred from another device, if applicable
    /// </summary>
    [ProtoMember(13)] public int time_transferred { get; set; }
}
