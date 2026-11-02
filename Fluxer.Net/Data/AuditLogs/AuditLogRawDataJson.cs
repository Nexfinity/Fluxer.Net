using Newtonsoft.Json.Linq;

namespace Fluxer.Net;

/// <summary>
/// The raw old and new values of an audit log entry, keyed by field name.
/// </summary>
public class AuditLogRawDataJson
{
    /// <summary>
    /// The values before the change.
    /// </summary>
    public JObject OldData { get; internal set; }

    /// <summary>
    /// The values after the change.
    /// </summary>
    public JObject NewData { get; internal set; }
}
