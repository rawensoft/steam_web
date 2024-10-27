using ProtoBuf;

namespace SteamWeb.Auth.v2.Models;

[ProtoContract]
class AuthSessionMobileRequest
{
    private string? wesiteid = null;

    [ProtoMember(2)] public string account_name { get; set; }
    [ProtoMember(3)] public string encrypted_password { get; set; }
    [ProtoMember(4)] public ulong encryption_timestamp { get; set; }
    [ProtoMember(5)] public bool remember_login { get; set; } = true;
    [ProtoMember(7)] public ESessionPersistence persistence { get; set; } = ESessionPersistence.Persistent;
    [ProtoMember(8)]
    public string website_id
    {
        get
        {
            if (wesiteid != null)
                return wesiteid;
            switch (device_details.platform_type)
            {
                case EAuthTokenPlatformType.SteamClient:
                    return wesiteid = "Client";
                case EAuthTokenPlatformType.MobileApp:
                    return wesiteid = "Mobile";
                default:
                    return wesiteid = "Community";
            }
        }
        set => wesiteid = value;
    }
    [ProtoMember(9)] public CAuthentication_DeviceDetails_Mobile device_details { get; set; } = new();
    [ProtoMember(11)] public int language { get; set; } = 0;
}