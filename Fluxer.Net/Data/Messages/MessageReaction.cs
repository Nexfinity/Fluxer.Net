namespace Fluxer.Net;

/// <inheritdoc />
public class MessageReaction : Entity, IMessageReaction
{
    /// <inheritdoc />
    public Emoji Emoji { get; private set; }

    /// <inheritdoc />
    public int Count { get; private set; }

    /// <inheritdoc />
    public bool? Me { get; private set; }

    IEmoji IMessageReaction.Emoji => Emoji;

    internal MessageReaction(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates a message reaction from JSON.
    /// </summary>
    public static MessageReaction Create(FluxerBaseClient client, MessageReactionJson json)
    {
        MessageReaction data = new MessageReaction(client);
        data.Update(json);
        return data;
    }

    internal void Update(MessageReactionJson json)
    {
        Emoji = Emoji.Create(Client, json.Emoji);
        Count = json.Count;
        Me = json.Me;
    }
}
