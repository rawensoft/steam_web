using ProtoBuf;

namespace SteamWeb.Script.DTO;

[ProtoContract]
public class CStoreSalesService_SetVote_Response
{
    [ProtoMember(1)]
    public CStoreSalesService_SetVote[] votes { get; set; } = new CStoreSalesService_SetVote[0];
}
