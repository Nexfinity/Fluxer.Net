using Fluxer.Net.Data.Models;
using Fluxer.Net.Gateway.Data;
using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Responses;

public class WebhookResponse : Webhook
{
    /// <summary>
    /// User who created the webhook
    /// </summary>
    [JsonPropertyName("user")]
    public UserPartialResponse User { get; set; }
}
