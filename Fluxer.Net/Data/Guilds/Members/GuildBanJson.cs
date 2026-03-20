using Fluxer.Net.Gateway.Data;
using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class GuildBanJson : IGuildBan
{
    /// <inheritdoc />
    [JsonProperty("banned_at")]
    public DateTime BannedAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("moderator_id")]
    public ulong ModeratorId { get; set; }

    /// <inheritdoc />
    [JsonProperty("reason")]
    public string? Reason { get; set; }

    /// <inheritdoc />
    [JsonProperty("user")]
    public UserPartialResponse User { get; set; }
}
