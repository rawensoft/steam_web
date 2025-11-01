using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO;

public class CommentResponse
{
    [JsonPropertyName("success")]
    [MemberNotNullWhen(true, [nameof(Name), nameof(CommentsHtml)])]
    public bool Success { get; init; } = false;

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("start")]
    public uint Start { get; init; }

    [JsonPropertyName("pagesize")]
    public byte PageSize { get; init; }

    [JsonPropertyName("total_count")]
    public uint TotalCount { get; init; }

    [JsonPropertyName("upvotes")]
    public int UpVotes { get; init; }

    [JsonPropertyName("has_upvoted")]
    public sbyte HasUpvoted { get; init; }

    [JsonPropertyName("comments_html")]
    public string? CommentsHtml { get; init; }

    [JsonPropertyName("timelastpost")]
    public int TimeLastPost { get; init; }
}
