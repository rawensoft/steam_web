using ProtoBuf;

namespace SteamWeb.Auth.v2.DTO;

[ProtoContract]
public class CAuthentication_GetAuthSessionInfo_Response
{
    [ProtoMember(1)] public string ip { get; set; }
    [ProtoMember(2)] public string geoloc { get; set; }
    [ProtoMember(3)] public string city { get; set; }
    [ProtoMember(4)] public string state { get; set; }
    [ProtoMember(5)] public string country { get; set; }
    /// <summary>
    /// platform type of requestor
    /// </summary>
    [ProtoMember(6)] public Enums.EAuthTokenPlatformType platform_type { get; set; }
    /// <summary>
    /// name of requestor device
    /// </summary>
    [ProtoMember(7)] public string device_friendly_name { get; set; }
    [ProtoMember(8)] public int version { get; set; }
    /// <summary>
    /// whether the ip has previuously been used on the account successfully
    /// </summary>
    [ProtoMember(9)] public EAuthSessionSecurityHistory login_history { get; set; }
    /// <summary>
    /// whether the requestor location matches this requests location
    /// </summary>
    [ProtoMember(10)] public bool requestor_location_mismatch { get; set; }
    /// <summary>
    /// whether this login has seen high usage recently
    /// </summary>
    [ProtoMember(11)] public bool high_usage_login { get; set; }
    [ProtoMember(12)] public ESessionPersistence requested_persistence { get; set; }
}

