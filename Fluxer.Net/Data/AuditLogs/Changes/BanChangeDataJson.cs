using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class BanChangeDataJson : IAuditLogData
{
    /// <summary>
    /// The id of the banned user.
    /// </summary>
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    /// <summary>
    /// The id of the moderator that issued the ban.
    /// </summary>
    [JsonProperty("moderator_id")]
    public ulong ModeratorId { get; set; }

    /// <summary>
    /// When the ban was issued.
    /// </summary>
    [JsonProperty("banned_at")]
    public DateTimeOffset? BannedAt { get; set; }

    /// <summary>
    /// When the ban expires.
    /// </summary>
    [JsonProperty("expires_at")]
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>
    /// The reason given for the ban.
    /// </summary>
    [JsonProperty("reason")]
    public string? Reason { get; set; }
}
