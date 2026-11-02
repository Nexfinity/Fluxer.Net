using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fluxer.Net;

/// <summary>
/// A changed field of an audit log entry, with its values before and after the change.
/// </summary>
public class AuditLogChangeJson
{
    /// <summary>
    /// The name of the changed field.
    /// </summary>
    [JsonProperty("key")]
    public string Key { get; set; }

    /// <summary>
    /// The value before the change.
    /// </summary>
    [JsonProperty("old_value")]
    public JToken? OldValue { get; set; }

    /// <summary>
    /// The value after the change.
    /// </summary>
    [JsonProperty("new_value")]
    public JToken? NewValue { get; set; }
}