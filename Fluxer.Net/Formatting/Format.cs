using System.Text;

namespace Fluxer.Net;

/// <summary>
/// Helpers for writing Fluxer markdown.
/// </summary>
public static class Format
{
    private static readonly string[] SensitiveCharacters = { "\\", "*", "_", "~", "`", ".", ":", "/", ">", "|" };

    /// <summary>
    /// Returns the text in bold.
    /// </summary>
    /// <param name="text">The text to format.</param>
    public static string Bold(string text) => $"**{text}**";

    /// <summary>
    /// Returns the text in italics.
    /// </summary>
    /// <param name="text">The text to format.</param>
    public static string Italics(string text) => $"*{text}*";

    /// <summary>
    /// Returns the text underlined.
    /// </summary>
    /// <param name="text">The text to format.</param>
    public static string Underline(string text) => $"__{text}__";

    /// <summary>
    /// Returns the text struck through.
    /// </summary>
    /// <param name="text">The text to format.</param>
    public static string Strikethrough(string text) => $"~~{text}~~";

    /// <summary>
    /// Returns the text hidden behind a spoiler.
    /// </summary>
    /// <param name="text">The text to format.</param>
    public static string Spoiler(string text) => $"||{text}||";

    /// <summary>
    /// Returns the text as a heading of the given level, from 1 to 3.
    /// </summary>
    /// <param name="text">The text of the heading.</param>
    /// <param name="level">The heading level. Values outside 1 to 3 are clamped.</param>
    public static string Header(string text, int level = 1) => $"{new string('#', Math.Clamp(level, 1, 3))} {text}";

    /// <summary>
    /// Returns the text as a subtext line, which starts with <c>-#</c>.
    /// </summary>
    /// <param name="text">The text of the line.</param>
    public static string Subtext(string text) => $"-# {text}";

    /// <summary>
    /// Returns a markdown link with the given text and target.
    /// </summary>
    /// <param name="text">The link text.</param>
    /// <param name="url">The link target.</param>
    public static string Url(string text, string url) => $"[{text}]({url})";

    /// <summary>
    /// Returns the URL wrapped in angle brackets, so that it shows no embed preview.
    /// </summary>
    /// <param name="url">The URL to wrap.</param>
    public static string EscapeUrl(string url) => $"<{url}>";

    /// <summary>
    /// Returns the text as inline code, or as a code block when a language is given or the text spans lines.
    /// </summary>
    /// <param name="text">The code.</param>
    /// <param name="language">The language of the code block.</param>
    public static string Code(string text, string? language = null)
    {
        if (language != null || text.Contains("\n"))
            return $"```{language ?? ""}\n{text}\n```";
        return $"`{text}`";
    }

    /// <summary>
    /// Returns every line of the text as a block quote. Empty or whitespace text is returned unchanged.
    /// </summary>
    /// <param name="text">The text to quote.</param>
    public static string Quote(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        StringBuilder result = new();
        string[] lines = text.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            if (i > 0)
                result.Append('\n');
            result.Append("> ").Append(lines[i]);
        }

        return result.ToString();
    }

    /// <summary>
    /// Returns the rest of the message from this point as a block quote. Empty or whitespace text is returned unchanged.
    /// </summary>
    /// <param name="text">The text to quote.</param>
    public static string BlockQuote(string text) => string.IsNullOrWhiteSpace(text) ? text : $">>> {text}";

    /// <summary>
    /// Puts a backslash before each markdown character in the text, and before <c>.</c>, <c>:</c> and <c>/</c>.
    /// </summary>
    /// <param name="text">The text to escape.</param>
    public static string Sanitize(string text)
    {
        foreach (string character in SensitiveCharacters)
            text = text.Replace(character, "\\" + character);
        return text;
    }
}
