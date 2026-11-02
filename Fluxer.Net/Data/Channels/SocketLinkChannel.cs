namespace Fluxer.Net;

/// <summary>
/// A cached link channel within a guild.
/// </summary>
public class SocketLinkChannel : LinkChannel, ISocketGuildChannel
{
    /// <inheritdoc />
    public SocketGuild Guild { get; internal set; }

    internal SocketLinkChannel(FluxerBaseClient client) : base(client)
    {

    }

}
