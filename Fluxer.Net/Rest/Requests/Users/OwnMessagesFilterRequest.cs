using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Narrows which of your own messages an operation covers.
/// </summary>
public class OwnMessagesFilterRequest
{
    /// <summary>
    /// "selected", or "inaccessible_only" for places you can no longer access.
    /// </summary>
    [JsonProperty("scope")]
    public string? Scope { get; set; }

    /// <summary>
    /// Include direct messages you still have open.
    /// </summary>
    [JsonProperty("include_dms")]
    public bool? IncludeDms { get; set; }

    /// <summary>
    /// Include direct messages you closed.
    /// </summary>
    [JsonProperty("include_dms_closed")]
    public bool? IncludeClosedDms { get; set; }

    /// <summary>
    /// Include group DMs you are still in.
    /// </summary>
    [JsonProperty("include_group_dms")]
    public bool? IncludeGroupDms { get; set; }

    /// <summary>
    /// Include text channels in guilds you are in.
    /// </summary>
    [JsonProperty("include_guilds")]
    public bool? IncludeGuilds { get; set; }

    /// <summary>
    /// "exclude" to cover every guild except <see cref="ExcludedGuildIds"/>, or "include_only" to cover only <see cref="IncludedGuildIds"/>.
    /// </summary>
    [JsonProperty("guild_filter_mode")]
    public string? GuildFilterMode { get; set; }

    /// <summary>
    /// Guilds to leave untouched, up to 500.
    /// </summary>
    [JsonProperty("excluded_guild_ids")]
    public IEnumerable<ulong>? ExcludedGuildIds { get; set; }

    /// <summary>
    /// The only guilds to cover, up to 500.
    /// </summary>
    [JsonProperty("included_guild_ids")]
    public IEnumerable<ulong>? IncludedGuildIds { get; set; }

    /// <summary>
    /// Inclusive lower bound for message timestamps.
    /// </summary>
    [JsonProperty("start_date")]
    public DateTimeOffset? StartDate { get; set; }

    /// <summary>
    /// Exclusive upper bound for message timestamps.
    /// </summary>
    [JsonProperty("end_date")]
    public DateTimeOffset? EndDate { get; set; }
}
