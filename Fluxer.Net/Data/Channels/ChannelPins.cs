namespace Fluxer.Net;

/// <inheritdoc />
public class ChannelPins : Entity, IChannelPins
{
    /// <inheritdoc />
    public IEnumerable<ChannelPin> Items { get; private set; }

    /// <inheritdoc />
    public bool HasMore { get; private set; }

    IEnumerable<IChannelPin> IChannelPins.Items => Items;

    internal ChannelPins(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a ChannelPins object from json.
    /// </summary>
    /// <returns><see cref="ChannelPins"/></returns>
    public static ChannelPins Create(FluxerBaseClient client, ChannelPinsJson json)
    {
        ChannelPins data = new ChannelPins(client);
        data.Update(json);
        return data;
    }

    internal void Update(ChannelPinsJson json)
    {
        Items = json.Items.Select(x => ChannelPin.Create(Client, x));
        HasMore = json.HasMore;
    }
}
