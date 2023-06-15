using ProtoBuf;

namespace SteamWeb.Auth.v2.DTO;

[ProtoContract]
public class CAuthentication_GetAuthSessionsForAccount_Response
{
    [ProtoMember(1)] public ulong[] client_ids { get; set; } = new ulong[0];
}

