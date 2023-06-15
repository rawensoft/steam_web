using ProtoBuf;

namespace SteamWeb.Auth.v2.Models;

[ProtoContract]
class CTwoFactor_AddAuthenticator_Request
{
    [ProtoMember(1, DataFormat = DataFormat.FixedSize)] public ulong steamid { get; set; }
    //[ProtoMember(2)] public ulong authenticator_time { get; set; }
    //[ProtoMember(3)] public ulong serial_number { get; set; }
    [ProtoMember(4)] public uint authenticator_type { get; set; } = 1;
    /// <summary>
    /// DeviceID, example: android:c4397b5c-d49a-4987-80dc-70e8129c61d0
    /// </summary>
    [ProtoMember(5)] public string device_identifier { get; set; }
    [ProtoMember(6)] public string sms_phone_id { get; set; } = "1";
    //[ProtoMember(7)] public string http_headers { get; set; }
    /// <summary>
    /// What the version of our token should be
    /// </summary>
    [ProtoMember(8)] public uint version { get; set; } = 2;
}
