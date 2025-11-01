using ProtoBuf;
using System.Text.Json.Serialization;

namespace SteamWeb.API.Protobufs;

[ProtoContract]
public class CAuthentication_RefreshToken_Enumerate_Request
{
    [ProtoMember(1, IsRequired = false)]
    [JsonPropertyName("include_revoked")]
    public bool IncludeRevoked { get; init; } = false;
}
