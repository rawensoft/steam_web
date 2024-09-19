using SteamWeb.Enums;

namespace SteamWeb.Models.PurchaseHistory;
public class PurchaseHistoryModel
{
    /// <summary>
    /// Обозначает игру, в которой была совершена покупка.
    /// <para/>
    /// Обычно указано когда <see cref="Type"/>==<see cref="PURCHASE_TYPE.InGamePurchase"/>, но может быть и при <see cref="Type"/>==<see cref="PURCHASE_TYPE.Refund"/>, если возврат за <see cref="PURCHASE_TYPE.InGamePurchase"/>.
    /// </summary>
    public uint? AppId { get; init; }
    /// <summary>
    /// Указывается id транзакции, но не при <see cref="Type"/>==<see cref="PURCHASE_TYPE.MarketTransactions"/> или <see cref="PURCHASE_TYPE.MarketTransaction"/>
    /// </summary>
    public ulong? TransactionId { get; init; }
    /// <summary>
    /// Указывается что было куплено.
    /// <para/>
    /// При нормальном парсинге данных длинна всегда >0
    /// </summary>
    public PurchasePaymentGameModel[] Items { get; init; }
    /// <summary>
    /// Дополнительная информация о покупаемом предмете.
    /// <para/>
    /// Обозначает покупаемый предмет(-ы). Записывается как "количество название_предмета".
    /// <para/>
    /// Обычно указано когда <see cref="Type"/>==<see cref="PURCHASE_TYPE.InGamePurchase"/>, но может быть и при <see cref="Type"/>==<see cref="PURCHASE_TYPE.Refund"/>, если возврат за <see cref="PURCHASE_TYPE.InGamePurchase"/>.
    /// </summary>
    public string? ItemDescription { get; init; }
    /// <summary>
    /// День, месяц и год, когда покупка была сделана.
    /// <para/>
    /// Не было выяснено по какому времени они ориентируются. Возможно, высчитывается часовой пояс на основе ip и на основе этой информации выдают дату.
    /// </summary>
    public DateOnly Date { get; init; }
    /// <summary>
    /// Тип покупки
    /// </summary>
    public PURCHASE_TYPE Type { get; init; }
    /// <summary>
    /// Использовавшиеся методы платежей в этой покупке
    /// </summary>
    public PurchasePaymentMethodModel[] PaymentMethods { get; init; }
    /// <summary>
    /// Полная стоимость этой покупки.<para/>При некоторых покупках информации может не быть. Рекомендуется смотреть <see cref="PaymentMethods"/>.
    /// </summary>
    public string? Total { get; init; }
    /// <summary>
    /// Значение баланса до выполнения покупки.<para/>При некоторых покупках информации может не быть.
    /// </summary>
    public string? PreviousBalance { get; init; }
    /// <summary>
    /// Значение на которое изменяется баланс. Может иметь + или -, в зависимости от типа операции.
    /// <para/>При некоторых покупках информации может не быть. Рекомендуется смотреть <see cref="PaymentMethods"/>.
    /// </summary>
    public string? Change { get; init; }
    /// <summary>
    /// Обозначает новый баланс после покупки.<para/>При некоторых покупках информации может не быть.
    /// </summary>
    public string? NewBalance { get; init; }
    /// <summary>
    /// Обычно означает что баланс зачислился именно с продажи предметов на маркете.
    /// <para/>
    /// Возможно это может означать что-то ещё, но этого не было замечено.
    /// </summary>
    public bool IsCredit { get; init; } = false;
    /// <summary>
    /// true если это покупка, которая должна отмениться, либо если <see cref="Type"/>==<see cref="PURCHASE_TYPE.Refund"/>
    /// </summary>
    public bool HasRefund { get; init; } = false;
}