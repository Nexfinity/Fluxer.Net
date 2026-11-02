namespace Fluxer.Net;

/// <inheritdoc />
public class ChannelPin : Entity, IChannelPin
{
    /// <inheritdoc />
    public Message Message { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset PinnedAt { get; private set; }

    IMessage IChannelPin.Message => Message;

    internal ChannelPin(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates a channel pin from JSON.
    /// </summary>
    public static ChannelPin Create(FluxerBaseClient client, ChannelPinJson json)
    {
        ChannelPin data = new ChannelPin(client);
        data.Update(json);
        return data;
    }

    internal void Update(ChannelPinJson json)
    {
        Message = Message.Create(Client, json.Message);
        PinnedAt = json.PinnedAt;
    }
}
