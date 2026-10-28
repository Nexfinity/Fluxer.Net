using Newtonsoft.Json;

namespace Fluxer.Net;

public class GuildMemberSearchJson
{
    [JsonProperty("members")]
    public GuildMemberSearchResultJson[] Members { get; set; } = Array.Empty<GuildMemberSearchResultJson>();

    [JsonProperty("page_result_count")]
    public int PageResultCount { get; set; }

    [JsonProperty("total_result_count")]
    public int TotalResultCount { get; set; }

    /// <summary>
    /// Whether Fluxer is still indexing the guild's members, so results may be missing.
    /// </summary>
    [JsonProperty("indexing")]
    public bool IsIndexing { get; set; }
}

public class GuildMemberSearchResultJson
{
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("discriminator")]
    public string Discriminator { get; set; }

    [JsonProperty("global_name")]
    public string? GlobalName { get; set; }

    [JsonProperty("nickname")]
    public string? Nickname { get; set; }

    [JsonProperty("role_ids")]
    public ulong[] RoleIds { get; set; } = Array.Empty<ulong>();

    /// <summary>
    /// When the member joined, in unix seconds.
    /// </summary>
    [JsonProperty("joined_at")]
    public long JoinedAt { get; set; }

    [JsonProperty("is_bot")]
    public bool IsBot { get; set; }
}
