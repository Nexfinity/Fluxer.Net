using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fluxer.Net;

public class AuditLogChangeJson
{
    [JsonProperty("key")]
    public string Key { get; set; }

    [JsonProperty("old_value")]
    public JToken? OldValue { get; set; }

    [JsonProperty("new_value")]
    public JToken? NewValue { get; set; }
}