using Fluxer.Net.Rest.Requests;

namespace Fluxer.Net;

/// <summary>
/// A builder for creating embed footers with validation.
/// Based on Discord.Net's EmbedFooterBuilder implementation.
/// </summary>
public class EmbedFooterBuilder
{
    /// <summary>
    /// Maximum length of the footer text.
    /// </summary>
    public static int MaxFooterTextLength { get; } = 2048;

    private string? _text;

    /// <summary>
    /// Gets or sets the text of the footer.
    /// </summary>
    /// <exception cref="ArgumentException">Text length exceeds <see cref="MaxFooterTextLength"/>.</exception>
    public string? Text
    {
        get => _text;
        set
        {
            if (value?.Length > MaxFooterTextLength)
                throw new ArgumentException($"Footer text length must be less than or equal to {MaxFooterTextLength}.", nameof(Text));
            _text = value;
        }
    }

    /// <summary>
    /// Gets or sets the icon URL of the footer.
    /// </summary>
    public string? IconUrl { get; set; }

    /// <summary>
    /// Sets the text of the footer.
    /// </summary>
    /// <param name="text">The text to set.</param>
    /// <returns>The current builder.</returns>
    public EmbedFooterBuilder WithText(string text)
    {
        Text = text;
        return this;
    }

    /// <summary>
    /// Sets the icon URL of the footer.
    /// </summary>
    /// <param name="iconUrl">The icon URL to set.</param>
    /// <returns>The current builder.</returns>
    public EmbedFooterBuilder WithIconUrl(string iconUrl)
    {
        IconUrl = iconUrl;
        return this;
    }

    /// <summary>
    /// Builds the embed footer.
    /// </summary>
    /// <returns>A new <see cref="EmbedFooterJson"/> object.</returns>
    public EmbedFooterRequest Build()
    {
        return new EmbedFooterRequest
        {
            Text = Text,
            IconUrl = IconUrl
        };
    }
}
