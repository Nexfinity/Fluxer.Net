namespace Fluxer.Net;

/// <summary>
/// A cached media channel in a guild.
/// </summary>
public class SocketMediaChannel : MediaChannel, ISocketGuildChannel
{
    /// <inheritdoc />
    public SocketGuild Guild { get; internal set; }

    internal SocketMediaChannel(FluxerBaseClient client) : base(client)
    {

    }

}