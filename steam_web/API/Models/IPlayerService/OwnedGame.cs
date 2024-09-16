using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IPlayerService;
public class OwnedGame
{
    [JsonPropertyName("appid")] public uint AppId { get; init; }
    [JsonPropertyName("playtime_2weeks")] public uint Playtime2Weeks { get; init; }
    [JsonPropertyName("playtime_forever")] public uint PlaytimeForever { get; init; }
    [JsonPropertyName("playtime_windows_forever")] public uint PlaytimeWindowsForever { get; init; }
    [JsonPropertyName("playtime_mac_forever")] public uint PlaytimeMacForever { get; init; }
    [JsonPropertyName("playtime_linux_forever")] public uint PlaytimeLinuxForever { get; init; }
    [JsonPropertyName("playtime_deck_forever")] public uint PlaytimeDeckForever { get; init; }
    [JsonPropertyName("playtime_disconnected")] public ushort PlaytimeDisconnected { get; init; }
    [JsonPropertyName("rtime_last_played")] public int RecentTimeLastPlayed { get; init; }

    /// <summary>
    /// Есть значение, если include_appinfo == true
    /// </summary>
    [JsonPropertyName("name")] public string? Name { get; init; }
    /// <summary>
    /// Есть значение, если include_appinfo == true
    /// </summary>
    [JsonPropertyName("img_icon_url")] public string? ImgIconUrl { get; init; }
    /// <summary>
    /// Возможно значение, если include_appinfo == true
    /// </summary>
    [JsonPropertyName("content_descriptorids")] public byte[]? ContentDescriptorIds { get; init; }
    /// <summary>
    /// Есть значение, если include_appinfo == true
    /// </summary>
    [JsonPropertyName("has_community_visible_stats")] public bool? HasCommunityVisibleStats { get; init; }

    /// <summary>
    /// Есть значение, если include_appinfo && include_extended_appinfo
    /// </summary>
    [JsonPropertyName("capsule_filename")] public string? CapsuleFilename { get; init; }
    /// <summary>
    /// Есть значение, если include_appinfo && include_extended_appinfo
    /// </summary>
    [JsonPropertyName("has_workshop")] public bool? HasWorkshop { get; init; }
    /// <summary>
    /// Есть значение, если include_appinfo && include_extended_appinfo
    /// </summary>
    [JsonPropertyName("has_market")] public bool? HasMarket { get; init; }
    /// <summary>
    /// Есть значение, если include_appinfo && include_extended_appinfo
    /// </summary>
    [JsonPropertyName("has_dlc")] public bool? HasDlc { get; init; }
}