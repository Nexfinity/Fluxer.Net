namespace Fluxer.Net;

/// <inheritdoc />
public class MessageReaction : Entity, IMessageReaction
{
    /// <inheritdoc />
    public Emoji Emoji { get; internal set; }

    /// <inheritdoc />
    public int Count { get; internal set; }

    /// <inheritdoc />
    public bool? Me { get; internal set; }

    IEmoji IMessageReaction.Emoji => Emoji;

    internal MessageReaction(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a MessageReaction object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static MessageReaction Create(FluxerBaseClient client, MessageReactionJson json)
    {
        MessageReaction data = new MessageReaction(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, MessageReactionJson json)
    {
        Emoji = Emoji.Create(client, json.Emoji);
        Count = json.Count;
        Me = json.Me;
    }
}
