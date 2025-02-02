using ProtoBuf;
using SteamWeb.Auth.v2.Enums;

namespace SteamWeb.Auth.v2.Models;

[ProtoContract]
class AuthSessionDesktopRequest
{
    private string wesiteid = null!;
    [ProtoMember(2)] public string account_name { get; init; }
    [ProtoMember(3)] public string encrypted_password { get; init; }
    [ProtoMember(4)] public ulong encryption_timestamp { get; init; }
    [ProtoMember(5)] public bool remember_login { get; init; } = true;
    [ProtoMember(7)] public ESessionPersistence persistence { get; init; } = ESessionPersistence.Persistent;
	/// <summary>
	/// Доступные для использования id:
	/// <br/>
	/// Client = Steam Desktop Client
	/// <br/>
	/// Mobile = Steam Mobile App
	/// <br/>
	/// Community = steamcommunity.com
	/// <br/>
	/// Store = store.steampowered.com
	/// <br/>
	/// Help = help.steampowered.com
	/// </summary>
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
    [ProtoMember(9)] public CAuthentication_DeviceDetails_Desktop device_details { get; init; } = new();
}
