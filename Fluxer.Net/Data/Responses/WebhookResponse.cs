using Fluxer.Net.Data.Models;
using Fluxer.Net.Gateway.Data;
using Newtonsoft.Json;

namespace Fluxer.Net.Data.Responses;

public class WebhookResponse : Webhook
{
    /// <summary>
    /// User who created the webhook
    /// </summary>
    [JsonProperty("user")]
    public UserPartialResponse User { get; set; }
}
