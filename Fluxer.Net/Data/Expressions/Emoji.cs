namespace Fluxer.Net;

/// <inheritdoc />
public class Emoji : Entity, IEmoji
{
    /// <inheritdoc />
    public ulong? Id { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt => Id.HasValue ? SnowflakeUtils.FromSnowflake(Id.Value) : null;

    /// <inheritdoc />
    public string Name { get; private set; }

    /// <inheritdoc />
    public bool IsAnimated { get; private set; }

    /// <inheritdoc />
    public bool AllowCloning { get; private set; }

    internal string ToRequest()
    {
        if (Id.HasValue)
            return $"{Name}:{Id}";
        return Name;
    }

    /// <inheritdoc />
    public Emoji(string name) : base(null)
    {
        Name = name;
    }

    /// <inheritdoc />
    public Emoji(ulong id, string name, bool isAnimated = false) : base(null)
    {
        Name = name;
        Id = id;
        IsAnimated = isAnimated;
    }

    /// <summary>
    /// Whether this is a custom emoji rather than a unicode one.
    /// </summary>
    public bool IsCustom => Id.HasValue;

    /// <summary>
    /// Creates a unicode emoji.
    /// </summary>
    /// <exception cref="ArgumentException">The text is empty.</exception>
    public static Emoji FromUnicode(string emoji)
    {
        if (string.IsNullOrWhiteSpace(emoji))
            throw new ArgumentException("An emoji cannot be empty.", nameof(emoji));
        return new Emoji(emoji.Trim());
    }

    /// <summary>
    /// Creates a custom emoji from its name and id.
    /// </summary>
    public static Emoji FromCustom(string name, ulong id, bool animated = false) => new(id, name, animated);

    /// <summary>
    /// Reads a custom emoji from its message markup.
    /// </summary>
    public static bool TryParseCustom(string? text, out Emoji emoji)
    {
        emoji = null!;
        if (text == null)
            return false;

        string value = text.Trim();
        bool animated = value.StartsWith("<a:", StringComparison.Ordinal);
        if ((!animated && !value.StartsWith("<:", StringComparison.Ordinal)) || !value.EndsWith(">", StringComparison.Ordinal))
            return false;

        string body = value.Substring(animated ? 3 : 2, value.Length - (animated ? 4 : 3));
        int split = body.LastIndexOf(':');
        if (split <= 0 || !ulong.TryParse(body.Substring(split + 1), System.Globalization.NumberStyles.None,
                System.Globalization.CultureInfo.InvariantCulture, out ulong id))
            return false;

        emoji = FromCustom(body.Substring(0, split), id, animated);
        return true;
    }

    /// <summary>
    /// Reads a custom emoji, or a unicode emoji.
    /// </summary>
    public static bool TryParse(string? text, out Emoji emoji)
    {
        if (TryParseCustom(text, out emoji))
            return true;

        string value = text?.Trim() ?? "";
        if (value.Length == 0 || value.Any(char.IsWhiteSpace) || value.All(c => c < 128))
            return false;

        emoji = FromUnicode(value);
        return true;
    }

    /// <summary>
    /// Reads a custom or unicode emoji.
    /// </summary>
    /// <exception cref="FormatException">The text is not an emoji.</exception>
    public static Emoji Parse(string text) =>
        TryParse(text, out Emoji emoji) ? emoji : throw new FormatException($"'{text}' is not an emoji.");

    /// <summary>
    /// Gets an <see cref="Emoji"/> for any <see cref="IEmoji"/>.
    /// </summary>
    public static Emoji From(IEmoji emoji) => emoji switch
    {
        Emoji known => known,
        { Id: { } id } => FromCustom(emoji.Name, id, emoji.IsAnimated),
        _ => FromUnicode(emoji.Name)
    };

    /// <summary>
    /// Whether another emoji is the same emoji, ignoring variation selectors on unicode emojis.
    /// </summary>
    public bool Matches(IEmoji? other)
    {
        if (other is null)
            return false;
        if (IsCustom || other.Id.HasValue)
            return Id == other.Id;
        return StripVariationSelectors(Name) == StripVariationSelectors(other.Name);
    }

    private static string StripVariationSelectors(string? name) =>
        name is null ? string.Empty : name.Replace("\uFE0F", string.Empty).Replace("\uFE0E", string.Empty);

    /// <summary>
    /// Returns the emoji as message markup.
    /// </summary>
    public override string ToString()
    {
        if (!IsCustom)
            return Name;
        return IsAnimated ? $"<a:{Name}:{Id}>" : $"<:{Name}:{Id}>";
    }

    internal Emoji(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a Emoji object from json.
    /// </summary>
    /// <returns><see cref="Emoji"/></returns>
    public static Emoji Create(FluxerBaseClient client, EmojiJson json)
    {
        Emoji data = new Emoji(client);
        data.Update(json);
        return data;
    }

    internal void Update(EmojiJson json)
    {
        Id = json.Id;
        Name = json.Name;
        IsAnimated = json.IsAnimated;
        AllowCloning = json.AllowCloning;
    }
}
