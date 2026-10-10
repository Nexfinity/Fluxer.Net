namespace Fluxer.Net;

public class ThreadMember : Entity, IThreadMember
{
    /// <inheritdoc />
    public ulong Id { get; private set; }

    /// <inheritdoc />
    public ulong ThreadId { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset JoinedAt { get; private set; }

    /// <inheritdoc />
    public int Flags { get; private set; }

    /// <inheritdoc />
    public GuildMember? Member { get; private set; }

    IGuildMember? IThreadMember.Member => Member;

    internal ThreadMember(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a ThreadMember object from json.
    /// </summary>
    /// <returns><see cref="ThreadMember"/></returns>
    public static ThreadMember Create(FluxerBaseClient client, ThreadMemberJson json)
    {
        ThreadMember data = new ThreadMember(client);
        data.Update(json);
        return data;
    }

    internal void Update(ThreadMemberJson json)
    {
        Id = json.Id;
        ThreadId = json.ThreadId;
        JoinedAt = json.JoinedAt;
        Flags = json.Flags;
        if (json.Member != null)
            Member = GuildMember.Create(Client, json.Member);
    }
}
