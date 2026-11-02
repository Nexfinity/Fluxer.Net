namespace Fluxer.Net.Commands;

/// <summary>
/// Extension methods for <see cref="SocketMessage"/> that check for a command prefix.
/// </summary>
public static class MessageExtensions
{
    /// <summary>
    /// Checks whether the message starts with the given character.
    /// </summary>
    /// <param name="msg">The message to check.</param>
    /// <param name="c">The character prefix.</param>
    /// <param name="argPos">Set to 1 when the message starts with the character, which is where the command text begins.</param>
    /// <returns>
    /// <see langword="true" /> if the message begins with <paramref name="c"/>. Otherwise <see langword="false" />.
    /// </returns>
    public static bool HasCharPrefix(this SocketMessage msg, char c, ref int argPos)
    {
        var text = msg.Content;
        if (!string.IsNullOrEmpty(text) && text[0] == c)
        {
            argPos = 1;
            return true;
        }
        return false;
    }
    /// <summary>
    /// Checks whether the message starts with the given string.
    /// </summary>
    /// <param name="msg">The message to check.</param>
    /// <param name="str">The string prefix.</param>
    /// <param name="argPos">Set to the length of <paramref name="str"/> when the message starts with it, which is where the command text begins.</param>
    /// <param name="comparisonType">The comparison used to match the prefix.</param>
    /// <returns>
    /// <see langword="true" /> if the message begins with <paramref name="str"/>. Otherwise <see langword="false" />.
    /// </returns>
    public static bool HasStringPrefix(this SocketMessage msg, string str, ref int argPos, StringComparison comparisonType = StringComparison.Ordinal)
    {
        var text = msg.Content;
        if (!string.IsNullOrEmpty(text) && text.StartsWith(str, comparisonType))
        {
            argPos = str.Length;
            return true;
        }
        return false;
    }
    // <summary>
    //     Gets whether the message starts with the user's mention string.
    // </summary>
    //public static bool HasMentionPrefix(this SocketMessage msg, IUser user, ref int argPos)
    //{
    //    var text = msg.Content;
    //    if (string.IsNullOrEmpty(text) || text.Length <= 3 || text[0] != '<' || text[1] != '@')
    //        return false;

    //    int endPos = text.IndexOf('>');
    //    if (endPos == -1)
    //        return false;
    //    if (text.Length < endPos + 2 || text[endPos + 1] != ' ')
    //        return false; //Must end in "> "

    //    if (!MentionUtils.TryParseUser(text.Substring(0, endPos + 1), out ulong userId))
    //        return false;
    //    if (userId == user.Id)
    //    {
    //        argPos = endPos + 2;
    //        return true;
    //    }
    //    return false;
    //}
}
