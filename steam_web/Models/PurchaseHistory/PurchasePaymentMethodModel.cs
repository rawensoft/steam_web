namespace SteamWeb.Models.PurchaseHistory;
public class PurchasePaymentMethodModel
{
    private const char _tabCh = '\t';
    private const string _newLineStr = "\n";

    /// <summary>
    /// Отображает какой метод оплаты был использован при использовании этой покупки
    /// </summary>
    public string Method { get; init; }
    /// <summary>
    /// Указывается сумма, если было несколько методов оплаты, иначе null
    /// </summary>
    public string? Amount { get; init; }

    internal static PurchasePaymentMethodModel Parse(string textContent)
    {
        string? amount = null;
        string? method = null;
        string[] splitted = textContent.Split(_tabCh);
        foreach (var item in splitted)
        {
            if (!string.IsNullOrEmpty(item) && item != _newLineStr)
            {
                if (string.IsNullOrEmpty(amount))
                    amount = item;
                else
                {
                    method = item;
                    break;
                }
            }
        }

        if (method != null)
            return new() { Method = method!, Amount = amount };
        return new() { Method = amount! };
    }
}