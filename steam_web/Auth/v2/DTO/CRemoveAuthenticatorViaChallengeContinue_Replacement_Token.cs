using ProtoBuf;

namespace SteamWeb.Auth.v2.DTO;

[ProtoContract]
public class CRemoveAuthenticatorViaChallengeContinue_Replacement_Token
{
    [ProtoMember(1)]
    public byte[] shared_secret { get; set; }

    [ProtoMember(2, DataFormat = DataFormat.FixedSize)]
    public ulong serial_number { get; set; }

    [ProtoMember(3)]
    public string revocation_code { get; set; }

    [ProtoMember(4)]
    public string uri { get; set; }

    [ProtoMember(5)]
    public int server_time { get; set; }

    [ProtoMember(6)]
    public string account_name { get; set; }

    [ProtoMember(7)]
    public string token_gid { get; set; }

    [ProtoMember(8)]
    public byte[] identity_secret { get; set; }

    [ProtoMember(9)]
    public byte[] secret_1 { get; set; }

    [ProtoMember(10)]
    public int status { get; set; }

    [ProtoMember(11)]
    public uint steamguard_scheme { get; set; }

    [ProtoMember(12, DataFormat = DataFormat.FixedSize)]
    public ulong steamid { get; set; }
}