using Newtonsoft.Json.Linq;

namespace Fluxer.Net;

public class AuditRawDataJson
{
    public JObject OldData { get; internal set; }
    public JObject NewData { get; internal set; }
}
