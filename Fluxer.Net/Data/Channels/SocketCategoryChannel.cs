namespace Fluxer.Net;

/// <summary>
/// A cached channel category in a guild.
/// </summary>
public class SocketCategoryChannel : CategoryChannel, ISocketGuildChannel
{
    /// <inheritdoc />
    public SocketGuild Guild { get; internal set; }

    internal SocketCategoryChannel(FluxerBaseClient client) : base(client)
    {

    }
}
