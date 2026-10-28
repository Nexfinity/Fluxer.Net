using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fluxer.Net;

public class ExperimentAssignmentsJson
{
    /// <summary>
    /// How often Fluxer's clients check their assignments again, in seconds.
    /// </summary>
    [JsonProperty("poll_interval_seconds")]
    public int PollInterval { get; set; }

    /// <summary>
    /// Each experiment's assignment by name, such as channel_threads, whose "active" says whether threads are available.
    /// </summary>
    [JsonProperty("assignments")]
    public Dictionary<string, JObject> Assignments { get; set; } = new Dictionary<string, JObject>();
}
