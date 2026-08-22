using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class ChannelPinJson : IChannelPin
{
    /// <inheritdoc />
    [JsonProperty("message")]
    public MessageJson Message { get; set; }

    /// <inheritdoc />
    [JsonProperty("pinned_at")]
    public DateTimeOffset PinnedAt { get; set; }

    IMessage IChannelPin.Message => Message;
}
