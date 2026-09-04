namespace Fluxer.Net;

/// <inheritdoc />
public class MessageCall : Entity, IMessageCall
{
    /// <inheritdoc />
    public HashSet<ulong> Participants { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? EndedAt { get; private set; }

    internal MessageCall(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a MessageCall object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static MessageCall Create(FluxerBaseClient client, MessageCallJson json)
    {
        MessageCall data = new MessageCall(client);
        data.Update(json);
        return data;
    }

    internal void Update(MessageCallJson json)
    {
        Participants = json.Participants;
        EndedAt = json.EndedAt;
    }
}
