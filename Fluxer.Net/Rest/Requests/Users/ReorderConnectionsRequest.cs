using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Sets the display order of your connections.
/// </summary>
public class ReorderConnectionsRequest
{
    /// <summary>
    /// The connection ids in their new order, between 1 and 20.
    /// </summary>
    [JsonProperty("connection_ids")]
    public IEnumerable<string> ConnectionIds { get; set; }
}
