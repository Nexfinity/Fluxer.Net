namespace Fluxer.Net;

/// <summary>
/// Represents the type of a message, including regular messages and system messages.
/// </summary>
public enum MessageType
{
    /// <summary>
    /// A regular user-generated message.
    /// </summary>
    Default = 0,

    /// <summary>
    /// System message indicating a recipient was added to a group DM or thread.
    /// </summary>
    RecipientAdd = 1,

    /// <summary>
    /// System message indicating a recipient was removed from a group DM or thread.
    /// </summary>
    RecipientRemove = 2,

    /// <summary>
    /// System message indicating a call was started.
    /// </summary>
    Call = 3,

    /// <summary>
    /// System message indicating the channel name was changed.
    /// </summary>
    ChannelNameChange = 4,

    /// <summary>
    /// System message indicating the channel icon was changed.
    /// </summary>
    ChannelIconChange = 5,

    /// <summary>
    /// System message indicating a message was pinned.
    /// </summary>
    ChannelPinnedMessage = 6,

    /// <summary>
    /// System message indicating a user joined the guild.
    /// </summary>
    UserJoin = 7,

    /// <summary>
    /// A message that is a reply to another message.
    /// </summary>
    Reply = 19,

    /// <summary>
    /// System message.
    /// </summary>
    SystemMessage = 99,
}
