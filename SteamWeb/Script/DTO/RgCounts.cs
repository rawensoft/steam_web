namespace SteamWeb.Script.DTO;

public class RgCounts
{
    public int cFollowing { get; init; } = 0;
    public int cFriends { get; init; } = 0;
    public int cFriendsBlocked { get; init; } = 0;
    public int cFriendsPending { get; init; } = 0;
    public int cGroups { get; init; } = 0;
    public int cGroupsPending { get; init; } = 0;
    public int success { get; init; } = 0;
}
