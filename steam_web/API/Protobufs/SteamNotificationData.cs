using ProtoBuf;

namespace SteamWeb.API.Protobufs;

[ProtoContract]
public class SteamNotificationData
{
    [ProtoMember(1)]
    public ulong notification_id { get; set; }
    [ProtoMember(2)]
    public uint notification_targets { get; set; }
    /// <summary>
    /// enum
    /// </summary>
    [ProtoMember(3)]
    public int notification_type { get; set; }
    [ProtoMember(4)]
    public string body_data { get; set; } = string.Empty;
    [ProtoMember(7)]
    public bool read { get; set; } = false;
    [ProtoMember(8)]
    public int timestamp { get; set; }
    [ProtoMember(9)]
    public bool hidden { get; set; } = false;
    [ProtoMember(10)]
    public int expiry { get; set; }
    [ProtoMember(11)]
    public int viewed { get; set; }
}