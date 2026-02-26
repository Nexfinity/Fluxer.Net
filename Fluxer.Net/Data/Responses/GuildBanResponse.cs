using Fluxer.Net.Gateway.Data;
using Newtonsoft.Json;

namespace Fluxer.Net.Data.Responses;

public class GuildBanResponse
{
    /// <summary>
    /// When the member was banned.
    /// </summary>
    [JsonProperty("banned_at")]
    public DateTime BannedAt { get; set; }

    /// <summary>
    /// When the ban expires (<see langword="null"/> for never)
    /// </summary>
    [JsonProperty("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Id of the user who issues the ban.
    /// </summary>
    [JsonProperty("moderator_id")]
    public ulong ModeratorId { get; set; }

    /// <summary>
    /// Ban Reason (max 512 characters)
    /// </summary>
    [JsonProperty("reason")]
    public string? Reason { get; set; }
    
    [JsonProperty("user")]
    public UserPartialResponse User { get; set; }
}
