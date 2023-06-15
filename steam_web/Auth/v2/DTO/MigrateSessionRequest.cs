using ProtoBuf;

namespace SteamWeb.Auth.v2.Models;

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
