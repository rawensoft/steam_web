using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IFriendMessagesService;
public class RecentMessageRequest
{
    public ulong SteamId1 { get; }
    public ulong SteamId2 { get; }
    /// <summary>
    /// If non-zero, cap the number of recent messages to return.
    /// </summary>
    public uint? Count { get; set; }
    /// <summary>
    /// Grab the block of chat from the most recent conversation (a ~5 minute period)
    /// </summary>
    public bool MostRecentConversation { get; set; } = false;
    /// <summary>
    /// If non-zero, return only messages with timestamps greater or equal to this. If zero, we only return messages from a recent time cutoff.
    /// </summary>
    public int? RecentTime32StartTime { get; set; }
    /// <summary>
    /// Return the results with bbcode formatting.
    /// </summary>
    public bool BbCodeFormat { get; set; } = false;
    /// <summary>
    /// Combined with start time, only messages after this ordinal are returned (dedupes messages in same second)
    /// </summary>
    public int? StartOrdinal { get; set; }
    /// <summary>
    /// if present/non-zero, return only messages before this.
    /// </summary>
    public int? TimeLast { get; set; }
    public int? OrdinalLast { get; set; }

    [JsonConstructor]
    public RecentMessageRequest(ulong steamid1, ulong steamid2)
    {
        SteamId1 = steamid1;
        SteamId2 = steamid2;
    }
}