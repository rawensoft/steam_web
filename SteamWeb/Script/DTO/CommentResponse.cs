namespace SteamWeb.Script.DTO;

public record CommentResponse
{
    public bool success { get; init; } = false;
    public string name { get; init; }
    public int start { get; init; }
    public string pagesize { get; init; }
    public int total_count { get; init; }
    public int upvotes { get; init; }
    public int has_upvoted { get; init; }
    public string comments_html { get; init; }
    public int timelastpost { get; init; }
}
