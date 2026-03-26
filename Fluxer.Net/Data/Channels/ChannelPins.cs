namespace Fluxer.Net;

/// <inheritdoc />
public class ChannelPins : Entity, IChannelPins
{
    /// <inheritdoc />
    public IEnumerable<ChannelPin> Items { get; internal set; }

    /// <inheritdoc />
    public bool HasMore { get; internal set; }

    IEnumerable<IChannelPin> IChannelPins.Items => Items;

    internal ChannelPins(FluxerBaseClient client) : base(client)
    {

    }

    public static ChannelPins Create(FluxerBaseClient client, ChannelPinsJson json)
    {
        var data = new ChannelPins(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, ChannelPinsJson json)
    {
        Items = json.Items.Select(x => ChannelPin.Create(client, x));
        HasMore = json.HasMore;
    }
}
