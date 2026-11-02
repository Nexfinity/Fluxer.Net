namespace Fluxer.Net;

/// <summary>
/// A thread member cached with its guild and thread.
/// </summary>
public class SocketThreadMember : ThreadMember
{
    /// <summary>
    /// The guild the thread is in.
    /// </summary>
    public SocketGuild Guild { get; private set; }

    /// <summary>
    /// The thread the member is in.
    /// </summary>
    public ThreadChannel Channel { get; private set; }

    internal SocketThreadMember(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates a cached thread member from its JSON representation.
    /// </summary>
    public static SocketThreadMember Create(FluxerBaseClient client, ThreadMemberJson json, ThreadChannel channel, SocketGuild guild)
    {
        SocketThreadMember data = new SocketThreadMember(client)
        {
            Channel = channel,
            Guild = guild
        };
        data.Update(json);
        return data;
    }

    internal void Update(ThreadMemberJson json)
    {
        base.Update(json);
    }
}
