namespace SteamWeb.Script.DTO;

public class SteamPurchase
{
    /// <summary>
    /// Если IsInGamePurchase == True, то все названия купленных предметов, иначе индекс 0 название игры, а всё остальное название предметов из этой игры
    /// </summary>
    public string[] Names { get; init; } = new string[0];
    /// <summary>
    /// Возвращает null, если Names.Count == 0, иначе первую строку в Names
    /// </summary>
    public string Name => Names.Length == 0 ? null : Names[0];
    public bool IsRefunded { get; init; } = false;
    /// <summary>
    /// Эта операция связана с SCM
    /// </summary>
    public bool IsSteamCommunity { get; init; } = false;
    /// <summary>
    /// Это внутри-игровая операция
    /// </summary>
    public bool IsInGamePurchase { get; init; } = false;
    public string Date { get; init; }
    /// <summary>
    /// Если начинается на +, то эта операция прибавила баланс, а если -, то уменьшила
    /// </summary>
    public string WalletChange { get; init; }
    /// <summary>
    /// Балансе после этой операции
    /// </summary>
    public string WalletBalance { get; init; }
    public string Type { get; init; }
    /// <summary>
    /// Способы оплаты, если Count == 1, то содержит тип оплаты, если Count > 1, то сумму оплаты + способ оплаты
    /// </summary>
    public string[] Payments { get; init; } = new string[0];
    /// <summary>
    /// Возвращает null, если Payments.Count == 0, иначе первую строку в Payments
    /// </summary>
    public string Payment => Payments.Length == 0 ? null : Payments[0];
    public string Total { get; init; }
    /// <summary>
    /// Доп информация для Total. Может быть Null.
    /// </summary>
    public string TotalPayment { get; init; }
}
