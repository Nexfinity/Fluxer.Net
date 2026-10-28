namespace Fluxer.Net;

/// <summary>
/// A list of threads, with the current user's thread member for each thread it joined.
/// </summary>
public class ThreadList : Entity
{
    /// <summary>
    /// The threads.
    /// </summary>
    public IReadOnlyList<ThreadChannel> Threads { get; private set; } = Array.Empty<ThreadChannel>();

    /// <summary>
    /// The current user's thread member for each listed thread it joined.
    /// </summary>
    public IReadOnlyList<ThreadMember> Members { get; private set; } = Array.Empty<ThreadMember>();

    /// <summary>
    /// Whether more threads can be fetched. Always false for active threads.
    /// </summary>
    public bool HasMore { get; private set; }

    /// <summary>
    /// The total number of threads matching a search, or null when the list is not a search result.
    /// </summary>
    public int? TotalResults { get; private set; }

    internal ThreadList(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a ThreadList object from json.
    /// </summary>
    /// <returns><see cref="ThreadList"/></returns>
    public static ThreadList Create(FluxerBaseClient client, ThreadListJson json)
    {
        ThreadList data = new ThreadList(client);
        data.Update(json);
        return data;
    }

    internal void Update(ThreadListJson json)
    {
        Threads = json.Threads.Select(x => Channel.Create(Client, x)).OfType<ThreadChannel>().ToArray();
        Members = json.Members.Select(x => ThreadMember.Create(Client, x)).ToArray();
        HasMore = json.HasMore;
        TotalResults = json.TotalResults;
    }
}
