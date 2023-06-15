namespace SteamWeb.Script.DTO;

public class SteamPurchaseCursor
{
    public string balance { get; init; }
    public ushort currency { get; init; }
    public int timestamp_newest { get; init; }
    public string wallet_txnid { get; init; }
}
