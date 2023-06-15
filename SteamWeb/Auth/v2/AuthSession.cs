using ProtoBuf;

namespace SteamWeb.Auth.v2;

[ProtoContract]
public class CUserAccount_GetClientWalletDetails_Request
{
    [ProtoMember(1)] public bool include_balance_in_usd { get; set; } = true;
    /// <summary>
    /// 2?
    /// </summary>
    [ProtoMember(2)] public int wallet_region { get; set; } = 1;
    [ProtoMember(3)] public bool include_formatted_balance { get; set; } = true;
}
[ProtoContract]
public class CUserAccount_GetWalletDetails_Response
{
    [ProtoMember(1)] public bool has_wallet { get; set; } = false;
    [ProtoMember(2)] public string user_country_code { get; set; }
    [ProtoMember(3)] public string wallet_country_code { get; set; }
    [ProtoMember(4)] public string wallet_state { get; set; }
    [ProtoMember(5)] public long balance { get; set; }
    [ProtoMember(6)] public long delayed_balance { get; set; }
    /// <summary>
    /// Текущий index валюты аккаунта
    /// </summary>
    [ProtoMember(7)] public int currency_code { get; set; }
    [ProtoMember(8)] public uint time_most_recent_txn { get; set; }
    [ProtoMember(9)] public ulong most_recent_txnid { get; set; }
    [ProtoMember(10)] public long balance_in_usd { get; set; }
    [ProtoMember(11)] public long delayed_balance_in_usd { get; set; }
    [ProtoMember(12)] public bool has_wallet_in_other_regions { get; set; } = false;
    [ProtoMember(13)] public int other_regions { get; set; }
    [ProtoMember(14)] public string formatted_balance { get; set; }
}

[ProtoContract]
class MobileSummeryRequest
{
    /// <summary>
    /// 630870302758561097
    /// </summary>
    [ProtoMember(1, DataFormat = DataFormat.FixedSize)] public ulong data { get; set; }
}

[ProtoContract]
class MigrateSessionRequest
{
    /// <summary>
    /// WebToken
    /// </summary>
    [ProtoMember(1)] public string token { get; set; }
    [ProtoMember(2, DataFormat = DataFormat.FixedSize)] public ulong steamid { get; set; }
    [ProtoMember(3)] public string signature { get; set; }
}

[ProtoContract]
class UpdateTokenRequest
{
    [ProtoMember(1)] public string refresh_token { get; set; }
    [ProtoMember(2, DataFormat = DataFormat.FixedSize)] public ulong steamid { get; set; }
}
[ProtoContract]
public class UpdateTokenResponse
{
    [ProtoMember(1)] public string access_token { get; set; }
}
[ProtoContract]
class AuthSessionDesktopRequest
{
    private string wesiteid = null;
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
            if (wesiteid != null) return wesiteid;
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
    [ProtoMember(9)] public CAuthentication_DeviceDetails_Desktop device_details { get; set; } = new();
}
[ProtoContract]
class CAuthentication_DeviceDetails_Desktop
{
    /// <summary>
    /// device_friendly_name
    /// </summary>
    [ProtoMember(1)] public string device_friendly_name { get; set; } = "device_friendly_name";
    [ProtoMember(2)] public EAuthTokenPlatformType platform_type { get; set; } = EAuthTokenPlatformType.WebBrowser;
}
[ProtoContract]
class AuthSessionMobileRequest
{
    private string wesiteid = null;
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
            if (wesiteid != null) return wesiteid;
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
[ProtoContract]
class CAuthentication_DeviceDetails_Mobile
{
    /// <summary>
    /// device_friendly_name
    /// </summary>
    [ProtoMember(1)] public string device_friendly_name { get; set; } = "F8331";
    [ProtoMember(2)] public EAuthTokenPlatformType platform_type { get; set; } = EAuthTokenPlatformType.MobileApp;
    [ProtoMember(3)] public ulong os_type { get; set; } = 18446744073709551116;
    [ProtoMember(4)] public uint gaming_device_type { get; set; } = 528;
}
[ProtoContract]
class AuthSessionResponse
{
    [ProtoMember(1)] public ulong client_id { get; set; }
    [ProtoMember(2)] public byte[] request_id { get; set; }
    [ProtoMember(3)] public long interval { get; set; }
    [ProtoMember(4)] public AuthSessionConfirmation[] allowed_confirmations { get; set; } = new AuthSessionConfirmation[0];
    [ProtoMember(5, DataFormat = DataFormat.FixedSize)] public ulong steamid { get; set; }
    [ProtoMember(6)] public string weak_token { get; set; }
    /// <summary>
    /// if login has been confirmed, may contain remembered machine ID for future login
    /// </summary>
    [ProtoMember(7)] public string new_guard_data { get; set; }
}
[ProtoContract]
class AuthSessionConfirmation
{
    [ProtoMember(1)] public EAuthSessionGuardType message { get; set; }
    [ProtoMember(2)] public string associated_message { get; set; }
}


[ProtoContract]
class AuthPollRequest
{
    [ProtoMember(1)] public ulong client_id { get; set; }
    [ProtoMember(2)] public byte[] request_id { get; set; }
    //[ProtoMember(3)] public string token_to_revoke { get; set; } = null;
}
[ProtoContract]
class AuthPollResponse
{
    [ProtoMember(1)] public string new_client_id { get; set; }
    [ProtoMember(2)] public string new_challenge_url { get; set; }
    [ProtoMember(3)] public string refresh_token { get; set; }
    /// <summary>
    /// steamLoginSecure={steamid64}%7C%7C{access_token}
    /// </summary>
    [ProtoMember(4)] public string access_token { get; set; }
    /// <summary>
    /// whether or not the auth session appears to have had remote interaction from a potential confirmer
    /// </summary>
    [ProtoMember(5)] public bool had_remote_interaction { get; set; } = false;
    [ProtoMember(6)] public string account_name { get; set; }
}

[ProtoContract]
class AuthGuardRequest
{
    [ProtoMember(1)] public ulong client_id { get; set; }
    [ProtoMember(2, DataFormat = DataFormat.FixedSize)] public ulong steamid { get; set; }
    [ProtoMember(3)] public string code { get; set; }
    [ProtoMember(4)] public EAuthSessionGuardType code_type { get; set; } = EAuthSessionGuardType.DeviceCode;
}
[ProtoContract]
class AuthGuardResponse { }
