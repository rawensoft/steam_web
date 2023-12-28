using ProtoBuf;

namespace SteamWeb.Auth.v2.DTO;

[ProtoContract]
public class CAuthentication_UpdateAuthSessionWithMobileConfirmation_Request
{
    [ProtoMember(1)]
    public short version { get; set; }
    [ProtoMember(2)]
    public ulong client_id { get; set; }
    [ProtoMember(3, DataFormat = DataFormat.FixedSize)]
    public ulong steamid { get; set; }
    [ProtoMember(4)]
    public byte[] signature { get; set; }
    [ProtoMember(5)]
    public bool confirm { get; set; }
    [ProtoMember(6)]
    public ESessionPersistence persistence { get; set; }
}