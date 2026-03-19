using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

namespace Fluxer.Net;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/c2b69be17d1877c5bb82d10c77fa67cbe4e882d7/packages/schema/src/domains/guild/GuildAuditLogSchemas.tsx#L46"/>
/// </remarks>
public class AuditLogResponseItemChangeJson
{
    [JsonRequired]
    [JsonProperty("key")]
    public string Key { get; set; }

    [JsonProperty("old_value")]
    public object? OldValue { get; set; }

    [JsonProperty("new_value")]
    public object? NewValue { get; set; }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        if (!Key.Equals("permissions_diff", StringComparison.OrdinalIgnoreCase)) return;

        if (NewValue is JObject newValueObj)
        {
            NewValue = newValueObj.ToObject<PermissionDiffSchemaJson>();
        }
        if (OldValue is JObject oldValueObj)
        {
            OldValue = oldValueObj.ToObject<PermissionDiffSchemaJson>();
        }
    }
}

public class PermissionDiffSchemaJson
{
    [JsonProperty("added")]
    public HashSet<string> Added { get; set; }

    [JsonProperty("removed")]
    public HashSet<string> Removed { get; set; }
}
