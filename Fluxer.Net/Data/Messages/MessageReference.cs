namespace Fluxer.Net;

/// <inheritdoc />
public class MessageReference : Entity, IMessageReference
{
    /// <inheritdoc />
    public ulong ChannelId { get; private set; }

    /// <inheritdoc />
    public ulong MessageId { get; private set; }

    /// <inheritdoc />
    public ulong? GuildId { get; private set; }

    /// <inheritdoc />
    public MessageReferenceType Type { get; private set; }

    internal MessageReference(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a MessageReference object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static MessageReference Create(FluxerBaseClient client, MessageReferenceJson json)
    {
        MessageReference data = new MessageReference(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, MessageReferenceJson json)
    {
        ChannelId = json.ChannelId;
        MessageId = json.MessageId;
        GuildId = json.GuildId;
        Type = json.Type;
    }
}
