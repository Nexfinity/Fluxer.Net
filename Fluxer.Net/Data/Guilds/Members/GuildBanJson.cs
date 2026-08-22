using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class GuildBanJson : IGuildBan
{
    /// <inheritdoc />
    [JsonProperty("banned_at")]
    public DateTimeOffset BannedAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("expires_at")]
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("moderator_id")]
    public ulong ModeratorId { get; set; }

    /// <inheritdoc />
    [JsonProperty("reason")]
    public string? Reason { get; set; }

    /// <inheritdoc />
    [JsonProperty("user")]
    public UserJson User { get; set; }

    IUser IGuildBan.User => User;
}
