using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// The response of a guild member search.
/// </summary>
public class GuildMemberSearchJson
{
    /// <summary>
    /// The members on the requested page.
    /// </summary>
    [JsonProperty("members")]
    public GuildMemberSearchResultJson[] Members { get; set; } = Array.Empty<GuildMemberSearchResultJson>();

    /// <summary>
    /// The number of members on the requested page.
    /// </summary>
    [JsonProperty("page_result_count")]
    public int PageResultCount { get; set; }

    /// <summary>
    /// The total number of members matching the search.
    /// </summary>
    [JsonProperty("total_result_count")]
    public int TotalResultCount { get; set; }

    /// <summary>
    /// Whether Fluxer is still indexing the guild's members, so results may be missing.
    /// </summary>
    [JsonProperty("indexing")]
    public bool IsIndexing { get; set; }
}

/// <summary>
/// A member in a guild member search result.
/// </summary>
public class GuildMemberSearchResultJson
{
    /// <summary>
    /// The ID of the user.
    /// </summary>
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    /// <summary>
    /// The username of the user.
    /// </summary>
    [JsonProperty("username")]
    public string Username { get; set; }

    /// <summary>
    /// The discriminator of the user.
    /// </summary>
    [JsonProperty("discriminator")]
    public string Discriminator { get; set; }

    /// <summary>
    /// The global display name of the user.
    /// </summary>
    [JsonProperty("global_name")]
    public string? GlobalName { get; set; }

    /// <summary>
    /// The nickname of the member in the guild.
    /// </summary>
    [JsonProperty("nickname")]
    public string? Nickname { get; set; }

    /// <summary>
    /// The IDs of the roles the member has.
    /// </summary>
    [JsonProperty("role_ids")]
    public ulong[] RoleIds { get; set; } = Array.Empty<ulong>();

    /// <summary>
    /// When the member joined, in unix seconds.
    /// </summary>
    [JsonProperty("joined_at")]
    public long JoinedAt { get; set; }

    /// <summary>
    /// Whether the user is a bot.
    /// </summary>
    [JsonProperty("is_bot")]
    public bool IsBot { get; set; }
}
