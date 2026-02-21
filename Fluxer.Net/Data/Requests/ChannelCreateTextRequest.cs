using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Requests;

public class ChannelCreateTextRequest : ChannelCreateRequest
{
    [JsonRequired]
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
