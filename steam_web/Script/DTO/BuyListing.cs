using SteamWeb.Models;
using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO;
public class BuyListing
{
    [JsonPropertyName("success")]
    public EResult Success { get; init; }

    [JsonPropertyName("need_confirmation")]
    public bool NeedConfirmation { get; init; } = false;

    [JsonPropertyName("confirmation")]
    public MarketConfirmation? Confirmation { get; init; }

    [JsonPropertyName("wallet_info")]
    public WalletInfo? WalletInfo { get; init; }
}
