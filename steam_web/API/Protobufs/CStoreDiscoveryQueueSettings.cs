using System.Text.Json.Serialization;
using ProtoBuf;

namespace SteamWeb.API.Protobufs;

[ProtoContract]
public class CStoreDiscoveryQueueSettings
{
    [ProtoMember(4)]
    [JsonPropertyName("os_win")]
    public bool os_win { get; init; } = false;

    [ProtoMember(5)]
    [JsonPropertyName("os_mac")]
    public bool os_mac { get; init; } = false;

    [ProtoMember(6)]
    [JsonPropertyName("os_linux")]
    public bool os_linux { get; init; } = false;

    [ProtoMember(7)]
    [JsonPropertyName("full_controller_support")]
    public bool full_controller_support { get; init; } = false;

    [ProtoMember(8)]
    [JsonPropertyName("native_steam_controller")]
    public bool native_steam_controller { get; init; } = false;

    [ProtoMember(9)]
    [JsonPropertyName("include_coming_soon")]
    public bool include_coming_soon { get; init; } = false;

    /// <summary>
    /// Don't return any games with these tags.
    /// </summary>
    [ProtoMember(10)]
    [JsonPropertyName("excluded_tagids")]
    public uint[]? excluded_tagids { get; init; }

    [ProtoMember(11)]
    [JsonPropertyName("exclude_early_access")]
    public bool exclude_early_access { get; init; } = false;

    [ProtoMember(12)]
    [JsonPropertyName("exclude_videos")]
    public bool exclude_videos { get; init; } = false;

    [ProtoMember(13)]
    [JsonPropertyName("exclude_software")]
    public bool exclude_software { get; init; } = false;

    [ProtoMember(14)]
    [JsonPropertyName("exclude_dlc")]
    public bool exclude_dlc { get; init; } = false;

    [ProtoMember(15)]
    [JsonPropertyName("exclude_soundtracks")]
    public bool exclude_soundtracks { get; init; } = false;

    /// <summary>
    /// Must be marked with one of these featured tagids (for sale pages and events)
    /// </summary>
    [ProtoMember(16)]
    [JsonPropertyName("featured_tagids")]
    public uint[]? featured_tagids { get; init; }
}