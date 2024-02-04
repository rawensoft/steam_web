using ProtoBuf;

namespace SteamWeb.API.Protobufs;

[ProtoContract]
public class CSteamNotification_GetSteamNotifications_Response
{
    [ProtoMember(1)]
    public SteamNotificationData? notifications { get; set; } = null;
    [ProtoMember(2)]
    public int confirmation_count { get; set; } = 0;
    [ProtoMember(3)]
    public uint pending_gift_count { get; set; }
    [ProtoMember(4)]
    public uint unnamed_field { get; set; }
    [ProtoMember(5)]
    public uint pending_friend_count { get; set; }
    [ProtoMember(6)]
    public uint unread_count { get; set; }
    [ProtoMember(7)]
    public uint pending_family_invite_count { get; set; }
}
