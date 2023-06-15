using ProtoBuf;

namespace SteamWeb.Auth.v2.Models;

[ProtoContract]
public class CUserAccount_GetClientWalletDetails_Request
{
    [ProtoMember(1)] public bool include_balance_in_usd { get; set; } = true;
    /// <summary>
    /// 2?
    /// </summary>
    [ProtoMember(2)] public int wallet_region { get; set; } = 1;
    [ProtoMember(3)] public bool include_formatted_balance { get; set; } = true;
}
