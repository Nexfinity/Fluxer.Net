using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class GuildMemberSearchRequest
{
    /// <summary>
    /// Text to look for in usernames, global names and nicknames, up to 100 characters.
    /// </summary>
    [JsonProperty("query")]
    public string? Query { get; set; }

    /// <summary>
    /// The number of members to return, from 1 to 100.
    /// </summary>
    [JsonProperty("limit")]
    public int? Limit { get; set; }

    [JsonProperty("offset")]
    public int? Offset { get; set; }

    /// <summary>
    /// Roles a member must all have.
    /// </summary>
    [JsonProperty("role_ids")]
    public IEnumerable<ulong>? RoleIds { get; set; }

    /// <summary>
    /// Only members who joined at or after this unix time, in seconds.
    /// </summary>
    [JsonProperty("joined_at_gte")]
    public long? JoinedAfter { get; set; }

    /// <summary>
    /// Only members who joined at or before this unix time, in seconds.
    /// </summary>
    [JsonProperty("joined_at_lte")]
    public long? JoinedBefore { get; set; }

    /// <summary>
    /// Only members who joined with one of these invite codes.
    /// </summary>
    [JsonProperty("source_invite_code")]
    public IEnumerable<string>? InviteCodes { get; set; }

    [JsonProperty("is_bot")]
    public bool? IsBot { get; set; }

    /// <summary>
    /// Only users whose account was created at or after this unix time, in seconds.
    /// </summary>
    [JsonProperty("user_created_at_gte")]
    public long? CreatedAfter { get; set; }

    /// <summary>
    /// Only users whose account was created at or before this unix time, in seconds.
    /// </summary>
    [JsonProperty("user_created_at_lte")]
    public long? CreatedBefore { get; set; }

    /// <summary>
    /// "joinedAt" or "relevance".
    /// </summary>
    [JsonProperty("sort_by")]
    public string? SortBy { get; set; }

    /// <summary>
    /// "asc" or "desc".
    /// </summary>
    [JsonProperty("sort_order")]
    public string? SortOrder { get; set; }
}
