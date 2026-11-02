namespace Fluxer.Net;

/// <summary>
/// A cached forum channel in a guild.
/// </summary>
public class SocketForumChannel : ForumChannel, ISocketGuildChannel
{
    /// <inheritdoc />
    public SocketGuild Guild { get; internal set; }

    internal SocketForumChannel(FluxerBaseClient client) : base(client)
    {

    }

}
