using Newtonsoft.Json;

namespace Fluxer.Net.Rest.Requests;

public class ChannelCreateLinkRequest : ChannelCreateRequest
{
    public override string Type => "GUILD_LINK";

    [JsonRequired]
    [JsonProperty("name")]
    public string Name { get; set; }
}
