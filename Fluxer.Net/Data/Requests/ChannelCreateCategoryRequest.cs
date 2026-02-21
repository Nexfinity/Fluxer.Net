using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Requests;

public class ChannelCreateCategoryRequest : ChannelCreateRequest
{
    [JsonRequired]
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
