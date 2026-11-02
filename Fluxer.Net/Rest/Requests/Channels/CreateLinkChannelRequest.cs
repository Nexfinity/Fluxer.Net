using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for creating a link channel in a guild.
/// </summary>
public class CreateLinkChannelRequest : CreateGuildChannelRequest
{
    /// <summary>
    /// The channel type, always a link channel.
    /// </summary>
    public override GuildChannelType Type => GuildChannelType.GuildLink;

    /// <summary>
    /// The URL the link channel points to.
    /// </summary>
    [JsonProperty("url")]
    public string? Url { get; set; }
}
