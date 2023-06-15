using ProtoBuf;

namespace SteamWeb.Auth.v2.Models;

[ProtoContract]
class CTwoFactor_FinalizeAddAuthenticator_Request
{
    [ProtoMember(1, DataFormat = DataFormat.FixedSize)] public ulong steamid { get; set; }
    /// <summary>
    /// 2FA code
    /// </summary>
    [ProtoMember(2)] public string? authenticator_code { get; set; }
    /// <summary>
    /// Текущее время клиента
    /// </summary>
    [ProtoMember(3)] public int authenticator_time { get; set; }
    /// <summary>
    /// Activation code from out-of-band message
    /// </summary>
    [ProtoMember(4)] public string? activation_code { get; set; }
    //[ProtoMember(5)] public string http_headers { get; set; }
    /// <summary>
    /// When finalizing with an SMS code, pass the request on to the PhoneService to update its state too.
    /// </summary>
    [ProtoMember(6)]
    public bool validate_sms_code { get; set; } = true;
}
