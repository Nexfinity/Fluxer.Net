namespace Fluxer.Net.Admin;

/// <summary>
/// A channel read through a <see cref="FluxerAdminClient"/>.
/// </summary>
public class ManagedChannel : Channel
{
    internal ManagedChannel(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates a channel from its API data.
    /// </summary>
    /// <param name="client">The admin client the channel was read with.</param>
    /// <param name="json">The channel data.</param>
    public static ManagedChannel Create(FluxerAdminClient client, ChannelJson json)
    {
        ManagedChannel data = new ManagedChannel(client);
        data.Update(json);
        return data;
    }
}
