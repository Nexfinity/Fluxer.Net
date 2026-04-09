namespace Fluxer.Net;

public interface IMessageReaction
{
    /// <summary>
    /// The emoji used for the reaction.
    /// </summary>
    IEmoji Emoji { get; }

    /// <summary>
    /// The total number of times this reaction has been used.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Whether the current user has reacted with this emoji.
    /// </summary>
    bool? Me { get; }
}
