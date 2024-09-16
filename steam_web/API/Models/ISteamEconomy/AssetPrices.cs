using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.ISteamEconomy;
public class AssetPrices
{
    [JsonPropertyName("success")] public bool Success { get; init; } = false;
    [JsonPropertyName("assets")] public AssetModel[] Assets { get; init; } = Array.Empty<AssetModel>();
}