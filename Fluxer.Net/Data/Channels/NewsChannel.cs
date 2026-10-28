namespace Fluxer.Net;

public class NewsChannel : TextChannel, INewsChannel
{
    internal NewsChannel(FluxerBaseClient client) : base(client)
    {

    }
}
