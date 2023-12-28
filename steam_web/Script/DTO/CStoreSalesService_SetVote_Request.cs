using ProtoBuf;

namespace SteamWeb.Script.DTO;

[ProtoContract]
public class CStoreSalesService_SetVote_Request
{
    [ProtoMember(1)]
    public int voteid { get; set; }
    [ProtoMember(2)]
    public int appid { get; set; }
    [ProtoMember(3)]
    public int sale_appid { get; set; }
}
