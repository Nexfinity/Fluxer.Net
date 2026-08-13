using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class CreateWebhookRequest
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("avatar")]
    public string? Avatar { get; set; }
}
