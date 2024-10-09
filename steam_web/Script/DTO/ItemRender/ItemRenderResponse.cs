using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO.ItemRender;
public class ItemRenderResponse
{
    [JsonPropertyName("success")] public bool Success { get; init; } = false;
    [JsonPropertyName("start")] public uint Start { get; init; }
    [JsonPropertyName("pagesize")] public ushort PageSize { get; init; }
    [JsonPropertyName("total_count")] public uint TotalCount { get; init; }
    [JsonPropertyName("listinginfo")] public Dictionary<ulong, ItemRenderListing> ListingInfo { get; init; } = new(2);
    [JsonPropertyName("assets")] public Dictionary<uint, Dictionary<byte, Dictionary<ulong, ItemRenderAsset>>> Assets { get; init; } = new(2);
    [JsonPropertyName("app_data")] public Dictionary<uint, ItemRenderAppData> AppData { get; init; } = new(2);
}