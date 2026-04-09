namespace Fluxer.Net;

public interface IGuildPermissions
{
    /// <summary>
    /// Grants all permissions and bypasses channel restrictions. Highly sensitive.
    /// </summary>
    bool Administrator { get; }

    /// <summary>
    /// Read the community's audit log of changes and moderation actions.
    /// </summary>
    bool ViewAuditLog { get; }

    /// <summary>
    /// Edit global settings like name, description, and icon.
    /// </summary>
    bool ManageGuild { get; }

    /// <summary>
    /// Kick users from the guild.
    /// </summary>
    bool KickMembers { get; }

    /// <summary>
    /// Ban users from joining the guild.
    /// </summary>
    bool BanMembers { get; }

    /// <summary>
    /// Update your own nickname.
    /// </summary>
    bool ChangeNickname { get; }

    /// <summary>
    /// Change other members nicknames.
    /// </summary>
    bool ManageNicknames { get; }

    /// <summary>
    /// Upload new emojis and stickers, and manage your own creations.
    /// </summary>
    bool CreateExpressions { get; }

    /// <summary>
    /// Edit or delete emojis and stickers created by other members.
    /// </summary>
    bool ManageExpressions { get; }

    /// <summary>
    /// Prevent members from sending messages, reacting, and joining voice for a duration.
    /// </summary>
    bool ModerateMembers { get; }
}
