using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class RecentMentionDeleteGatewayData
{
    [JsonProperty("message_id")]
    public ulong MessageId { get; set; }
}