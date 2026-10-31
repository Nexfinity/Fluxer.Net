using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Reports a guild.
/// </summary>
public class ReportGuildRequest
{
    /// <summary>
    /// The guild being reported.
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    /// <summary>
    /// One of harassment, hate_speech, extremist_community, illegal_activity, child_safety, raid_coordination, spam, malware_distribution or other.
    /// </summary>
    [JsonProperty("category")]
    public string Category { get; set; }

    /// <summary>
    /// An invite code proving access, needed when you are not in a guild that is not in discovery.
    /// </summary>
    [JsonProperty("invite_code")]
    public string? InviteCode { get; set; }
}
