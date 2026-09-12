using Newtonsoft.Json;

namespace Fluxer.Net;

public class BanChangeDataJson : IAuditLogData
{
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("moderator_id")]
    public ulong ModeratorId { get; set; }

    [JsonProperty("banned_at")]
    public DateTimeOffset? BannedAt { get; set; }

    [JsonProperty("expires_at")]
    public DateTimeOffset? ExpiresAt { get; set; }

    [JsonProperty("reason")]
    public string? Reason { get; set; }
}
