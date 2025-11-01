using ProtoBuf;
using System.Text.Json.Serialization;

namespace SteamWeb.Auth.v2.DTO;

[ProtoContract]
public class UpdateTokenResponse
{
    [ProtoMember(1)]
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; init; }

	[ProtoMember(2)]
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; init; }
}