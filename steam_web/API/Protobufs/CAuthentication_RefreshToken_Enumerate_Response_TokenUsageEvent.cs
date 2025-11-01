using ProtoBuf;
using System.Text.Json.Serialization;

namespace SteamWeb.API.Protobufs;

[ProtoContract]
public class CAuthentication_RefreshToken_Enumerate_Response_TokenUsageEvent
{
    [ProtoMember(1)]
    [JsonPropertyName("time")]
    public uint Time { get; init; }

    [ProtoMember(2)]
    [JsonPropertyName("ip")]
    public CMsgIPAddress? Ip { get; init; }

    [ProtoMember(3)]
    [JsonPropertyName("locale")]
    public string? Locale { get; init; }

    [ProtoMember(4)]
    [JsonPropertyName("country")]
    public string? Country { get; init; }

    [ProtoMember(5)]
    [JsonPropertyName("state")]
    public string? State { get; init; }

    [ProtoMember(6)]
    [JsonPropertyName("city")]
    public string? City { get; init; }
}