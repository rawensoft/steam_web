using ProtoBuf;

namespace SteamWeb.Auth.v2.Models;

[ProtoContract]
class MobileSummeryRequest
{
    [ProtoMember(1, DataFormat = DataFormat.FixedSize)] public ulong data { get; set; }
}
