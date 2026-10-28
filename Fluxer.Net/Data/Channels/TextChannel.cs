namespace Fluxer.Net;

public class TextChannel : Channel, ITextChannel
{
    public virtual ulong? CategoryId => ParentId;

    internal TextChannel(FluxerBaseClient client) : base(client)
    {

    }
}
