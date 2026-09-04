using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class MuteConfigurationJson : IMuteConfiguration
{
    /// <inheritdoc />
    [JsonProperty("end_time")]
    public DateTimeOffset? EndAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("selected_time_window")]
    public int? SelectedTimeSeconds { get; set; }
}
