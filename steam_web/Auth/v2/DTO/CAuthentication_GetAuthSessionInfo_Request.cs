using ProtoBuf;

namespace SteamWeb.Auth.v2.DTO;

[ProtoContract]
public class CAuthentication_GetAuthSessionInfo_Request
{
    [ProtoMember(1)] public uint client_id { get; set; }
}

