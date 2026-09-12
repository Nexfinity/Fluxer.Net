using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

namespace Fluxer.Net;

public class AuditLogChangeJson
{
    [JsonProperty("key")]
    public string Key { get; set; }

    [JsonProperty("old_value")]
    public JToken? OldValue { get; set; }

    [JsonProperty("new_value")]
    public JToken? NewValue { get; set; }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        if (!Key.Equals("permissions_diff", StringComparison.OrdinalIgnoreCase)) return;

    }
}

public class PermissionDiffSchemaJson
{
    [JsonProperty("added")]
    public HashSet<string> Added { get; set; }

    [JsonProperty("removed")]
    public HashSet<string> Removed { get; set; }
}
