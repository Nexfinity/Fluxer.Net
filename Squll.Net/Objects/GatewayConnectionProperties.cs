using Newtonsoft.Json;

namespace Squll.Net.Objects;

public class GatewayConnectionProperties
{
    [JsonProperty("ignored_events")]
    public string[] IgnoredEvents { get; set; }
}
