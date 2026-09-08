using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// Gateway data for SAVED_MESSAGE_DELETE event.
/// </summary>
public class SavedMessageDeletedGatewayData
{
    [JsonProperty("message_id")]
    public ulong MessageId { get; set; }
}
