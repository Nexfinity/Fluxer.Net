using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class CreateGuildBanRequest
{
    /// <summary>
    /// Duration of the ban in seconds
    /// (0 or null for permanent, or anything greater than zero for it to be temporary)
    /// </summary>
    [JsonProperty("ban_duration_seconds")]
    public int? BanDurationSeconds { get; set; }

    /// <summary>
    /// Number of days of messages to delete from the banned user (0-7)
    /// </summary>
    [JsonProperty("delete_message_days")]
    public int? DeleteMessageDays { get; set; }

    /// <summary>
    /// The reason for the ban (max 512 characters)
    /// </summary>
    [JsonProperty("reason")]
    public string? Reason { get; set; }
}
