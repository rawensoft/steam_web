using ProtoBuf;
using System.Text.Json.Serialization;

namespace SteamWeb.API.Protobufs;

[ProtoContract]
public class CAuthentication_RefreshToken_Enumerate_Response
{
    [ProtoMember(1, IsRequired = true)]
    [JsonPropertyName("refresh_tokens")]
    public List<CAuthentication_RefreshToken_Enumerate_Response_RefreshTokenDescription> RefreshTokens { get; init; } = new(2);

    [ProtoMember(2, DataFormat = DataFormat.FixedSize)]
    [JsonPropertyName("requesting_token")]
    public ulong RequestingToken { get; init; }
}
