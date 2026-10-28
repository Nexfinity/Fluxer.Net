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
    public static string Bold(string text) => $"**{text}**";

    /// <summary>
    /// Returns the text in italics.
    /// </summary>
    public static string Italics(string text) => $"*{text}*";

    /// <summary>
    /// Returns the text underlined.
    /// </summary>
    public static string Underline(string text) => $"__{text}__";

    /// <summary>
    /// Returns the text struck through.
    /// </summary>
    public static string Strikethrough(string text) => $"~~{text}~~";

    /// <summary>
    /// Returns the text hidden behind a spoiler.
    /// </summary>
    public static string Spoiler(string text) => $"||{text}||";

    /// <summary>
    /// Returns the text as a heading of the given level, from 1 to 3.
    /// </summary>
    public static string Header(string text, int level = 1) => $"{new string('#', Math.Clamp(level, 1, 3))} {text}";

    /// <summary>
    /// Returns the text as small, muted subtext.
    /// </summary>
    public static string Subtext(string text) => $"-# {text}";

    /// <summary>
    /// Returns the text as a markdown link.
    /// </summary>
    public static string Url(string text, string url) => $"[{text}]({url})";

    /// <summary>
    /// Returns the url wrapped so it shows no embed preview.
    /// </summary>
    public static string EscapeUrl(string url) => $"<{url}>";

    /// <summary>
    /// Returns the text as inline code, or as a code block when a language is given or the text spans lines.
    /// </summary>
    public static string Code(string text, string? language = null)
    {
        if (language != null || text.Contains("\n"))
            return $"```{language ?? ""}\n{text}\n```";
        return $"`{text}`";
    }

    /// <summary>
    /// Returns every line of the text as a block quote.
    /// </summary>
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
    /// Returns the rest of the message from this point as a block quote.
    /// </summary>
    public static string BlockQuote(string text) => string.IsNullOrWhiteSpace(text) ? text : $">>> {text}";

    /// <summary>
    /// Escapes the markdown characters in the text.
    /// </summary>
    public static string Sanitize(string text)
    {
        foreach (string character in SensitiveCharacters)
            text = text.Replace(character, "\\" + character);
        return text;
    }
}
