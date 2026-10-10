namespace Fluxer.Net;

public class SocketThreadMember : ThreadMember
{
    public SocketGuild Guild { get; private set; }

    public ThreadChannel Channel { get; private set; }

    internal SocketThreadMember(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a ThreadMember object from json.
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
