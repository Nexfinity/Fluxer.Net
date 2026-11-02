namespace Fluxer.Net;

/// <summary>
/// A cached guild text channel.
/// </summary>
public class SocketTextChannel : TextChannel, ISocketGuildChannel
{
    /// <inheritdoc />
    public SocketGuild Guild { get; internal set; }

    internal SocketTextChannel(FluxerBaseClient client) : base(client)
    {

    }
}
