using ProtoBuf;

namespace SteamWeb.Auth.v2.Models;

[ProtoContract]
class CPhone_SetAccountPhoneNumber_Request
{
    /// <summary>
    /// example: +7 9773130028
    /// </summary>
    [ProtoMember(1)] public string phone_number { get; set; }
    /// <summary>
    /// example: RU
    /// </summary>
    [ProtoMember(2)] public string phone_country_code { get; set; }
}
