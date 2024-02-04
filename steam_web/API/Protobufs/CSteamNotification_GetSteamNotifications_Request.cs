using ProtoBuf;

namespace SteamWeb.API.Protobufs;

[ProtoContract]
public class CSteamNotification_GetSteamNotifications_Request
{
    [ProtoMember(1)]
    public bool include_hidden { get; set; } = false;
    [ProtoMember(2)]
    public int language { get; set; } = 0;
    [ProtoMember(3)]
    public bool include_confirmation_count { get; set; } = true;
    [ProtoMember(4)]
    public bool include_pinned_counts { get; set; } = false;
    [ProtoMember(5)]
    public bool include_read { get; set; } = true;
    [ProtoMember(6)]
    public bool count_only { get; set; } = false;
}
