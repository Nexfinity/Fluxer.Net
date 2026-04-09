namespace Fluxer.Net;

public interface IMessageReference
{
    /// <summary>
    /// The ID of the channel containing the referenced message
    /// </summary>
    ulong ChannelId { get; }

    /// <summary>
    /// The ID of the referenced message
    /// </summary>
    ulong MessageId { get; }

    /// <summary>
    /// The ID of the guild containing the referenced message
    /// </summary>
    ulong? GuildId { get; }

    /// <summary>
    /// The type of message reference.
    /// </summary>
    MessageReferenceType Type { get; }
}
