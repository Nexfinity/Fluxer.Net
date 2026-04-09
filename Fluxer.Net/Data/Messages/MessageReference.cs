namespace Fluxer.Net;

/// <inheritdoc />
public class MessageReference : Entity, IMessageReference
{
    /// <inheritdoc />
    public ulong ChannelId { get; internal set; }

    /// <inheritdoc />
    public ulong MessageId { get; internal set; }

    /// <inheritdoc />
    public ulong? GuildId { get; internal set; }

    /// <inheritdoc />
    public MessageReferenceType Type { get; internal set; }

    internal MessageReference(FluxerBaseClient client) : base(client)
    {

    }

    public static MessageReference Create(FluxerBaseClient client, MessageReferenceJson json)
    {
        var data = new MessageReference(client);
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
