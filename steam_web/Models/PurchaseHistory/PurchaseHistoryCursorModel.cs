using System.Text.Json.Serialization;

namespace SteamWeb.Models.PurchaseHistory;
/// <summary>
/// Класс используется для загрузки следующих данных в истории покупок аккаунта.
/// <para/>
/// Не рекомендуется заполнять абы как!
/// </summary>
public class PurchaseHistoryCursorModel
{
    [JsonPropertyName("wallet_txnid")] public ulong WalletTxnId { get; init; }
    [JsonPropertyName("timestamp_newest")] public int TimestampNewest { get; init; }
    [JsonPropertyName("balance")] public uint Balance { get; init; }
    [JsonPropertyName("currency")] public byte Currency { get; init; }
}