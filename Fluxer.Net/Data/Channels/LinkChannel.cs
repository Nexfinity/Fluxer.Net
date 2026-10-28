namespace Fluxer.Net;

public class LinkChannel : Channel, INestedChannel
{
    public ulong? CategoryId => ParentId;

    internal LinkChannel(FluxerBaseClient client) : base(client)
    {

    }
}