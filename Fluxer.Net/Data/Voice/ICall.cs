namespace Fluxer.Net;

/// <summary>
/// A voice call in a channel.
/// </summary>
public interface ICall
{
    /// <summary>
    /// The channel the call is in.
    /// </summary>
    ulong ChannelId { get; }

    /// <summary>
    /// The id of the message that started the call.
    /// </summary>
    ulong MessageId { get; }

    /// <summary>
    /// The voice region the call uses, or null when none is set.
    /// </summary>
    string? Region { get; }

    /// <summary>
    /// The ids of the users the call is ringing.
    /// </summary>
    ulong[] RingingUsers { get; }

    /// <summary>
    /// The voice states of the users in the call.
    /// </summary>
    IVoiceState[] VoiceStates { get; }

    /// <summary>
    /// The ids of the call's recipients, or null when they are not sent.
    /// </summary>
    ulong[]? Recipients { get; }

    /// <summary>
    /// When the call was created, or null when it is not sent.
    /// </summary>
    DateTimeOffset? CreatedAt { get; }
}
