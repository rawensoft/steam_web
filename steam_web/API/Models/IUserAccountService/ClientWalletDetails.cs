using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IUserAccountService;
public class ClientWalletDetails
{
    [JsonPropertyName("has_wallet")] public bool HasWallet { get; init; } = false;
    [JsonPropertyName("user_country_code")] public string? UserCountryCode { get; init; }
    [JsonPropertyName("wallet_country_code")] public string? WalletCountryCode { get; init; }
    [JsonPropertyName("wallet_state")] public string WalletState { get; init; } = string.Empty;
    [JsonPropertyName("balance")] public uint Balance { get; init; }
    [JsonPropertyName("delayed_balance")] public uint DelayedBalance { get; init; }
    [JsonPropertyName("currency_code")] public byte CurrencyCode { get; init; }
    [JsonPropertyName("time_most_recent_txn")] public int TimeMostRecentTxn { get; init; }
    [JsonPropertyName("most_recent_txnid")] public string? MostRecentTxnId { get; init; }
    [JsonPropertyName("balance_in_usd")] public uint BalanceInUsd { get; init; }
    [JsonPropertyName("delayed_balance_in_usd")] public uint DelayedBalanceInUsd { get; init; }
    [JsonPropertyName("has_wallet_in_other_regions")] public bool HasWalletInOtherRegions { get; init; } = false;
    [JsonPropertyName("formatted_balance")] public string? FormattedBalance { get; init; }
    [JsonPropertyName("formatted_delayed_balance")] public string? FormattedDelayedBalance { get; init; }
    [JsonPropertyName("other_regions")] public byte[] OtherRegions { get; init; } = Array.Empty<byte>();
}