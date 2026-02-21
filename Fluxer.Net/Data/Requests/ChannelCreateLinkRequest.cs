using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Requests;

public class ChannelCreateLinkRequest : ChannelCreateRequest
{
    [JsonRequired]
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
