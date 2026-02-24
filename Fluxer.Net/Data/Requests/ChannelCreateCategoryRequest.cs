using Newtonsoft.Json;

namespace Fluxer.Net.Data.Requests;

public class ChannelCreateCategoryRequest : ChannelCreateRequest
{
    public override string Type => "GUILD_CATEGORY";

    [JsonRequired]
    [JsonProperty("name")]
    public string Name { get; set; }
}
