using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class CreateLinkChannelRequest : CreateGuildChannelRequest
{
    public override GuildChannelType Type => GuildChannelType.GuildLink;

    [JsonProperty("url")]
    public string? Url { get; set; }
}
