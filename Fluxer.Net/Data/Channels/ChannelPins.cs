using Fluxer.Net.Data.Channels;

namespace Fluxer.Net;

/// <inheritdoc />
public class ChannelPins : Entity, IChannelPins
{
    /// <inheritdoc />
    public IEnumerable<ChannelPin> Items { get; internal set; }

    /// <inheritdoc />
    public bool HasMore { get; internal set; }

    IEnumerable<IChannelPin> IChannelPins.Items => Items;

    internal ChannelPins(BaseClient client) : base(client)
    {

    }

    public static ChannelPins Create(BaseClient client, ChannelPinsJson json)
    {
        var data = new ChannelPins(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(BaseClient client, ChannelPinsJson json)
    {
        Items = json.Items.Select(x => ChannelPin.Create(client, x));
        HasMore = json.HasMore;
    }
}
