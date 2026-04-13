using Newtonsoft.Json;

namespace Fluxer.Net;

public class MuteConfigurationJson
{
    [JsonProperty("end_time")]
    public DateTime? EndAt { get; set; }

    [JsonProperty("selected_time_window")]
    public int? SelectedTimeSeconds { get; set; }
}
