using System.Text.Json.Serialization;
using ProtoBuf;

namespace SteamWeb.API.Protobufs;

[ProtoContract]
public class CSteamAwards_GetNominationRecommendations_Response
{
    [ProtoMember(1)]
    [JsonPropertyName("played_app")]
    public CSteamAwards_PlayedApp[] PlayedApp { get; init; } = Array.Empty<CSteamAwards_PlayedApp>();

    [ProtoMember(3)]
    [JsonPropertyName("suggested_apps")]
    public CSteamAwards_SuggestedApp[] SuggestedApps { get; init; } = Array.Empty<CSteamAwards_SuggestedApp>();
}
