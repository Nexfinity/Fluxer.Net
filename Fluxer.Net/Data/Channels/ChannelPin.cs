namespace Fluxer.Net;

/// <inheritdoc />
public class ChannelPin : Entity, IChannelPin
{
    /// <inheritdoc />
    public Message Message { get; internal set; }

    /// <inheritdoc />
    public DateTime PinnedAt { get; internal set; }

    IMessage IChannelPin.Message => Message;

    internal ChannelPin(FluxerBaseClient client) : base(client)
    {

    }

    public static ChannelPin Create(FluxerBaseClient client, ChannelPinJson json)
    {
        ChannelPin data = new ChannelPin(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, ChannelPinJson json)
    {
        Message = Message.Create(client, json.Message);
        PinnedAt = json.PinnedAt;
    }
}
