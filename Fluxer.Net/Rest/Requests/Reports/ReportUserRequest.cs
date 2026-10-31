using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Reports a user.
/// </summary>
public class ReportUserRequest
{
    /// <summary>
    /// The user being reported.
    /// </summary>
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    /// <summary>
    /// One of harassment, hate_speech, spam_account, impersonation, underage_user, inappropriate_profile or other.
    /// </summary>
    [JsonProperty("category")]
    public string Category { get; set; }

    /// <summary>
    /// The guild where the violation happened.
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong? GuildId { get; set; }
}
