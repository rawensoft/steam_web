using ProtoBuf;
using SteamWeb.Auth.v2.Enums;

namespace SteamWeb.Auth.v2.Models;

[ProtoContract]
class CAuthentication_DeviceDetails_Mobile
{
    /// <summary>
    /// device_friendly_name
    /// </summary>
    [ProtoMember(1)] public string device_friendly_name { get; init; } = "F8331";
    [ProtoMember(2)] public EAuthTokenPlatformType platform_type { get; init; } = EAuthTokenPlatformType.MobileApp;
    [ProtoMember(3)] public ulong os_type { get; init; } = 18446744073709551116;
    [ProtoMember(4)] public uint gaming_device_type { get; init; } = 528;
}