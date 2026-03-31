namespace Fluxer.Net;

public class TextChannel : GuildChannel, ITextable
{
    internal TextChannel(FluxerBaseClient client) : base(client)
    {

    }
}
