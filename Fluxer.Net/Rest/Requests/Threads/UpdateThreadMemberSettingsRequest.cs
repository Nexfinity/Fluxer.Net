using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Changes your notification settings for a thread.
/// </summary>
public class UpdateThreadMemberSettingsRequest
{
    /// <summary>
    /// Notification flags: all messages 2, only mentions 4, no messages 8.
    /// </summary>
    [JsonProperty("flags")]
    public int? Flags { get; set; }

    /// <summary>
    /// Whether the thread is muted.
    /// </summary>
    [JsonProperty("muted")]
    public bool? Muted { get; set; }

    /// <summary>
    /// How long the thread is muted for.
    /// </summary>
    [JsonProperty("mute_config")]
    public ThreadMuteConfigRequest? MuteConfig { get; set; }
}

/// <summary>
/// How long a thread is muted for.
/// </summary>
public class ThreadMuteConfigRequest
{
    /// <summary>
    /// When the mute expires, or null to mute until unmuted.
    /// </summary>
    [JsonProperty("end_time", NullValueHandling = NullValueHandling.Include)]
    public DateTimeOffset? EndTime { get; set; }

    /// <summary>
    /// The selected mute duration.
    /// </summary>
    [JsonProperty("selected_time_window")]
    public int SelectedTimeWindow { get; set; }
}
