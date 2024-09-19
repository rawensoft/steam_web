namespace SteamWeb.Models.PurchaseHistory;
public class PurchasePaymentGameModel
{
    /// <summary>
    /// Название игры, программы или чего-либо ещё
    /// </summary>
    public string Name { get; init; }
    /// <summary>
    /// Username аккаунта, на который отправлен подарок.
    /// <para/>
    /// Указывается, если <see cref="PurchaseHistoryModel.Type"/>==<see cref="PURSHASE_TYPE.GiftPurchase"/> или <see cref="PURSHASE_TYPE.Refund"/>, если был возврат за такую покупку.
    /// </summary>
    public string? AccountName { get; set; }
    /// <summary>
    /// SteamId32 аккаунта, на который отправлен подарок.
    /// <para/>
    /// Указывается, если <see cref="PurchaseHistoryModel.Type"/>==<see cref="PURSHASE_TYPE.GiftPurchase"/> или <see cref="PURSHASE_TYPE.Refund"/>, если был возврат за такую покупку.
    /// </summary>
    public uint? AccountId { get; set; }

    public PurchasePaymentGameModel() { }
    public PurchasePaymentGameModel(string name) => Name = name;
}