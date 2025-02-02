using ProtoBuf;
using SteamWeb.Auth.v2.Enums;

namespace SteamWeb.Auth.v2.Models;

[ProtoContract]
class CAuthentication_DeviceDetails_Desktop
{
    /// <summary>
    /// device_friendly_name
    /// </summary>
    [ProtoMember(1)] public string device_friendly_name { get; init; } = "device_friendly_name";
    [ProtoMember(2)] public EAuthTokenPlatformType platform_type { get; init; } = EAuthTokenPlatformType.WebBrowser;
}