using System.Text.Json.Serialization;

namespace SteamWeb.Models.PurchaseHistory;
public class PurchasePaymentMethodModel
{
    private const char _tabCh = '\t';
    private const string _newLineStr = "\n";
    private const string _wallet = "Wallet";
    private const string _walletCredit = "Wallet Credit";
    private const string _retail = "Retail";
    private const string _kiosk = "Kiosk";
    private const string _payPal = "PayPal";

    /// <summary>
    /// Отображает какой метод оплаты был использован при использовании этой покупки
    /// </summary>
    public required string Method { get; init; }
    /// <summary>
    /// Указывается сумма, если было несколько методов оплаты, иначе null
    /// </summary>
    public string? Amount { get; init; }
    [JsonIgnore] public bool IsWalletMethod => Method == _wallet || Method == _walletCredit;
    [JsonIgnore] public bool IsRetailMethod => Method == _retail;
    [JsonIgnore] public bool IsKioskMethod => Method == _kiosk;
    [JsonIgnore] public bool IsPayPalMethod => Method == _payPal;

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
                    if (item.StartsWith(PurchasePaymentMethods.MasterCard, StringComparison.OrdinalIgnoreCase))
                        method = PurchasePaymentMethods.MasterCard;
                    else if (item!.StartsWith(PurchasePaymentMethods.VISA, StringComparison.OrdinalIgnoreCase))
                        method = PurchasePaymentMethods.VISA;
                    else if (item!.StartsWith(PurchasePaymentMethods.Mir, StringComparison.OrdinalIgnoreCase))
                        method = PurchasePaymentMethods.Mir;
                    else
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