namespace Fluxer.Net;

/// <summary>
/// A builder for creating embed authors with validation.
/// Based on Discord.Net's EmbedAuthorBuilder implementation.
/// </summary>
public class EmbedAuthorBuilder
{
    /// <summary>
    /// Maximum length of the author name.
    /// </summary>
    public const int MaxAuthorNameLength = 256;

    private string? _name;

    /// <summary>
    /// Gets or sets the name of the author.
    /// </summary>
    /// <exception cref="ArgumentException">Name length exceeds <see cref="MaxAuthorNameLength"/>.</exception>
    public string? Name
    {
        get => _name;
        set
        {
            if (value?.Length > MaxAuthorNameLength)
                throw new ArgumentException($"Author name length must be less than or equal to {MaxAuthorNameLength}.", nameof(Name));
            _name = value;
        }
    }

    /// <summary>
    /// Gets or sets the URL of the author.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the icon URL of the author.
    /// </summary>
    public string? IconUrl { get; set; }

    /// <summary>
    /// Sets the name of the author.
    /// </summary>
    /// <param name="name">The name to set.</param>
    /// <returns>The current builder.</returns>
    public EmbedAuthorBuilder WithName(string name)
    {
        Name = name;
        return this;
    }

    /// <summary>
    /// Sets the URL of the author.
    /// </summary>
    /// <param name="url">The URL to set.</param>
    /// <returns>The current builder.</returns>
    public EmbedAuthorBuilder WithUrl(string url)
    {
        Url = url;
        return this;
    }

    /// <summary>
    /// Sets the icon URL of the author.
    /// </summary>
    /// <param name="iconUrl">The icon URL to set.</param>
    /// <returns>The current builder.</returns>
    public EmbedAuthorBuilder WithIconUrl(string iconUrl)
    {
        IconUrl = iconUrl;
        return this;
    }

    /// <summary>
    /// Builds the embed author.
    /// </summary>
    /// <returns>A new <see cref="EmbedAuthor"/> object.</returns>
    public EmbedAuthor Build()
    {
        return new EmbedAuthor
        {
            Name = Name,
            Url = Url,
            IconUrl = IconUrl
        };
    }
}
