using System.Globalization;

namespace Fluxer.Net;

/// <summary>
/// Builds and parses user, role and channel mentions.
/// </summary>
public static class MentionUtils
{
    /// <summary>
    /// Returns the mention for a user.
    /// </summary>
    public static string MentionUser(ulong userId) => $"<@{userId}>";

    /// <summary>
    /// Returns the mention for a role.
    /// </summary>
    public static string MentionRole(ulong roleId) => $"<@&{roleId}>";

    /// <summary>
    /// Returns the mention for a channel.
    /// </summary>
    public static string MentionChannel(ulong channelId) => $"<#{channelId}>";

    /// <summary>
    /// Reads the user id from a user mention.
    /// </summary>
    public static bool TryParseUser(string text, out ulong userId)
    {
        if (TryParse(text, "<@!", out userId))
            return true;
        return !text.StartsWith("<@&", StringComparison.Ordinal) && TryParse(text, "<@", out userId);
    }

    /// <summary>
    /// Reads the role id from a role mention.
    /// </summary>
    public static bool TryParseRole(string text, out ulong roleId) => TryParse(text, "<@&", out roleId);

    /// <summary>
    /// Reads the channel id from a channel mention.
    /// </summary>
    public static bool TryParseChannel(string text, out ulong channelId) => TryParse(text, "<#", out channelId);

    /// <summary>
    /// Reads the user id from a user mention.
    /// </summary>
    /// <exception cref="ArgumentException">The text is not a user mention.</exception>
    public static ulong ParseUser(string text) =>
        TryParseUser(text, out ulong id) ? id : throw new ArgumentException("Invalid user mention.", nameof(text));

    /// <summary>
    /// Reads the role id from a role mention.
    /// </summary>
    /// <exception cref="ArgumentException">The text is not a role mention.</exception>
    public static ulong ParseRole(string text) =>
        TryParseRole(text, out ulong id) ? id : throw new ArgumentException("Invalid role mention.", nameof(text));

    /// <summary>
    /// Reads the channel id from a channel mention.
    /// </summary>
    /// <exception cref="ArgumentException">The text is not a channel mention.</exception>
    public static ulong ParseChannel(string text) =>
        TryParseChannel(text, out ulong id) ? id : throw new ArgumentException("Invalid channel mention.", nameof(text));

    private static bool TryParse(string text, string prefix, out ulong id)
    {
        id = 0;
        if (text.Length < prefix.Length + 2 || !text.StartsWith(prefix, StringComparison.Ordinal) || text[text.Length - 1] != '>')
            return false;

        return ulong.TryParse(text.Substring(prefix.Length, text.Length - prefix.Length - 1), NumberStyles.None,
            CultureInfo.InvariantCulture, out id);
    }
}
