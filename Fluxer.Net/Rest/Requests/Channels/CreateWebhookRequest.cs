using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for creating a webhook in a channel.
/// </summary>
public class CreateWebhookRequest
{
    /// <summary>
    /// The name of the webhook.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// The avatar image of the webhook, or null for none.
    /// </summary>
    [JsonProperty("avatar")]
    public string? Avatar { get; set; }
}
