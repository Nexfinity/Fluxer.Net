using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// The answers of a finished user report flow.
/// </summary>
public class ReportFlowUserSubmissionRequest : ReportFlowSubmissionRequest
{
    /// <summary>
    /// The user being reported.
    /// </summary>
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    /// <summary>
    /// The guild where the violation happened.
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }
}
