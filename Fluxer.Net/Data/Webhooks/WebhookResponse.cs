using Fluxer.Net.Gateway.Data;
using Newtonsoft.Json;

namespace Fluxer.Net;

public class WebhookResponse : Webhook
{
    /// <summary>
    /// User who created the webhook
    /// </summary>
    [JsonProperty("user")]
    public UserPartialResponse User { get; set; }
}
