using ProtoBuf;

namespace SteamWeb.Script.DTO;

[ProtoContract]
public class CStoreSalesService_SetVote
{
    [ProtoMember(1)]
    public int voteid { get; set; }
    [ProtoMember(2)]
    public int appid { get; set; }
    [ProtoMember(3)]
    public long data { get; set; }
}