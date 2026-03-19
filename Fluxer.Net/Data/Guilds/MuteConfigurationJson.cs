using Newtonsoft.Json;

namespace Fluxer.Net;

public class MuteConfigurationJson
{
    [JsonProperty("end_time")]
    public DateTime? EndTime { get; set; }

    [JsonProperty("selected_time_window")]
    public int? SelectedTimeWindow { get; set; }
}
