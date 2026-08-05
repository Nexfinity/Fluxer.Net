namespace Fluxer.Net;

/// <inheritdoc />
public class MessageCall : Entity, IMessageCall
{
    /// <inheritdoc />
    public HashSet<ulong> Participants { get; internal set; }

    /// <inheritdoc />
    public DateTime? EndedAt { get; internal set; }

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
        var data = new MessageCall(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, MessageCallJson json)
    {
        Participants = json.Participants;
        EndedAt = json.EndedAt;
    }
}
