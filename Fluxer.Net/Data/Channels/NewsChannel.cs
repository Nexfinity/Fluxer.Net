namespace Fluxer.Net;

/// <summary>
/// A guild announcement channel.
/// </summary>
public class NewsChannel : TextChannel, INewsChannel
{
    internal NewsChannel(FluxerBaseClient client) : base(client)
    {

    }
}
