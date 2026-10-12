using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class SavedMessageDeletedGatewayData
{
    [JsonProperty("message_id")]
    public ulong MessageId { get; set; }
}