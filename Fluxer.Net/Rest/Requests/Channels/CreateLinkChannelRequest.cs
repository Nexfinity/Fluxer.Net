using Newtonsoft.Json;

namespace Fluxer.Net;

public class CreateLinkChannelRequest : CreateGuildChannelRequest
{
    public override string Type => "GUILD_LINK";

    [JsonRequired]
    [JsonProperty("name")]
    public string Name { get; set; }
}
