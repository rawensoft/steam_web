using ProtoBuf;
namespace SteamWeb.Auth.v2;

[ProtoContract]
class PasswordRSARequest
{
    [ProtoMember(1)] public string account_name { get; set; }
}

[ProtoContract]
class PasswordRSAResponse
{
    [ProtoMember(1)] public string publickey_mod { get; set; }
    [ProtoMember(2)] public string publickey_exp { get; set; }
    [ProtoMember(3)] public ulong timestamp { get; set; }
}
