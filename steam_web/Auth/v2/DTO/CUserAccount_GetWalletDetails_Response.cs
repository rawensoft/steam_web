using ProtoBuf;

namespace SteamWeb.Auth.v2.Models;

[ProtoContract]
public class CUserAccount_GetWalletDetails_Response
{
    [ProtoMember(1)] public bool has_wallet { get; set; } = false;
    [ProtoMember(2)] public string user_country_code { get; set; }
    [ProtoMember(3)] public string wallet_country_code { get; set; }
    [ProtoMember(4)] public string wallet_state { get; set; }
    [ProtoMember(5)] public long balance { get; set; }
    [ProtoMember(6)] public long delayed_balance { get; set; }
    /// <summary>
    /// Текущий index валюты аккаунта
    /// </summary>
    [ProtoMember(7)] public int currency_code { get; set; }
    [ProtoMember(8)] public uint time_most_recent_txn { get; set; }
    [ProtoMember(9)] public ulong most_recent_txnid { get; set; }
    [ProtoMember(10)] public long balance_in_usd { get; set; }
    [ProtoMember(11)] public long delayed_balance_in_usd { get; set; }
    [ProtoMember(12)] public bool has_wallet_in_other_regions { get; set; } = false;
    [ProtoMember(13)] public int other_regions { get; set; }
    [ProtoMember(14)] public string formatted_balance { get; set; }
}
