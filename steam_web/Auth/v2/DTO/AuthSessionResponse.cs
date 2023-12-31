using ProtoBuf;

namespace SteamWeb.Auth.v2.Models;

[ProtoContract]
class AuthSessionResponse
{
    [ProtoMember(1)] public ulong client_id { get; set; }
    [ProtoMember(2)] public byte[] request_id { get; set; }
    [ProtoMember(3, DataFormat = DataFormat.FixedSize)] public uint interval { get; set; }
    [ProtoMember(4)] public AuthSessionConfirmation[] allowed_confirmations { get; set; } = new AuthSessionConfirmation[0];
    [ProtoMember(5, DataFormat = DataFormat.FixedSize)] public ulong steamid { get; set; }
    [ProtoMember(6)] public string? weak_token { get; set; }
    /// <summary>
    /// if login has been confirmed, may contain remembered machine ID for future login
    /// </summary>
    [ProtoMember(7)] public string new_guard_data { get; set; }
}
