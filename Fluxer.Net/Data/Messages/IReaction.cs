namespace Fluxer.Net;

/// <summary>
/// A reaction added to a message by a user.
/// </summary>
public interface IReaction
{
    /// <summary>
    /// The ID of the user who reacted.
    /// </summary>
    ulong UserId { get; }
    /// <summary>
    /// The ID of the message that was reacted to.
    /// </summary>
    ulong MessageId { get; }
    /// <summary>
    /// The emoji used for the reaction.
    /// </summary>
    IEmoji Emoji { get; }
}
