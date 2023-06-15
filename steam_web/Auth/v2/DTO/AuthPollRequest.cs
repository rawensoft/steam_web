using ProtoBuf;

namespace SteamWeb.Auth.v2.Models;

[ProtoContract]
class AuthPollRequest
{
    [ProtoMember(1)] public ulong client_id { get; set; }
    [ProtoMember(2)] public byte[] request_id { get; set; }
    //[ProtoMember(3)] public string token_to_revoke { get; set; } = null;
}
