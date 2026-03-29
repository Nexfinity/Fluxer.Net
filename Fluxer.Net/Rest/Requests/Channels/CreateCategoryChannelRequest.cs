using Newtonsoft.Json;

namespace Fluxer.Net;

public class CreateCategoryChannelRequest : CreateChannelRequest
{
    public override string Type => "GUILD_CATEGORY";

    [JsonRequired]
    [JsonProperty("name")]
    public string Name { get; set; }
}
