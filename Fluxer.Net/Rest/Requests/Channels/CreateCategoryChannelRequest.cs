using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class CreateCategoryChannelRequest : CreateGuildChannelRequest
{
    public override string Type => "GUILD_CATEGORY";

    [JsonRequired]
    [JsonProperty("name")]
    public string Name { get; set; }
}
