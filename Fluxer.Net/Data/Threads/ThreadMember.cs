namespace Fluxer.Net;

/// <summary>
/// A user's membership in a thread.
/// </summary>
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

    /// <summary>
    /// Whether the user has muted the thread. Only known for the current user.
    /// </summary>
    public bool? Muted { get; private set; }

    /// <inheritdoc />
    public GuildMember? Member { get; private set; }

    IGuildMember? IThreadMember.Member => Member;

    internal ThreadMember(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates a thread member from its JSON representation.
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
        Muted = json.Muted ?? Muted;
        if (json.Member != null)
            Member = GuildMember.Create(Client, json.Member);
    }
}
