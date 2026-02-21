using Fluxer.Net.Gateway.Data;
using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Responses;

public class GuildBanResponse
{
    /// <summary>
    /// When the member was banned.
    /// </summary>
    [JsonPropertyName("banned_at")]
    public DateTime BannedAt { get; set; }

    /// <summary>
    /// When the ban expires (<see langword="null"/> for never)
    /// </summary>
    [JsonPropertyName("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Id of the user who issues the ban.
    /// </summary>
    [JsonPropertyName("moderator_id")]
    public ulong ModeratorId { get; set; }

    /// <summary>
    /// Ban Reason (max 512 characters)
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
    
    [JsonPropertyName("user")]
    public UserPartialResponse User { get; set; }
}
