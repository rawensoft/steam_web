using System.Text.Json.Serialization;

namespace SteamWeb.Models;
public class WalletInfo
{
    [JsonPropertyName("wallet_country")] public string? WalletCountry { get; init; }
    [JsonPropertyName("wallet_currency")] public uint WalletCurrency { get; init; }
    [JsonPropertyName("wallet_state")] public string? WalletState { get; init; }
    [JsonPropertyName("wallet_fee")] public uint WalletFee { get; init; }
    [JsonPropertyName("wallet_fee_minimum")] public uint WalletFeeMinimum { get; init; }
    [JsonPropertyName("wallet_fee_percent")] public decimal WalletFeePercent { get; init; }
    [JsonPropertyName("wallet_publisher_fee_percent_default")] public decimal WalletPublisherFeePercentDefault { get; init; }
    [JsonPropertyName("wallet_fee_base")] public decimal WalletFeeBase { get; init; }
    [JsonPropertyName("wallet_balance")] public uint WalletBalance { get; init; }
    [JsonPropertyName("wallet_delayed_balance")] public uint WalletDelayedBalance { get; init; }
    [JsonPropertyName("wallet_max_balance")] public uint WalletMaxBalance { get; init; }
    [JsonPropertyName("wallet_trade_max_balance")] public uint WalletTradeMaxBalance { get; init; }
    [JsonPropertyName("success")] public EResult Success { get; init; }
    [JsonPropertyName("rwgrsn")] public sbyte Rwgrsn { get; init; }
}