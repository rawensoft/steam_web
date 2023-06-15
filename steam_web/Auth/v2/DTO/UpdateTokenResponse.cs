using ProtoBuf;

namespace SteamWeb.Auth.v2.DTO;

[ProtoContract]
public class UpdateTokenResponse
{
    [ProtoMember(1)] public string access_token { get; set; }
}
