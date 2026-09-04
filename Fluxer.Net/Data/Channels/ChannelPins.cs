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
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static ChannelPins Create(FluxerBaseClient client, ChannelPinsJson json)
    {
        ChannelPins data = new ChannelPins(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, ChannelPinsJson json)
    {
        Items = json.Items.Select(x => ChannelPin.Create(client, x));
        HasMore = json.HasMore;
    }
}
